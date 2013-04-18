#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System.Json;
using System.Net.Http;
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

        public async Task<bool> PostMessageAsync(HttpInput input, JsonValue json)
        {
            var content = new JsonContent(json);
            var response = await _client.PostAsync(input.LoggingUrl, content);
            return response.IsSuccessStatusCode;
        }
    }
}