using System;
using System.Threading;
using Microsoft.Owin.Hosting;
using Owin;

namespace DotNetMalaga.AkkaExample.NancyWebClient
{

    class Program
    {
        private static readonly ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            int port = 29005;

            if (args.Length > 0)
            {
                int.TryParse(args[0], out port);
            }

            Console.CancelKeyPress += (sender, eArgs) =>
            {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            using (WebApp.Start<Startup>("http://localhost:" + port))
            {
                Console.WriteLine(string.Format("Running a http server on port {0}", port));
                Console.ReadKey();
            }
        }
    }
}