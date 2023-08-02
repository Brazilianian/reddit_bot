using reddit_bor.domain.logs;
using reddit_bor.service;
using System;
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
                LogService logService = new LogService();
                logService.WriteLog(new Log(ex.Message, LogLevel.Error));
            }
        }
    }
}