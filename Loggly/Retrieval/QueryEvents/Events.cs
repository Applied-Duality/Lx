#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Loggly.Retrieval
{
    public struct Events
    {
        HttpClient _client;
        public Events(HttpClient client) { _client = client; }

        public FilteredEvents Where(Func<FilteredEvents.Event, FilteredEvents.Bool> pattern)
        {
            return new FilteredEvents(_client, pattern);
        }
        public DatedEvents Where(Func<DatedEvents.Event, DatedEvents.Bool> timeRange)
        {
            return new DatedEvents(_client, null, timeRange);
        }
        public OrderedEvents OrderBy(Func<OrderedEvents.Event, OrderedEvents.Time> keySelector)
        {
            return new OrderedEvents(_client, null, null, false);
        }
        public OrderedEvents OrderByDescending(Func<OrderedEvents.Event, OrderedEvents.Time> keySelector)
        {
            return new OrderedEvents(_client, null, null, true);
        }
        public ProjectedEvents<T> Select<T>(Expression<Func<Event, T>> selector)
        {
            return new ProjectedEvents<T>(_client, null, null, true, selector);
        }
        public SkippedEvents<Loggly.Event> Skip(int n)
        {
            return new SkippedEvents<Loggly.Event>(_client, null, null, true, x=>x, n);
        }
        public TakenEvents<Loggly.Event> Take(int n)
        {
            return new TakenEvents<Loggly.Event>(_client, null, null, true, x=>x, 0, n);
        }

        public TaskAwaiter<Event[]> GetAwaiter()
        {
            return this.Take(10).GetAwaiter();
        }
    }
}