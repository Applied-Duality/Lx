#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Loggly.Retrieval
{
    public struct TakenEvents<T>
    {
        HttpClient _client;
        Func<FilteredEvents.Event, FilteredEvents.Bool> _pattern;
        Func<DatedEvents.Event, DatedEvents.Bool> _timeRange;
        bool _descending;
        Expression<Func<Event, T>> _selector;
        int _skip;
        int _take;

        public TakenEvents
            (HttpClient client, Func<FilteredEvents.Event, FilteredEvents.Bool> pattern, Func<DatedEvents.Event, DatedEvents.Bool> timeRange, bool descending, Expression<Func<Event, T>> selector, int skip, int take)
        {
            _client = client;
            _pattern = pattern;
            _timeRange = timeRange;
            _descending = descending;
            _selector = selector;
            _skip = skip;
            _take = take;
        }

        async Task<T[]> _GetEventsAsync()
        {
            var query = new Dictionary<string, string>();

            query["q"] = "*";
            if (_pattern != null)
            {
                query["q"] = _pattern(default(FilteredEvents.Event))._pattern;
            }

            if (_timeRange != null)
            {
                var t = _timeRange(default(DatedEvents.Event));
                if(t._start != default(DateTimeOffset))
                    query["from"] = t._start.ToString("yyyy-MM-dd HH:mm:ss.fffzzzz");
                if(t._end != default(DateTimeOffset))
                    query["until"] = t._end.ToString("yyyy-MM-dd HH:mm:ss.fffzzzz");
            }

            if (!_descending)
            {
                query["order"] = "asc";
            }

            if (_selector != null) // should never be null
            {
                var fields = Fields.Collect(_selector.Body);
                if (fields != null)
                {
                    query["fields"] = string.Join(",", fields);
                }
            }

            if (_skip > 0)
            {
                query["start"] = _skip.ToString();
            }

            if (_take > 0 && _take != 10)
            {
                query["rows"] = _take.ToString();
            }

            var queryString = string.Join("&", query.Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)));

            var response = await _client.GetAsync(string.Format("search?{0}", queryString));

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var raw = SearchResults.Parse(results);
                var transform = _selector.Compile();
                return raw.Select(transform).ToArray();
            }
            else
            {
                return new T[]{};
            }
        }

        public TaskAwaiter<T[]> GetAwaiter()
        {
            return _GetEventsAsync().GetAwaiter();
        }

        // Little visitor to analyze projection function
        class Fields : ExpressionVisitor
        {
            internal static List<string> Collect(Expression expr)
            {
                var visitor = new Fields();
                visitor.Visit(expr);
                return visitor._fields;
            }

            internal List<string> _fields = new List<string>();

            Expression parent;
            public override Expression Visit(Expression node)
            {
                if (_fields == null) return node;
                if (node.NodeType != ExpressionType.Parameter)
                {
                    var _parent = parent;
                    parent = node;
                    var result = base.Visit(node);
                    parent = _parent;
                    return result;
                }
                return base.Visit(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (parent != null && parent.NodeType == ExpressionType.MemberAccess)
                {
                    var field = (parent as MemberExpression).Member.Name.ToLowerInvariant();
                    _fields.Add(field);
                }
                else
                {
                    _fields = null; // conservatively assume all members are used
                }
                return node;
            }
        }
    }
}