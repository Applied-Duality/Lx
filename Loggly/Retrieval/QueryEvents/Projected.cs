#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Loggly; 
namespace Loggly.Retrieval
{
    public struct ProjectedEvents
    {
        HttpClient _client;
        Func<FilteredEvents.Event, FilteredEvents.Bool> _pattern;
        Func<DatedEvents.Event, DatedEvents.Bool> _timeRange;
        bool _descending;
        Expression<Func<Event, Event>> _selector;

        public ProjectedEvents
            (HttpClient client
            , Func<FilteredEvents.Event, FilteredEvents.Bool> pattern
            , Func<DatedEvents.Event, DatedEvents.Bool> timeRange
            , bool descending
            , Expression<Func<Event, Event>> selector
            )
        {
            _client = client;
            _pattern = pattern;
            _timeRange = timeRange;
            _descending = descending;
            _selector = selector;
        }

        public SkippedEvents Skip(int n)
        {
            return new SkippedEvents(_client, _pattern, _timeRange,  _descending, _selector, n);
        }
        public TakenEvents Take(int n)
        {
            return new TakenEvents(_client, _pattern, _timeRange, _descending, _selector, 0, n);
        }

        public TaskAwaiter<SearchResult[]> GetAwaiter()
        {
            return this.Take(10).GetAwaiter();
        }
    }
}