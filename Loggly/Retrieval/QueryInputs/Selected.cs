#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Loggly.Retrieval
{
    public class SelectedInputs
    {
        HttpClient _client;
        Func<FilteredInputs.Input, FilteredInputs.Bool> _predicate;

        internal SelectedInputs(HttpClient client, Func<FilteredInputs.Input, FilteredInputs.Bool> predicate)
        {
            _client = client;
            _predicate = predicate;
        }

        async Task<HttpInput[]> _GetInputsAsync()
        {
            var query = "";

            if (_predicate != null)
            {
                var b = _predicate(default(FilteredInputs.Input));
                if (!string.IsNullOrWhiteSpace(b._name)) query = string.Format("?name={0}", b._name);
                else if (b._id != 0) query = string.Format("{0}", b._id);
            }

            var response = await _client.GetAsync(string.Format("inputs/{0}",query));

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return HttpInputs.Parse(json);
            }
            else
            {
                return new HttpInput[] { };
            }
        }

        public TaskAwaiter<HttpInput[]> GetAwaiter()
        {
            return _GetInputsAsync().GetAwaiter();
        }

        public struct Input { }
    }
}
