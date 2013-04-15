#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Loggly.Retrieval
{
    public struct FilteredEvents
    {
        HttpClient _client;
        Expression<Func<Event, Bool>> _pattern;

        internal FilteredEvents(HttpClient client, Expression<Func<Event, Bool>> pattern)
        {
            _client = client;
            _pattern = pattern;
        }

        public DatedEvents Where(Expression<Func<DatedEvents.Event, DatedEvents.Bool>> timeRange)
        {
            return new DatedEvents(_client, _pattern, timeRange);
        }
        public OrderedEvents OrderBy(Expression<Func<OrderedEvents.Event, OrderedEvents.Time>> keySelector)
        {
            return new OrderedEvents(_client, _pattern, null, false);
        }
        public OrderedEvents OrderByDescending(Expression<Func<OrderedEvents.Event, OrderedEvents.Time>> keySelector)
        {
            return new OrderedEvents(_client, _pattern, null, true);
        }
        public ProjectedEvents Select(Expression<Func<ProjectedEvents.Event, ProjectedEvents.Event>> selector)
        {
            return new ProjectedEvents(_client, _pattern, null, true, selector);
        }
        public SkippedEvents Skip(int n)
        {
            return new SkippedEvents(_client, _pattern, null, true, null, 0);
        }
        public TakenEvents Take(int n)
        {
            return new TakenEvents(_client, _pattern, null, true, null, 0, 10);
        }

        public TaskAwaiter<string> GetAwaiter()
        {
            return this.Take(10).GetAwaiter();
        }

        public struct IP
        {
            public Bool Matches(string pattern)
            {
                return new Bool(string.Format("ip:{0}", pattern));
            }
        }
        public struct Pattern
        {
            public Bool Matches(string pattern)
            {
                return new Bool(string.Format("{0}", pattern));
            }
        }
        public struct Name
        {
            public static Bool operator ==(Name _, string name)
            {
                return new Bool(string.Format("inputname:{0}", name));
            }

            public static Bool operator ==(string name, Name _)
            {
                return (_ == name);
            }

            [Obsolete]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static Bool operator !=(Name _, string name)
            {
                throw new NotImplementedException();
            }

            [Obsolete]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static Bool operator !=(string name, Name _)
            {
                throw new NotImplementedException();
            }
        }
        public struct Bool
        {
            public string _pattern;
            public Bool(string pattern) { _pattern = pattern; }

            public static Bool operator &(Bool left, Bool right)
            {
                return new Bool(string.Format("({0} AND {1})", left._pattern, right._pattern));
            }
            public static Bool operator |(Bool left, Bool right)
            {
                return new Bool(string.Format("({0} OR {1})", left._pattern, right._pattern));
            }

            // http://stackoverflow.com/questions/5203093/how-does-operator-overloading-of-true-and-false-work/5203185#5203185
            // so always return false since we want to compute result eagerly on client
            public static bool operator true(Bool p)
            {
                return false;
            }
            public static bool operator false(Bool p)
            {
                return false;
            }
        }
        public struct Event
        {
            public IP Ip { get { return new IP(); } }
            public Pattern Text { get { return new Pattern(); } }
            public Name InputName { get { return new Name(); } }
        }
    }
}