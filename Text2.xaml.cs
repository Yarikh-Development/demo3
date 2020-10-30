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
using System.Windows.Shapes;

namespace demo
{
	/// <summary>
	/// Text2.xaml 的交互逻辑
	/// </summary>
	public partial class Text2 : Window
	{
		public Text2()
		{
			InitializeComponent();
		}

		private void s_Click(object sender, RoutedEventArgs e)
		{
			this.menu.PlacementTarget = this.s;
			this.menu.IsOpen = true;
		}
	}
}
