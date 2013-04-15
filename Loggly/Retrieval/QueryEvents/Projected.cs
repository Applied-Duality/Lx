#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Loggly.Retrieval
{
    public struct ProjectedEvents
    {
        HttpClient _client;
        Expression<Func<FilteredEvents.Event, FilteredEvents.Bool>> _pattern;
        Expression<Func<DatedEvents.Event, DatedEvents.Bool>> _timeRange;
        bool _descending;
        Expression<Func<ProjectedEvents.Event, ProjectedEvents.Event>> _selector;

        public ProjectedEvents
            (HttpClient client
            , Expression<Func<FilteredEvents.Event, FilteredEvents.Bool>> pattern
            , Expression<Func<DatedEvents.Event, DatedEvents.Bool>> timeRange
            , bool descending
            , Expression<Func<ProjectedEvents.Event, ProjectedEvents.Event>> selector
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
        
        public TaskAwaiter<string> GetAwaiter()
        {
            return this.Take(10).GetAwaiter();
        }

        // id', 'timestamp', 'ip', 'inputname', 'text'.
        public struct Event
        {
            public int Id;
            public DateTimeOffset TimeStamp;
            public string Ip;
            public string Text;
            public string InputName;
        }
    }
}