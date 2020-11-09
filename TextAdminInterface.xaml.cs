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
	/// TextAdminInterface.xaml 的交互逻辑
	/// </summary>
	public partial class TextAdminInterface : Page
	{
		private TextCommon textCommon = new TextCommon();

		private demo.Binding _bindingClass = new demo.Binding();

		public TextAdminInterface()
		{
			InitializeComponent();
		}

		private void ZPL_Click(object sender, RoutedEventArgs e)
		{
			this.ZPL.Visibility = Visibility.Visible;
			this.SGD.Visibility = Visibility.Hidden;
		}
		private void SGD_Click(object sender, RoutedEventArgs e)
		{
			this.SGD.Visibility = Visibility.Visible;
			this.ZPL.Visibility = Visibility.Hidden;
		}

		private void Preview_Click(object sender, RoutedEventArgs e)
		{
			 
		}
		private void Copy_Click(object sender, RoutedEventArgs e)
		{
			 
		}
		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			/*//删除选中的某项
			var btn = sender as Button;
			var c = btn.DataContext as demo.CShowTag;
			int index = int.Parse(c.Id);
			this.listView.Items.GetItemAt(index);
			MessageBoxResult boxResult = MessageBox.Show($"确定删除：id={c.Id},Qr={c.Qr} 吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (boxResult == MessageBoxResult.Yes)
			{
				_bindingClass.ShowList.Remove(c);
				this.listView.Items.Refresh();
			}
			return;*/
		}
		
	}
}
