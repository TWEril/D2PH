using ModernWpf.Controls;
using System;
using System.Collections.Generic;
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

namespace Dota2PatchHelper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ModernWpf.ThemeManager.Current.ActualApplicationThemeChanged += DarkBackgroundEvent;
        }

        //视图模型
        private ViewModel.MainWindow vmmw = null;

        // 载入完成
        private void D2PH_MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            vmmw = new ViewModel.MainWindow();
            this.DataContext = vmmw;
            vmmw.SetMainWindowObject(this);
            vmmw.RefSystemInfo();
            vmmw.LoadPackageData();

            if (Environment.CommandLine.Contains("-le"))
            {
                buc.Visibility = Visibility.Collapsed;
                AppTitlebar_Background.Opacity = 0.8;
            }
            else
            {
                var rt = System.Windows.Media.RenderCapability.Tier >> 16;
                if (rt < 2)
                {
                    return;
                }
                buc.BlurContainer = Background_Image;
            }

            vmmw.CheckSystemVer();
        }

        #region 函数

        #region 对话框
        /// <summary>
        /// 显示对话框
        /// </summary>
        public void ShowContentDialog(string Title, string Text)
        {
            var cd = new ModernWpf.Controls.ContentDialog()
            {
                Title = Title,
                Content = Text,
                CloseButtonText = "确定"
            };
            cd.ShowAsync();
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        async public Task<ContentDialogResult> ShowContentDialog(string Title, string Text, string pbt, string cbt)
        {
            var cd = new ContentDialog()
            {
                Title = Title,
                Content = Text,
                PrimaryButtonText = pbt,
                CloseButtonText = cbt
            };
            return await cd.ShowAsync();
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        async public Task<ContentDialogResult> ShowContentDialog(ContentDialog cd)
        {
            return await cd.ShowAsync();
        }
        #endregion

        //载入背景
        private void LoadBackground()
        {
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "bg.jpg";
            var img = new BitmapImage(new Uri(path));
            Background_Image.Source = img;
        }

        #endregion

        #region 事件

        //压暗壁纸
        private void DarkBackgroundEvent(object obj1 = null, object obj2 = null)
        {
            var ct = ModernWpf.ThemeManager.Current.ActualApplicationTheme;
            if (ct == ModernWpf.ApplicationTheme.Dark)
            {
                DarkBackground_Canvas.Visibility = Visibility.Visible;
            }
            else
            {
                DarkBackground_Canvas.Visibility = Visibility.Hidden;
            }
        }
        #endregion
    }
}
