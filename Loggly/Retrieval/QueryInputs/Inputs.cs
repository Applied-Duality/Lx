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
    public class Inputs
    {
        HttpClient _client;
        internal Inputs(HttpClient client) { _client = client; }

        public FilteredInputs Where(Expression<Func<FilteredInputs.Input, FilteredInputs.Bool>> predicate)
        {
            return new FilteredInputs(_client, predicate);
        }
        public SelectedInputs Select(Expression<Func<SelectedInputs.Input, SelectedInputs.Input>> selector)
        {
            return new SelectedInputs(_client, null);
        }

        public TaskAwaiter<HttpInput[]> GetAwaiter()
        {
            return this.Select(x => x).GetAwaiter();
        }
    }
}