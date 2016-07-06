using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Examples;
using DialogStrategy.Knowledge;
using DialogStrategy.Computation.Model;

using DialogStrategy.Learning;

namespace DialogStrategy
{
    class Program
    {

        static void Main(string[] args)
        {
            mathDemo();
            //mathConsole();
        }

        private static void mathDemo()
        {
       /*     mathConsole(new[]{
                "1 2 is 3",
                "4 1 is 3",
                "5 6", //1
                "no",
                "5 6", //11
                "4 6", //10
                "no",
                "4 6", //2
                "7 8" //15
            });*/

            mathConsole(new[]{
                "1 add 2 is 3",
                "5 sub 4",  
                "5 sub 4 is 1",  
                "6 sub 2"
            });

        }

        private static void mathConsole(params string[] initialUtterances)
        {
            var manager = createAlgebraManager();
            var consoleProvider = new Dialog.ConsoleDialogProvider(manager);
            consoleProvider.SimulateInput(initialUtterances);
            consoleProvider.Run();
        }

        private static DialogManagerBase createAlgebraManager()
        {
            var algebraLayer = new AlgebraLayer(20);

            var manager = new MultiTurnDialogManager(algebraLayer);
            return manager;
        }
    }
}
