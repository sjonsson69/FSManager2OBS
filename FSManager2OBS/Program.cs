using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FSManager2OBS
{
    static class program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.ThreadException += new ThreadExceptionEventHandler(uiThreadException);

                Logger.Info("Startar programmet");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new formFSM2OBS());
            }
            finally
            {
                Logger.Info("Stänger programmet");
                NLog.LogManager.Shutdown();
            }
        }

        private static void uiThreadException(object sender, ThreadExceptionEventArgs t)
        {
            Exception ex = t.Exception;
            //Bygg felmeddelande
            String msg = ex.Message + "\n" + ex.StackTrace;
            if (ex.InnerException != null)
            {
                msg = msg + "\n\n" + ex.InnerException.Message + "\n" + ex.InnerException.StackTrace;
            }
            MessageBox.Show(msg, "Fel!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Logger.Fatal(ex, "Övergripande error fångat");

            Application.Exit();
        }

    }
}
