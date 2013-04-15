#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Loggly.Retrieval
{
    public class FilteredInputs
    {
        HttpClient _client;
        public Expression<Func<Input, Bool>> _predicate;

        internal FilteredInputs(HttpClient client, Expression<Func<Input, Bool>> predicate)
        {
            _client = client;
            _predicate = predicate;
        }

        public SelectedInputs Select(Expression<Func<SelectedInputs.Input, SelectedInputs.Input>> selector)
        {
            return new SelectedInputs(_client, _predicate);
        }

        public TaskAwaiter<HttpInput[]> GetAwaiter()
        {
            return this.Select(x => x).GetAwaiter();
        }

        public struct Id
        {
            public static Bool operator ==(Id _, int id)
            {
                return new Bool(id: id);
            }

            public static Bool operator ==(int id, Id _)
            {
                return (_ == id);
            }

            [Obsolete]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static Bool operator !=(Id _, int id)
            {
                throw new NotImplementedException();
            }

            [Obsolete]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static Bool operator !=(int id, Id _)
            {
                throw new NotImplementedException();
            }
        }
        public struct Name
        {
            public static Bool operator ==(Name _, string name)
            {
                return new Bool(name: name);
            }

            public static Bool operator ==(string name, Name _)
            {
                return (_ == name);
            }

            [Obsolete]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static Bool operator !=(Name _, string name)
            {
                throw new NotImplementedException();
            }

            [Obsolete]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static Bool operator !=(string name, Name _)
            {
                throw new NotImplementedException();
            }
        }
        public struct Bool 
        {
            internal string _name;
            internal int _id;

            public Bool(string name = null, int id = 0)
            {
                _name = name;
                _id = id;
            }
        }
        public struct Input
        {
            public Id Id { get { return new Id(); } }
            public Name Name { get { return new Name(); } }
        }
    }
}