using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dota2PatchHelper.ContentDialogs
{
    /// <summary>
    /// Extpkg.xaml 的交互逻辑
    /// </summary>
    public partial class Extpkg : ContentDialog
    {
        /// <summary>
        /// 初始化
        /// </summary>
        internal Extpkg(ViewModel.MainWindow.datpkg dp)
        {
            data = dp;
            InitializeComponent();
        }

        //载入完成
        async private void Extpkg_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Extpkg_Loaded;
            w_TextBlock.Text = "整理文件中..";
            await Task.Delay(2000);
            Run();
        }

        #region 字段
        private ViewModel.MainWindow.datpkg data = null;

        #endregion

        #region 函数
        //执行
        private void Run()
        {
            RunCore();
            IsPrimaryButtonEnabled = true;
        }

        //执行核心
        async private void RunCore()
        {
            if (File.Exists(data.Filepath))
            {
                //检查路径
                var dota2path = TWEril.SteamHelper.GetDota2Path();
                if (dota2path == null)
                {
                    Fail("错误", "找不到Dota2目录，请确认是否已正确安装");
                    return;
                }

                //准备
                var apppath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                var temppath = apppath + "temp\\";
                string zfn = null;

                #region 读取压缩包
                try
                {
                    using (var za = ZipFile.OpenRead(data.Filepath))
                    {
                        if (za.Entries.Count > 2)
                        {
                            zfn = za.Entries[0].FullName.Replace("/", "\\");
                        }
                    }
                }
                catch
                {
                    Fail("错误...", "解压文件失败");
                    return;
                }
                if (zfn == null)
                {
                    Fail("错误...", "解压文件失败");
                    return;
                }
                #endregion

                #region 检查和创建目录
                if (Directory.Exists(temppath))
                {
                    try
                    {
                        Directory.Delete(temppath, true);
                    }
                    catch
                    {
                        Fail("错误..", "解压文件失败");
                        return;
                    }
                }

                //创建目录
                try
                {
                    Directory.CreateDirectory(temppath);
                }
                catch
                {
                    Fail("错误", "解压文件失败");
                    return;
                }
                #endregion

                //异步操作
                Func<bool> lambdaFunc = () =>
                {
                    //解压
                    try
                    {
                        ZipFile.ExtractToDirectory(data.Filepath, temppath);
                    }
                    catch
                    {
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            Fail("错误.", "解压文件失败");
                        });
                        return false;
                    }

                    //统计
                    var cfs = 0;
                    var ffs = 0;

                    //取得所有文件
                    var ZipRootPath = temppath + zfn;
                    var allfiles = TWEril.FileHelper.GetFolderAllFiles(ZipRootPath);
                    if (allfiles.Count > 0)
                    {
                        //移动覆盖
                        //Console.WriteLine("---------------------------------------------------------");
                        foreach (var item in allfiles)
                        {
                            //修正路径
                            var objpath = item.FullName;
                            var objdir = item.DirectoryName.Replace(ZipRootPath + "dota 2 beta", dota2path);
                            var spath = objpath.Replace(ZipRootPath + "dota 2 beta", dota2path);

                            //开始操作
                            try
                            {
                                //Console.WriteLine(objpath);
                                //Console.WriteLine(spath);
                                //Console.WriteLine("---------------------------------------------------------");

                                if (!Directory.Exists(objdir))
                                {
                                    Directory.CreateDirectory(objdir);
                                }
                                File.Copy(objpath, spath, true);
                            }
                            catch
                            {
                                ffs++;
                                continue;
                            }
                            cfs++;
                        }

                        //完成
                        if (ffs > 0)
                        {
                            this.Dispatcher.InvokeAsync(() =>
                            {
                                Fail("移动文件失败", $"成功{cfs}个，失败{ffs}个\r\n建议您重试此操作");
                            });
                            return false;
                        }
                    }
                    else
                    {
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            Fail("错误", "移动文件失败");
                        });
                        return false;
                    }
                    return true;
                };
                var asyncr = await Task.Run(lambdaFunc);
                if (!asyncr) 
                {
                    ClearTempFolder();
                    return;
                }

                //清理
                if (!ClearTempFolder())
                {
                    if (Directory.Exists(temppath))
                    {
                        Complete("操作已完成", "但是临时文件夹删除失败\r\n您可以在稍后手动删除[temp]文件夹");
                        return;
                    }
                }

                //完成
                Complete("完成", "操作已完成");
            }
            else
            {
                Fail("错误", "找不到指定的压缩包\r\n请检查文件目录是否缺失");
            }
        }

        //错误
        private void Fail(string title, string text)
        {
            w_ProgressBar.Value = 1;
            w_ProgressBar.ShowError = true;
            w_TextBlock.Text = text;
            Title = title;
        }

        //完成
        private void Complete(string title, string text)
        {
            w_ProgressBar.Value = w_ProgressBar.Maximum;
            w_ProgressBar.ShowError = false;
            w_ProgressBar.ShowPaused = false;
            w_ProgressBar.IsIndeterminate = false;
            w_TextBlock.Text = text;
            Title = title;
            exitpar_CheckBox.Visibility = Visibility.Visible;
            ClearTempFolder();
        }

        //清理临时目录
        private bool ClearTempFolder()
        {
            var temppath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "temp\\";
            if (Directory.Exists(temppath))
            {
                //清理
                try
                {
                    Directory.Delete(temppath, true);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 事件
        //勾选
        private void exitpar_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SecondaryButtonText = "确定";
            PrimaryButtonText = null;
        }

        //取消勾选
        private void exitpar_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SecondaryButtonText = null;
            PrimaryButtonText = "确定";
        }

        #endregion
    }
}
