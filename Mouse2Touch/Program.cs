using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mouse2Touch {
    static class Program {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main() {

            //避免重複執行
            string ProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            var p = System.Diagnostics.Process.GetProcessesByName(ProcessName);
            if (p.Length > 1) {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);        
            Application.Run(new Form1());

           
        }
    }
}
