using System.IO;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Shapes;

namespace demo
{
	/// <summary>
	/// RelationEditWindow.xaml 的交互逻辑
	/// </summary>
	public partial class RelationEditWindow : Window
	{
		private int line = -1;

		public RelationEditWindow()
		{
			InitializeComponent();

			Printer.SetPrinters(Printer_ComboBox);
		}

		public RelationEditWindow(int line)
        {
			InitializeComponent();

			string[] data = File.ReadAllLines(FileTools.relationFilePath)[line].Split(new char[] { ';' });
			this.line = line;
			EE_Number_TextBox.Text = data[0];
			Template_TextBox.Text = data[1];
			Printer.SetPrinters(Printer_ComboBox);
		}
		
		private void SavaRelationRecordClick(object sender, RoutedEventArgs e)
        {
			if (EE_Number_TextBox.Text == "" || Template_TextBox.Text == "")
				return;
			string data = EE_Number_TextBox.Text + ';' + Template_TextBox.Text + ';' + Printer_ComboBox.Text + ';';
			if (line == -1) //添加记录
				FileTools.WriteLineFile(FileTools.relationFilePath, data);
			else if (line >= 0 ) //修改记录
				FileTools.CoverLine(FileTools.relationFilePath, data, line);

			this.DialogResult = true;
			this.Close();
		}
	}
}
