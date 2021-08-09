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

namespace Dota2PatchHelper.ContentDialogs
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : ContentDialog
    {
        public About()
        {
            InitializeComponent();

            Ver_TreeViewItem.Header = App.Ver;
            Build_TreeViewItem.Header = App.Build;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mi = (sender as MenuItem);
            var str = mi.CommandParameter?.ToString();
            var con = info_TreeView.SelectedItem;
            switch (str)
            {
                case "copy":
                    if (con != null)
                    {
                        var tvi = con as TreeViewItem;
                        try
                        {
                            Clipboard.SetText(tvi.Header?.ToString(), TextDataFormat.UnicodeText);
                        }
                        catch
                        {
                            //...
                        }
                    }
                    break;

                case "open":
                    if (con != null)
                    {
                        var tvi = con as TreeViewItem;
                        var url = tvi.Header?.ToString();
                        if (url.Length > 5)
                        {
                            if (url.Substring(0, 5) == "https")
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start(url);
                                }
                                catch
                                {
                                    //...
                                }
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }

    }
}
