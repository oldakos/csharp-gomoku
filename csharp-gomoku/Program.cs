using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csharp_gomoku {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var gs = new Gamestate();
            var engine = new MyGreatEngine();
            var ctrl = new Controller(gs, engine);            
            Application.Run(new GUI(ctrl, gs));
        }
    }
}
