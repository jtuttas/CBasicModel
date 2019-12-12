using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CBasicModel
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new Form1());
//                Application.Run(new Alert());
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Exception:" + e);
                Application.Run(new Alert(e.ToString()));
            }
        }
    }
}