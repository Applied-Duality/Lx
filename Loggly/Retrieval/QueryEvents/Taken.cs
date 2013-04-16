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
    public struct TakenEvents
    {
        HttpClient _client;
        Expression<Func<FilteredEvents.Event, FilteredEvents.Bool>> _pattern;
        Expression<Func<DatedEvents.Event, DatedEvents.Bool>> _timeRange;
        bool _descending;
        Expression<Func<ProjectedEvents.Event, ProjectedEvents.Event>> _selector;
        int _skip;
        int _take;

        public TakenEvents
            (HttpClient client, Expression<Func<FilteredEvents.Event, FilteredEvents.Bool>> pattern, Expression<Func<DatedEvents.Event, DatedEvents.Bool>> timeRange, bool descending, Expression<Func<ProjectedEvents.Event, ProjectedEvents.Event>> selector, int skip, int take)
        {
            _client = client;
            _pattern = pattern;
            _timeRange = timeRange;
            _descending = descending;
            _selector = selector;
            _skip = skip;
            _take = take;
        }

        async Task<SearchResult[]> _GetEventsAsync()
        {
            var query = new Dictionary<string, string>();

            if (_pattern != null) query["q"] = _pattern.Compile()(default(FilteredEvents.Event))._pattern;
            else query["q"] = "*";

            if (_timeRange != null)
            {
                var t = _timeRange.Compile()(default(DatedEvents.Event));
                if(t._start != default(DateTimeOffset))
                    query["from"] = t._start.ToString("yyyy-MM-dd HH:mm:ss.fffzzzz");
                if(t._end != default(DateTimeOffset))
                    query["until"] = t._end.ToString("yyyy-MM-dd HH:mm:ss.fffzzzz");
            }

            if (!_descending) query["order"] = "asc";

            if (_selector != null)
            {
                try
                {
                    var fields = string.Join(",",
                            (_selector.Body as MemberInitExpression)
                            .Bindings
                            .Select(mb => mb.Member.Name.ToLowerInvariant()));
                    query["fields"] = fields;
                }
                catch { }
            }

            if(_skip > 0) query["start"] = _skip.ToString();

            if (_take > 0 && _take != 10) query["rows"] = _take.ToString();

            var queryString = string.Join("&", query.Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)));

            var response = await _client.GetAsync(string.Format("search?{0}", queryString));

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                return SearchResults.Parse(results);
            }
            else
            {
                return new SearchResult[]{};
            }
        }

        public TaskAwaiter<SearchResult[]> GetAwaiter()
        {
            return _GetEventsAsync().GetAwaiter();
        }
    }
}