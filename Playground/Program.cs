#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using Loggly;
using System;
using System.Configuration;
using System.Json;
using System.Threading.Tasks;
using Retrieval = Loggly.Retrieval;
using Submission = Loggly.Submission;

namespace Playground
{
    partial class Program
    {
        static void Main(string[] args)
        {
            //ProtectConfiguration();
            UnProtectConfiguration();
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            // Submission client does not need to authenticate
            // gets the Url from the HttpInput
            var s = new Submission.Client();

            // Retrieval and management need to log in to subdomain
            var r = new Retrieval.Client
                ( ConfigurationManager.AppSettings["username"]
                , ConfigurationManager.AppSettings["password"]
                , ConfigurationManager.AppSettings["subdomain"]
                );

            // Create new tmp input 
            // Might change/add API to GetOrCreate like IronMQ
            var tmp = await r.CreateHttpInputAsync(string.Format("tmp{0}", DateTime.Now.Ticks));

            Console.WriteLine(tmp.Value.Name);

            // Look up the newly created input
            var inputs = await from i in r.GetInputsAsync()
                         where i.Name == tmp.Value.Name
                         select i;
            
            if (inputs.Length != 0)
            {
                Console.Write("Send a few messages ... ");
                var m = int.Parse(Console.ReadLine());
                for (var n = 0; n < m; n++)
                {
                    // submitting events can take several seconds (> 10)
                    // use .ToBackGround() to fire and forget
                    // or put tasks in a list and call .WhenAll to await all
                    // Also, despite returning 200 OK, some events seem to dissapear
                    // (or may show up much later, I don't know)
                    var b = await s.PostMessageAsync
                            (tmp.Value, 
                            new JsonObject { { "alert", string.Format("Oops I did it again {0}", n) } });

                    Console.WriteLine("{0}-->{1}", n, b);
                }

                // Check if the values were received
                var q = await (from e in r.QueryEventsAsync()
                               where e.InputName == tmp.Value.Name
                                  && e["alert"].Matches("O*") //!e["alert"].Matches("Z*")
                               select new Event { Text = e.Text.ToUpper(), TimeStamp = e.TimeStamp });

                // Show us what you got ...
                Console.WriteLine("found {0} results", q.Length);
                foreach(var result in q) Console.WriteLine(result.ToString());
            }

            // Clean up the queue
            var d = await r.DeleteHttpInputAsync(tmp.Value);

            Console.ReadLine();
        }
    }
}
