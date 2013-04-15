#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Loggly
{
     
    public enum EventFormat { Other, Text, Json }

    public static class SearchResults
    {
        public static SearchResult[] Parse(string json)
        {
            dynamic _json = JsonValue.Parse(json);
            var xs = _json.data as IEnumerable<JsonValue>;
            return (xs).Select(input => new SearchResult(input)).ToArray();
        }
    }

    public struct SearchResult
    {
        dynamic _json;
        public SearchResult(string json):this(JsonValue.Parse(json)){ }
        public SearchResult(JsonValue json) { this._json = json; }

        public bool IsJson { get { return _json.isjson; } }
        public DateTimeOffset TimeStamp { get { return DateTimeOffset.Parse((string)_json.timestamp); } }
        public string InputName { get { return _json.inputname; } }
        public int InputId { get { return _json.inputid; } }
        public string Ip { get { return _json.ip; } }
        public JsonValue Json { get { return _json.json; } }

        public override string ToString()
        {
            return _json.ToString();
        }
    }

    public struct Service
    {
        dynamic _json;
        public Service(JsonValue json) { this._json = json; }

        public string Name { get { return _json.name; } }
        public string Display { get { return _json.display; } }

        public override string ToString()
        {
            return _json.ToString();
        }
    }

    public struct HttpInput
    {
        dynamic _json;
        public HttpInput(JsonValue json) { this._json = json; }
        public HttpInput(string json):this(JsonValue.Parse(json)) {  }
        public string Description { get { return _json.description; } }
        public int  Id { get { return _json.id; } }
        public Guid InputToken { get { return Guid.Parse(_json.input_token); } }
        public DateTimeOffset Created { get { return DateTimeOffset.Parse(_json.created); } }
        public EventFormat Format
        {
            get
            {
                string format = _json.format;
                return format == "text" ? EventFormat.Text : format == "json" ? EventFormat.Json : EventFormat.Other;
            }
        }
        public string Name { get { return _json.name; } }
        public Uri LoggingUrl 
        { 
            get 
            {
                string uri = _json.logging_url;
                return new Uri(uri);
            } 
        }
        public Service Service { get { return new Service(_json.service); } }

        public override string ToString()
        {
            return _json.ToString();
        }
    }

    public struct _HttpInputs
    {
        dynamic _json;
        internal _HttpInputs(string json) 
        { 
            var p = JsonValue.Parse(json);
            if (p.JsonType != JsonType.Array) p = new JsonArray(p); 
            _json = p;

        }

        public IEnumerable<HttpInput> Inputs
        {
            get 
            {
                var xs = _json as IEnumerable<JsonValue>;
                return (xs).Select(input => new HttpInput(input)); 
            }
        }

        public override string ToString()
        {
            return _json.ToString();
        }
    }
}