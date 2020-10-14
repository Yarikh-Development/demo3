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

namespace demo
{
    /// <summary>
    /// TextMenu.xaml 的交互逻辑
    /// </summary>
    public partial class TextMenu : Page
    {

        TextMenuSimple simple = new TextMenuSimple();
        TextPicturesPage simplePage = new TextPicturesPage();
        TextMenuDetail detailPage = new TextMenuDetail();
        public TextMenu()
        {
            InitializeComponent();
        }

        public void SkipPages()
        {   
            SkipPage.Content = simple;

        }

        private void SimplePage_Click(object sender, RoutedEventArgs e)
        {
            
            SkipPage.Content = simplePage;
        }

        private void DetailPage_Click(object sender, RoutedEventArgs e)
        {
            
            SkipPage.Content = detailPage;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SkipPage.Content = simplePage;
        }
    }
}
