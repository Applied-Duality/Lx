#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;

// http://json2csharp.com/
namespace Loggly
{
    public enum EventFormat { Other, Text, Json }

    public static class SearchResults
    {
        public static IEnumerable<Event> Parse(string json)
        {
            dynamic _json = JsonValue.Parse(json);
            var xs = _json.data as IEnumerable<JsonValue>;
            return (xs).Select(input => new Event(input));
        }
    }

    public class Event
    {
        dynamic _json;
        public Event() { _json = new JsonObject(); }
        public Event(string json):this(JsonValue.Parse(json)){ }
        public Event(JsonValue json) { this._json = json; }

        public bool IsJson { get { return _json.isjson; } set { _json.isjson = value; } }
        public DateTimeOffset TimeStamp 
        { 
            get { return DateTimeOffset.Parse((string)_json.timestamp); }
            set { _json.timestamp = value.ToString(); } 
        }
        public string InputName { get { return _json.inputname; } set { _json.inputname = value; } }
        public int InputId { get { return _json.inputid; } set { _json.inputid = value; }}
        public string Ip { get { return _json.ip; } set { _json.ip = value; } }
        public JsonValue Json { get { return _json.json; } set { _json.json = value; } }
        public string Text { get { return _json.text; } set { _json.text = value; } }

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
        public static HttpInput Parse(string json)
        {
            return new HttpInput(json);
        }

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

    public static class HttpInputs
    {
        public static HttpInput[] Parse(string json)
        {
            var _json = JsonValue.Parse(json);
            if (_json.JsonType != JsonType.Array) _json = new JsonArray(_json);
            var xs = _json as IEnumerable<JsonValue>;
            return (xs).Select(input => new HttpInput(input)).ToArray(); 
        }
    }
}