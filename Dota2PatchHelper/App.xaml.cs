using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Dota2PatchHelper
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        //启动
        private void App_Startup(object sender, StartupEventArgs e)
        {
            ExceptionHandle.Init();
        }

        //异常处理
        internal static class ExceptionHandle
        {
            private static bool ExceptionLock = false;
            private static bool Inited = false;

            public static void Init()
            {
                if (Inited) { return; }
                Inited = true;

                Application.Current.DispatcherUnhandledException += ExceptionHandle.DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += ExceptionHandle.UnhandledException;
            }

            private static void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
            {
                e.Handled = true;
                if (ExceptionLock == true)
                {
                    return;
                }
                else
                {
                    ExceptionLock = true;
                }
                MessageBox.Show("错误内容：\r\n" + e.Exception.Message, "意料之外的错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                if (ExceptionLock == true)
                {
                    return;
                }
                else
                {
                    ExceptionLock = true;
                }
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    MessageBox.Show("错误内容：\r\n" + exception.Message, "意料之外的错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
            }
        }

    }
}
