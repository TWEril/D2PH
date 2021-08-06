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
    /// VerifyFail.xaml 的交互逻辑
    /// </summary>
    public partial class VerifyFail : ContentDialog
    {
        public VerifyFail()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //Hide();
            this.PrimaryButtonText = "跳过本次";
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.PrimaryButtonText =  null;
        }
    }
}
