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
		public TextAdminInterface()
		{
			InitializeComponent();
		}

		//常用指令窗口测试
		private void Textbtn_Click(object sender, RoutedEventArgs e)
		{
			/*RelationEditWindow relationEditWindow = new RelationEditWindow(Relation_ListView.SelectedIndex);
			relationEditWindow.ShowDialog();*/
			TextCommon textCommon = new TextCommon();
			//textCommon.ShowDialog();
			//TextCommon textCommonWindow = new TextCommon;
			/*this.Content = TextCommon;
			textCommonWindow.ShowDialog();*/
			//User userWindow = new User(this);
			//userWindow.ShowDialog();

			//this.Content = textCommon;
			textCommon.WindowStartupLocation = WindowStartupLocation.Manual;
			textCommon.Left = 0;
			textCommon.Top = 0;
			textCommon.ShowDialog();
		}

	}
}
