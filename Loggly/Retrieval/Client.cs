#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Loggly.Retrieval
{
    public partial class Client
    {
        HttpClient _client;
        public Client(string username, string password, string subdomain)
        {
            _client = new HttpClient();
            var header = new AuthenticationHeaderValue
                            ( "Basic"
                            , Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password)))
                            );
            _client.DefaultRequestHeaders.Authorization = header;
            var baseUri = new Uri(string.Format("https://{0}.loggly.com/api/",subdomain));
            _client.BaseAddress = baseUri;
        }

        public Inputs GetInputsAsync()
        {
            return new Inputs(_client);
        }

        public async Task<HttpInput?> CreateHttpInputAsync(string name, string description = "", EventFormat format = EventFormat.Json)
        {
            var content = new FormUrlEncodedContent
            (new Dictionary<string, string>
            {
                {"name" , name},
                {"description", description},
                {"service", "http" },
                { "format", "json" },
            });
            var response = await _client.PostAsync("inputs/", content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return new HttpInput(json);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteHttpInputAsync(HttpInput input)
        {
            var response = await _client.DeleteAsync(string.Format("inputs/{0}", input.Id));
            return response.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<string> SearchAsync(string query)
        {
            var response = await _client.GetAsync(string.Format("search?{0}",query));
            return await response.Content.ReadAsStringAsync();
        }

        public Events QueryEventsAsync()
        {
            return new Loggly.Retrieval.Events(_client);
        }
    }
}