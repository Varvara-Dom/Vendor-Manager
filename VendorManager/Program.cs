using System;
using System.Windows.Forms;
using VendorManager.Forms;

namespace VendorManager
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            DatabaseInitializer.InitializeDatabase();

            Application.Run(new MainForm());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show($"Произошла ошибка:\n{e.Exception.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show($"Критическая ошибка:\n{ex?.Message}", "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}