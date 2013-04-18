#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.ComponentModel;
using System.Json;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Loggly;

namespace Loggly.Retrieval
{
    public struct FilteredEvents
    {
        HttpClient _client;
        Func<Event, Bool> _pattern;

        internal FilteredEvents(HttpClient client, Func<Event, Bool> pattern)
        {
            _client = client;
            _pattern = pattern;
        }

        public DatedEvents Where(Func<DatedEvents.Event, DatedEvents.Bool> timeRange)
        {
            return new DatedEvents(_client, _pattern, timeRange);
        }
        public OrderedEvents OrderBy(Func<OrderedEvents.Event, OrderedEvents.Time> keySelector)
        {
            return new OrderedEvents(_client, _pattern, null, false);
        }
        public OrderedEvents OrderByDescending(Func<OrderedEvents.Event, OrderedEvents.Time> keySelector)
        {
            return new OrderedEvents(_client, _pattern, null, true);
        }
        public ProjectedEvents<T> Select<T>(Expression<Func<Loggly.Event, T>> selector)
        {
            return new ProjectedEvents<T>(_client, _pattern, null, true, selector);
        }
        public SkippedEvents<Loggly.Event> Skip(int n)
        {
            return new SkippedEvents<Loggly.Event>(_client, _pattern, null, true, x=>x, n);
        }
        public TakenEvents<Loggly.Event> Take(int n)
        {
            return new TakenEvents<Loggly.Event>(_client, _pattern, null, true, x=>x, 0, n);
        }

        public TaskAwaiter<Loggly.Event[]> GetAwaiter()
        {
            return this.Take(10).GetAwaiter();
        }


        public struct JsonContent
        {
            string _field;
            public JsonContent(string field)
            {
                _field = field;
            }
            public Bool Matches(string pattern)
            {
                return new Bool(string.Format("json.{0}:{1}", _field, pattern));
            }
            public JsonContent this[string field]
            {
                get
                {
                    return new JsonContent(string.Format("{0}.{1}", _field, field));
                }
            }

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
            
            public static Bool operator !=(Name _, string name)
            {
                return new Bool(string.Format("-inputname:{0}", name));
            }

            public static Bool operator ==(string name, Name _)
            {
                return (_ == name);
            }

            public static Bool operator !=(string name, Name _)
            {
                return (_ != name);
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
            public static Bool operator !(Bool left)
            {
                return new Bool(string.Format("-{0}", left._pattern));
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

        //
        // TODO add support for Json search and figure out search API precise semantics and syntax
        // http://loggly.com/support/using-data/search-basics/
        // http://loggly.com/support/using-data/search-guide/
        //
        public struct Event
        {
            public IP Ip { get { return new IP(); } }
            public Pattern Text { get { return new Pattern(); } }
            public Name InputName { get { return new Name(); } }
            public JsonContent this[string field] { get { return new JsonContent(field); } }
        }
    }
}