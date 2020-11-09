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
	/// Text.xaml 的交互逻辑
	/// </summary>
	public partial class Text : Page
	{
		public Text()
		{
			InitializeComponent();

			///绑定数据
			this.list.ItemsSource = _bindingClass.ShowList;

		}


        private demo.Binding _bindingClass = new demo.Binding();
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            //删除选中的某项
            var btn = sender as Button;
            var c = btn.DataContext as demo.CShowTag;
            int index = int.Parse(c.Id);
            this.list.Items.GetItemAt(index);
            MessageBoxResult boxResult = MessageBox.Show($"确定删除：id={c.Id},Qr={c.Qr} 吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (boxResult == MessageBoxResult.Yes)
            {
                _bindingClass.ShowList.Remove(c);
                this.list.Items.Refresh();
            }
            return;
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            _bindingClass.LoadData();
        }
    



    /*private void BTT_SP_Click(object sender, RoutedEventArgs e)
		{
			//DG_SP.Visibility = Visibility.Visible;//显示页面
			*//*DataTable mm = myFrm_MerchandiseInventoryClient.Frm_CommodityInventoryEnquiry().Tables[0];
			DG_SP.ItemsSource = mm.DefaultView;*//*
		}*/

	}
}
