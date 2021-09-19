using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TurboEmu
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var mutex = new Mutex(false, "TurboEmu"))
            {
                if (!mutex.WaitOne(TimeSpan.Zero))
                {
                    MessageBox.Show("TurboEmu is already running", "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TurboEmu());
                mutex.ReleaseMutex();
            }
        }
    }
}
