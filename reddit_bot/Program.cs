using System;
using System.IO;
using System.Windows.Forms;

namespace reddit_bot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new AccountsForm());
            } catch (Exception ex)
            {
                using (StreamWriter streamWriter = new StreamWriter("./data/errors.txt", true)) 
                {
                    streamWriter.WriteLine(ex.Message);
                }
            }
        }
    }
}