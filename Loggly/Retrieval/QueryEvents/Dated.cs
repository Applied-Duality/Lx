#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;

/// For now only absolute time computed on client
namespace Loggly.Retrieval
{
    public struct DatedEvents
    {
        HttpClient _client;
        Expression<Func<FilteredEvents.Event, FilteredEvents.Bool>> _pattern;
        Expression<Func<DatedEvents.Event, DatedEvents.Bool>> _timeRange;

        public DatedEvents
            (HttpClient client
            , Expression<Func<FilteredEvents.Event, FilteredEvents.Bool>> pattern
            , Expression<Func<DatedEvents.Event, DatedEvents.Bool>> timeRange)
        {
            _client = client;
            _pattern = pattern;
            _timeRange = timeRange;
        }

        public OrderedEvents OrderBy(Expression<Func<OrderedEvents.Event, OrderedEvents.Time>> keySelector)
        {
            return new OrderedEvents(_client, _pattern, _timeRange, false);
        }
        public OrderedEvents OrderByDescending(Expression<Func<OrderedEvents.Event, OrderedEvents.Time>> keySelector)
        {
            return new OrderedEvents(_client, _pattern, _timeRange, true);
        }
        public ProjectedEvents Select(Expression<Func<ProjectedEvents.Event, ProjectedEvents.Event>> selector)
        {
            return new ProjectedEvents(_client, _pattern, _timeRange, true, selector);
        }
        public SkippedEvents Skip(int n)
        {
            return new SkippedEvents(_client, _pattern, _timeRange, true, null, 0);
        }
        public TakenEvents Take(int n)
        {
            return new TakenEvents(_client, _pattern, _timeRange, true, null, 0, 10);
        }
        public TaskAwaiter<string> GetAwaiter()
        {
            return this.Take(10).GetAwaiter();
        }

        public struct Time
        {
            public static Bool operator <=(DateTimeOffset start, Time time)
            {
                return new Bool(start: start);
            }
            public static Bool operator <=(Time time, DateTimeOffset end)
            {
                return new Bool(end: end);
            }
            public static Bool operator >=(DateTimeOffset end, Time time)
            {
                return time <= end;
            }
            public static Bool operator >=(Time time, DateTimeOffset start)
            {
                return start <= time;
            }
        }
        public struct Bool
        {
            public DateTimeOffset _start;
            public DateTimeOffset _end;

            public Bool(DateTimeOffset start = default(DateTimeOffset), DateTimeOffset end = default(DateTimeOffset))
            {
                _start = start; 
                _end = end; 
            }

            public static Bool operator &(Bool left, Bool right)
            {
                var b = new Bool(left.Start, left.End);
                b.Start = right.Start;
                b.End = right.End;
                return b;
            }
            public static bool operator true(Bool p)
            {
                return false;
            }
            public static bool operator false(Bool p)
            {
                return false;
            }

            DateTimeOffset Start
            {
                set
                {
                    if (value == default(DateTimeOffset)) return;
                    if (_start == default(DateTimeOffset) || value < _start) _start = value;
                }
                get { return _start; }
            }
            DateTimeOffset End
            {
                set
                {
                    if (value == default(DateTimeOffset)) return;
                    if (_end == default(DateTimeOffset) || value > _end) _end = value;
                }
                get { return _end; }
            }
        }
        public struct Event
        {
            public Time Time { get { return new Time(); } }
        }
    }
}