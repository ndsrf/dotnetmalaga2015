using Topshelf;

namespace DotNetMalaga.AkkaExample.Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x => //1
            {
                x.Service<AnalysisService>(s => //2
                {
                    s.ConstructUsing(name => new AnalysisService()); //3
                    s.WhenStarted(tc => tc.Start()); //4
                    s.WhenStopped(tc => tc.Stop()); //5
                });
                x.RunAsLocalSystem(); //6

                x.SetDescription("Sample Twitter emotional statistics service analyzer"); //7
                x.SetDisplayName("DotNetMalaga.AkkaExample.Service"); //8
                x.SetServiceName("DotNetMalaga.AkkaExample.Service"); //9
            });
        }
    }
}