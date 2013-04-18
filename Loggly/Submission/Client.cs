#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.Json;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;

namespace Loggly.Submission
{
    public class Client
    {
        HttpClient _client;
        public Client()
        {
            _client = new HttpClient();
        }

        public IObserver<T> CreateObserver<T>(T dummy, HttpInput input)
        {
            return Observer.Create<T>(t =>
            {
                this.PostMessageAsync(input, t).ToBackGround();
            });
        }

        public Task<bool> PostMessageAsync<T>(HttpInput input, T value)
        {
            return PostMessageAsync(input, value.Serialize());
        }

        public Task<bool> PostMessageAsync(HttpInput input, JsonValue json)
        {
            return PostMessageAsync(input, json.ToString());
        }

        public async Task<bool> PostMessageAsync(HttpInput input, string json)
        {
            var content = new JsonContent(json);
            try
            {
                var response = await _client.PostAsync(input.LoggingUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}