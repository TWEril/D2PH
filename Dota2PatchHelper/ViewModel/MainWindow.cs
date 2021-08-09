using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TWEril;

namespace Dota2PatchHelper.ViewModel
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    internal class MainWindow : ViewModelBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public MainWindow()
        {
            #region SHA256
            SHA256wl.Add("8855E535C0AE4C710B3CCE9E65D10FDBAA32816FBEE56653CBF225C08684049A");
            SHA256wl.Add("A2296FE845F8694DA1ED5D89C3BC8A41693EC3F77E912F8E87E966E7CD9022EB");
            SHA256wl.Add("80273B4273051DE86BCF652FA901E641C9291C1F8FC1B90908DB2F3D4533D8C5");
            SHA256wl.Add("4DE3AC2EA3D232DBD63E5A00102CE883556E4A17CE3B1F945CCC395DCE389F45");
            SHA256wl.Add("B7551339F532CAA9B2268A72E21F288C938EB5319B0350BEAD2983F27F8E3033");
            SHA256wl.Add("35247804295D889B28AD19B0C72B1C4002201E449F203B4A7F46A285CB65CA0B");
            #endregion

            PackageCollection = new ObservableCollection<datpkg>();
            InitCommand();
        }

        #region 字段
        /// <summary>
        /// 主窗口对象
        /// </summary>
        private Dota2PatchHelper.MainWindow mw = null;

        //强制验证文件
        private bool ForceCheckFile = true;

        //SHA256 LIST
        private List<string> SHA256wl = new List<string>();
        #endregion

        #region 属性
        /// <summary>
        /// 系统信息
        /// </summary>
        public string SystemInfo { get; set; }

        /// <summary>
        /// 包合集
        /// </summary>
        public ObservableCollection<datpkg> PackageCollection { get; set; }
        #endregion

        #region 委托
        public delegate void upfunptr(datpkg pdata);

        #endregion

        #region 命令
        //Dota
        public ICommand OpenDotaCommand { get; private set; }
        public ICommand OpenDotaPathCommand { get; private set; }
        public ICommand DotaHelpCommand { get; private set; }

        //Steam
        public ICommand OpenSteamCommand { get; private set; }
        public ICommand OpenSteamPathCommand { get; private set; }

        //外观
        public ICommand ThemeWindows { get; private set; }
        public ICommand ThemeLight { get; private set; }
        public ICommand ThemeDark { get; private set; }

        //文件验证
        public ICommand CheckFileCommand { get; private set; }

        //帮助
        public ICommand HelpCommand { get; private set; }

        //关于
        public ICommand AboutCommand { get; private set; }
        #endregion

        #region 函数
        /// <summary>
        /// 初始化命令
        /// </summary>
        private void InitCommand()
        {
            OpenDotaCommand = new RelayCommand(() =>
            {
                OpenDota2();
            });
            OpenSteamCommand = new RelayCommand(() =>
            {
                OpenSteam();
            });
            OpenDotaPathCommand = new RelayCommand(() =>
            {
                var path = SteamHelper.GetDota2Path();
                if (path != null && System.IO.Directory.Exists(path + "\\"))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(path + "\\");
                    }
                    catch (Exception exc)
                    {
                        mw.ShowContentDialog("打开目录失败", exc.Message);
                    }
                }
                else
                {
                    mw.ShowContentDialog("找不到目录", "请确认此计算机是否已 安装/正确安装 Dota2");
                }
            });
            OpenSteamPathCommand = new RelayCommand(() =>
            {
                var steampath = SteamHelper.GetSteamPath();
                if (steampath != null && System.IO.Directory.Exists(steampath + "\\"))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(steampath + "\\");
                    }
                    catch (Exception exc)
                    {
                        mw.ShowContentDialog("打开目录失败", exc.Message);
                    }
                }
                else
                {
                    mw.ShowContentDialog("找不到Steam", "请确认此计算机是否已 安装/正确安装 Steam");
                }
            });
            DotaHelpCommand = new RelayCommand(async () =>
            {
                var text = new StringBuilder();
                text.AppendLine("① 打开Steam → 右键Dota2");
                text.AppendLine("② 点击属性 → 再点击本地文件");
                text.AppendLine("③ 最后点击验证文件完整性");
                text.AppendLine("等待Steam验证或下载完成即可");
                var cd = new ModernWpf.Controls.ContentDialog()
                {
                    Title = "如何还原文件更改",
                    Content = text.ToString(),
                    PrimaryButtonText = "打开Steam",
                    CloseButtonText = "确定",
                    DefaultButton = ModernWpf.Controls.ContentDialogButton.Primary
                };
                var cdr = await mw.ShowContentDialog(cd);
                if (cdr == ModernWpf.Controls.ContentDialogResult.Primary)
                {
                    OpenSteam();
                }
            });

            ThemeWindows = new RelayCommand(() =>
            {
                ModernWpf.ThemeManager.Current.ApplicationTheme = null;
            });
            ThemeLight = new RelayCommand(() =>
            {
                ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Light;
            });
            ThemeDark = new RelayCommand(() =>
            {
                ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;
            });

            CheckFileCommand = new RelayCommand(() =>
            {
                SetForceCheckFile();
            });

            HelpCommand = new RelayCommand(() =>
             {
                 var sb = new StringBuilder();
                 sb.AppendLine("点击右侧的【使用】按钮即可完成文件替换。");
                 sb.AppendLine("您可以替换【DatPackage】目录内的压缩包，");
                 sb.AppendLine("但您需要关闭【文件验证】否则您会收到文件警告。");
                 sb.AppendLine("-----------------------------------------");
                 sb.AppendLine("支持其他压缩包替换吗？");
                 sb.AppendLine("只要按照【DatPackage】目录内压缩包的");
                 sb.AppendLine("文件存放方式就行。");
                 mw.ShowContentDialog("提示", sb.ToString());
             });

            AboutCommand = new RelayCommand(async () =>
            {
                await mw.ShowContentDialog(new ContentDialogs.About());
            });
        }

        /// <summary>
        /// 刷新系统信息
        /// </summary>
        public void RefSystemInfo()
        {
            var sb = new StringBuilder();

            var winver = Environment.OSVersion.Version.Major;
            var build = Environment.OSVersion.Version.Build;
            if (winver >= 10)
            {
                if (winver == 10)
                {
                    if (build < 19041)
                    {
                        mw.ShowContentDialog("提示", "程序推荐 Windows 10 2004 以上\r\n当前 Windows 低于此版本，但您仍可运行此程序");
                    }
                }
                sb.Append($"Windows  {winver} {build} ");
            }
            else
            {
                sb.Append("不受支持的 Windows ");
            }

            if (Environment.Is64BitOperatingSystem)
            {
                sb.Append("x64");
            }
            else
            {
                sb.Append("x86");
            }

            SystemInfo = sb.ToString();
        }

        /// <summary>
        /// 检查系统版本
        /// </summary>
        public void CheckSystemVer()
        {
            var winver = Environment.OSVersion.Version.Major;
            var build = Environment.OSVersion.Version.Build;
            if (winver >= 10)
            {
                if (winver == 10)
                {
                    if (build < 19041)
                    {
                        mw.ShowContentDialog("提示", "程序推荐 Windows 10 2004 以上\r\n当前 Windows 低于此版本，但您仍可运行此程序");
                    }
                }
            }
            else
            {
                mw.ShowContentDialog("提示", "推荐您使用Windows 10运行本程序\r\n当前 Windows 低于此版本，但您仍可运行此程序");
            }
        }

        /// <summary>
        /// 设置主窗口对象
        /// </summary>
        public void SetMainWindowObject(Dota2PatchHelper.MainWindow obj)
        {
            mw = obj;
        }

        /// <summary>
        /// 载入包数据
        /// </summary>
        async public void LoadPackageData()
        {
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "DatPackage\\";
            if (System.IO.Directory.Exists(path))
            {
                var fl = FileHelper.GetFiles(path, ".zip");
                foreach (var item in fl)
                {
                    string s256 = null;
                    System.IO.FileInfo fileInfo = null;
                    double _fs = 0;
                    try
                    {
                        fileInfo = new System.IO.FileInfo(item.FullName);
                        _fs = System.Math.Ceiling(fileInfo.Length / 1024.0);
                    }
                    catch
                    {

                    }
                    if (_fs < 1024)
                    {
                        s256 = FileHelper.GetFileSHA256(item.FullName);
                    }
                    fileInfo = null;
                    PackageCollection.Add(new datpkg(Fly)
                    {
                        Name = item.Name,
                        Filepath = item.FullName,
                        SHA256 = s256
                    });
                    await Task.Delay(100);
                }
            }
        }

        /// <summary>
        /// 启动应用
        /// </summary>
        public Tuple<bool, string> LaunchApp(string AppPath, string Arge = null)
        {
            if (System.IO.File.Exists(AppPath))
            {
                try
                {
                    System.Diagnostics.Process.Start(AppPath, Arge);
                }
                catch (Exception exc)
                {
                    return new Tuple<bool, string>(false, exc.Message);
                }
            }
            return new Tuple<bool, string>(false, "找不到指定的应用程序");
        }

        //打开Dota2
        private void OpenDota2(bool ExitApp = false)
        {
            var steampath = SteamHelper.GetSteamPath() + "\\steam.exe";
            if (System.IO.File.Exists(steampath))
            {
                var pl = System.Diagnostics.Process.GetProcesses();
                var plr = pl.Where(x => x.ProcessName == "steam").ToArray();
                if (plr.Length < 1)
                {
                    mw.ShowContentDialog("提示", "Steam 未在后台运行\r\n您需要等待Steam启动完成\r\nDota2将稍后启动");
                }
                LaunchApp(steampath, "steam://rungameid/570");
                mw.WindowState = System.Windows.WindowState.Minimized;
                if (ExitApp)
                {
                    App.Current.Shutdown();
                }
            }
            else
            {
                mw.ShowContentDialog("找不到Steam", "请确认此计算机是否已 安装/正确安装 Steam");
            }
        }

        //打开steam
        async private void OpenSteam()
        {
            var steampath = SteamHelper.GetSteamPath() + "\\steam.exe";
            if (System.IO.File.Exists(steampath))
            {
                LaunchApp(steampath);
            }
            else
            {
                var cd = new ModernWpf.Controls.ContentDialog()
                {
                    Title = "找不到Steam",
                    Content = "请确认此计算机是否已 安装/正确安装 Steam",
                    PrimaryButtonText = "前往下载",
                    CloseButtonText = "确定",
                    DefaultButton = ModernWpf.Controls.ContentDialogButton.Primary
                };
                var cdr = await mw.ShowContentDialog(cd);
                if (cdr == ModernWpf.Controls.ContentDialogResult.Primary)
                {
                    try
                    {
                        System.Diagnostics.Process.Start("https://store.steampowered.com/about/");
                    }
                    catch
                    {
                        //...
                    }
                }
            }
        }

        //文件检查设置
        async private void SetForceCheckFile()
        {
            var sb = new StringBuilder();
            sb.Append("当强制验证打开的时候\r\n在执行文件覆盖前会验证文件来源是否被改变\r\n若已被改变则会取消操作\r\n");
            if (ForceCheckFile)
            {
                sb.Append("当前状态：已打开");
            }
            else
            {
                sb.Append("当前状态：已关闭");
            }
            var cd = new ModernWpf.Controls.ContentDialog()
            {
                Title = "提示",
                Content = sb.ToString(),
                DefaultButton = ModernWpf.Controls.ContentDialogButton.Primary,
                PrimaryButtonText = "保持打开",
                SecondaryButtonText = "关闭验证",
                CloseButtonText = "取消",
            };
            var r = await mw.ShowContentDialog(cd);
            sb.Clear();
            switch (r)
            {
                case ModernWpf.Controls.ContentDialogResult.Primary:
                    ForceCheckFile = true;
                    break;
                case ModernWpf.Controls.ContentDialogResult.Secondary:
                    ForceCheckFile = false;
                    break;

                case ModernWpf.Controls.ContentDialogResult.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        async public void Fly(datpkg dp)
        {
            if (ForceCheckFile)
            {
                var r = SHA256wl.Where(x => x == dp.SHA256).ToArray();
                if (r.Length < 1)
                {
                    var cd = new ContentDialogs.VerifyFail();
                    var cdr = await mw.ShowContentDialog(cd);
                    if (cdr != ModernWpf.Controls.ContentDialogResult.Primary)
                    {
                        return;
                    }
                }
            }

            var cd2 = new ContentDialogs.Extpkg(dp);
            var cdr2 = await mw.ShowContentDialog(cd2);
            if (cdr2 == ModernWpf.Controls.ContentDialogResult.Secondary)
            {
                OpenDota2(true);
            }
        }
        #endregion

        #region 类
        /// <summary>
        /// 包数据
        /// </summary>
        public class datpkg : System.ComponentModel.INotifyPropertyChanged
        {
            /// <summary>
            /// 初始化
            /// </summary>
            public datpkg(upfunptr ufp)
            {
                upfp = ufp;
                UsePkgCommand = new RelayCommand(() =>
                {
                    upfp?.Invoke(this);
                });
            }

            private upfunptr upfp { get; set; }

            #region 属性
            /// <summary>
            /// 名称
            /// </summary>
            public string Name
            {
                get => _Name;
                set
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
            private string _Name { get; set; }

            /// <summary>
            /// 文件完整路径
            /// </summary>
            public string Filepath
            {
                get => _Filepath;
                set
                {
                    _Filepath = value;
                    OnPropertyChanged();
                }
            }
            private string _Filepath { get; set; }

            /// <summary>
            /// SHA256
            /// </summary>
            public string SHA256
            {
                get => _SHA256;
                set
                {
                    _SHA256 = value;
                    OnPropertyChanged();
                }
            }
            private string _SHA256 { get; set; }
            #endregion

            /// <summary>
            /// 使用
            /// </summary>
            public ICommand UsePkgCommand { get; private set; }

            #region 改变通知
            /// <summary>
            /// 绑定改变
            /// </summary>
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// 改变通知
            /// </summary>
            private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string _PropertyName = null)
            {
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(_PropertyName));
            }
            #endregion
        }

        #endregion
    }
}
