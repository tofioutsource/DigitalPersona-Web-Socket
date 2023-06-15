using NLog;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace DPReceiver
{
    static class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false); 
            Application.Run(new MainForm());
        }


    }
}
