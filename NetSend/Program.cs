using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace NetSend
{
    static class Program
    {
		private static string[] Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion.Split('.');
        public static readonly string AppName = "NetSend v" + Version[0] + "." + Version[1] + "." + Version[2];
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainContext());
        }
    }
}
