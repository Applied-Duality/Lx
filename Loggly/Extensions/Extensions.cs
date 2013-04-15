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
            try{ await task; } catch {}
        }
    }
}
