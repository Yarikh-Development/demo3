using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace demo
{
    public struct RelationStruct
    {
        public string name;
        public List<FrameworkElement> obj;
    };
    /// <summary>
    /// User.xaml 的交互逻辑
    /// </summary>
    public partial class User : Window
    {

        RelationStruct[] texts = new RelationStruct[3];

        public User(Window window)
        {
            InitializeComponent();
            //设置父窗口
            this.Owner = window;

            SetTextBox();
            this.Loaded += LoadPath;
        }

        private void LoadPath(object sender, RoutedEventArgs e)
        {
            Relation_File_TextBox.Text = FileTools.relationFilePath;
            Label_Dir_TextBox.Text = FileTools.labelDirPath;
            Picture_Dir_TextBox.Text = FileTools.pictureDirPath;
            Log_Dir_TextBox.Text = FileTools.logDirPath;
            Exception_File_TextBox.Text = FileTools.exceptionFilePath;
            Setting_File_TextBox.Text = FileTools.settingFilePath;
        }

        //设置对应关系
        private void SetTextBox()
        {
            texts[0].obj = new List<FrameworkElement>();
            texts[1].obj = new List<FrameworkElement>();
            texts[2].obj = new List<FrameworkElement>();
            //设置字符串对应的TextBox控件
            texts[0].name = "Label_Dir_Button";
            texts[0].obj.Add(Label_Dir_TextBox);
            texts[1].name = "Picture_Dir_Button";
            texts[1].obj.Add(Picture_Dir_TextBox);
            texts[2].name = "Log_Dir_Button";
            texts[2].obj.Add(Log_Dir_TextBox);
        }

        //选择文件按钮事件
        private void RelationFileButtonClick(object sender, RoutedEventArgs e)
        {
            string path = FileTools.OpenFile("*.txt|*.txt", ".txt");
            if (path.Length > 4)
                Relation_File_TextBox.Text = path;
        }

        //选择目录按钮事件
        private void ChoiceDirButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            string path = FileTools.OpenFolder();
            if (path.Length > 4)
            {
                foreach (RelationStruct keyValue in texts)
                {
                    if (keyValue.name == button.Name)
                    {
                        ((TextBox)keyValue.obj[0]).Text = path;
                        return;
                    }
                }
            }
        }

        //保存设置
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            string[] path = 
            {
                Relation_File_TextBox.Text, Label_Dir_TextBox.Text,
                Picture_Dir_TextBox.Text, Log_Dir_TextBox.Text,
                //Exception_File_TextBox.Text
            };
            FileTools.SaveSettingFile(Setting_File_TextBox.Text, path);
            /*if (((MainWindow)this.Owner).GetLogPage() != null)
            {
                ((MainWindow)this.Owner).GetLogPage().LoadData(FileTools.logDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                ((MainWindow)this.Owner).GetLogPage().SetComboBox();
            }

            if (((MainWindow)this.Owner).GetRelationPage() != null)
                ((MainWindow)this.Owner).GetRelationPage().LoadData();

            this.Close();*/

            if (((Window1)this.Owner).GetLogPage() != null)
            {
                ((Window1)this.Owner).GetLogPage().LoadData(FileTools.logDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                ((Window1)this.Owner).GetLogPage().SetComboBox();
            }

            if (((Window1)this.Owner).GetRelationPage() != null)
                ((Window1)this.Owner).GetRelationPage().LoadData();

            this.Close();

        }
    }
}
