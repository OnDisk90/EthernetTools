using System;
using System.Windows.Forms;

namespace MultiCastSend
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception exp)
            {
                string Trace = exp.StackTrace;
                MessageBox.Show("Error detailes: " + exp.Message + Environment.NewLine + Trace, "Multicast Send Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
    }
}
