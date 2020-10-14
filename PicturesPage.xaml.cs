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
	/// PicturesPage.xaml 的交互逻辑
	/// </summary>
	public partial class PicturesPage : Page
	{
		private RelationStruct[] texts = new RelationStruct[3];

		public PicturesPage()
		{
			InitializeComponent();
			SetData();
			//ClearNumber();
		}

        public void SetNumber(string name)
        {
            EE_Number_TextBlock.Text = name;
        }

        //获取编号
        public string GetNumber()
        {
            return EE_Number_TextBlock.Text;
        }

        //设置编号
        private void SetData()
        {
            texts[0].obj = new List<FrameworkElement>();
            texts[1].obj = new List<FrameworkElement>();
            texts[2].obj = new List<FrameworkElement>();
            //设置字符串对应的TextBox控件
            texts[0].name = "EE_Picture_Change_Button";
            texts[0].obj.Add(EE_Number_TextBlock);
            texts[0].obj.Add(EE_Piceture_Image);
            texts[1].name = "Overprint_Picture_Change_Button";
            texts[1].obj.Add(Overprint_Number_TextBlock);
            texts[1].obj.Add(Overprint_Picture_Image);
            texts[2].name = "Preview_Picture_Change_Button";
            texts[2].obj.Add(Preview_Number_TextBlock);
            texts[2].obj.Add(Preview_Picture_Image);
        }

        private void PictureChangeButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string path = FileTools.OpenFile("*.png|*.png|*.jpg|*.jpg|*.jepg|*.jepg", ".png");

            if (path.Length > 4)
            {
                string name = FileTools.GetFileShortName(path);
                foreach (RelationStruct keyValue in texts)
                {
                    if (keyValue.name == button.Name)
                    {
                        BitmapImage imagesouce = new BitmapImage();
                        imagesouce = new BitmapImage(new Uri(path));
                        ((Image)keyValue.obj[1]).Source = imagesouce.Clone();
                        ((TextBox)keyValue.obj[0]).Text = name.Split(new char[] { '.' })[0];
                        break;
                    }
                }
            }
        }

        public void HideControl(bool state)
        {
            if (state) //自动
            {
                EE_Picture_Change_Button.IsEnabled = false;
                Overprint_Picture_Change_Button.IsEnabled = false;
                Preview_Picture_Change_Button.IsEnabled = false;
                EE_Number_TextBlock.IsReadOnly = false;
            }
            else //手动
            {
                EE_Picture_Change_Button.IsEnabled = true;
                Overprint_Picture_Change_Button.IsEnabled = true;
                Preview_Picture_Change_Button.IsEnabled = true;
                EE_Number_TextBlock.IsReadOnly = false;
            }
        }

        public string LoadPicture()
        {
            string number = EE_Number_TextBlock.Text;
            if (number == "")
                return "请输入能效编号";
            BitmapImage[] imagesouce = new BitmapImage[3];
            string EEPath = FileTools.FindFiles(FileTools.pictureDirPath, EE_Number_TextBlock.Text, ".jpg");
            string overprintPath = FileTools.FindFiles(FileTools.labelDirPath, EE_Number_TextBlock.Text, ".png");
            string previewPath = FileTools.FindFiles(FileTools.pictureDirPath, EE_Number_TextBlock.Text, ".png");

            if (EEPath != "")
            {
                EE_Piceture_Image.Source = new BitmapImage(new Uri(EEPath)).Clone();
            }

            if (overprintPath != "")
            {
                Overprint_Picture_Image.Source = new BitmapImage(new Uri(overprintPath)).Clone();
                Overprint_Number_TextBlock.Text = FileTools.GetFileShortName(overprintPath);
            }

            if (previewPath != "")
            {
                Preview_Picture_Image.Source = new BitmapImage(new Uri(previewPath)).Clone();
                Preview_Number_TextBlock.Text = FileTools.GetFileShortName(overprintPath);
            }
            return "";
        }

        public void ClearNumber()
        {
            //MainWindow mainWindow = new MainWindow();
            //if (MainWindow.isWriteLog)
            EE_Number_TextBlock.Text = "";
            MainWindow.isWriteLog = false;
        }

    }
}
