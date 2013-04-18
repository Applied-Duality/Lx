#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System.Threading.Tasks;

namespace Loggly
{
    public static class Extensions
    {
        public static async void ToBackGround(this Task task)
        {
            try { await task; } catch { }
        }

        public static string Serialize<T>(this T value)
        {
            return ServiceStack.Text.TypeSerializer.SerializeToString<T>(value);
        }

        public static T Deserialize<T>(this string s, T dummy)
        {
            ServiceStack.Text.TypeConfig<T>.EnableAnonymousFieldSetters = true;
            return ServiceStack.Text.TypeSerializer.DeserializeFromString<T>(s);
        }
    }
}
