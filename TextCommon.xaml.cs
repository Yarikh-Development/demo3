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
	//public struct RelationStruct
	//{
	//	public string name;
	//	public List<FrameworkElement> obj;
	//};

	/// <summary>
	/// TextCommon.xaml 的交互逻辑
	/// </summary>
	public partial class TextCommon : Window
	{
		//private TextAdminInterface textAdminInterface;

		public TextCommon()
		{
			InitializeComponent();
			//设置父窗口
			//this.Owner = window;
		}

		//public TextCommon(TextAdminInterface textAdminInterface)
		//{
		//	this.textAdminInterface = textAdminInterface;
		//}
	}
}
