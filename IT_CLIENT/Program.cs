using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
namespace IT_CLIENT
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string resource1 = "IT_CLIENT.MyTactics.blogspot.com.dll";
            EmbeddedAssembly.Load(resource1, "MyTactics.blogspot.com.dll");


            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            Application.EnableVisualStyles();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Splash_Screen());
        }
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
