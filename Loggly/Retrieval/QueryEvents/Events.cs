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

        public FilteredEvents Where(Expression<Func<FilteredEvents.Event, FilteredEvents.Bool>> pattern)
        {
            return new FilteredEvents(_client, pattern);
        }
        public DatedEvents Where(Expression<Func<DatedEvents.Event, DatedEvents.Bool>> timeRange)
        {
            return new DatedEvents(_client, null, timeRange);
        }
        public OrderedEvents OrderBy(Expression<Func<OrderedEvents.Event, OrderedEvents.Time>> keySelector)
        {
            return new OrderedEvents(_client, null, null, false);
        }
        public OrderedEvents OrderByDescending(Expression<Func<OrderedEvents.Event, OrderedEvents.Time>> keySelector)
        {
            return new OrderedEvents(_client, null, null, true);
        }
        public ProjectedEvents Select(Expression<Func<ProjectedEvents.Event, ProjectedEvents.Event>> selector)
        {
            return new ProjectedEvents(_client, null, null, true, selector);
        }
        public SkippedEvents Skip(int n)
        {
            return new SkippedEvents(_client, null, null, true, null, 0);
        }
        public TakenEvents Take(int n)
        {
            return new TakenEvents(_client, null, null, true, null, 0, 10);
        }

        public TaskAwaiter<SearchResult[]> GetAwaiter()
        {
            return this.Take(10).GetAwaiter();
        }
    }
}