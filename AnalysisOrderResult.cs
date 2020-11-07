using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSockets;

namespace demo
{    

    class AnalysisOrderResult
    {
        private String OrderResult = "";
        private string SN;
        public List<string> DMP;

        List<DMPFile> DMPs;
        WebSocketConsole.MainWindow frmMainmF = null;
        public AnalysisOrderResult(String OrderResult, string SN)
        {
            this.OrderResult = OrderResult;
            this.SN = SN;
            DMPList();
        }

        //检索结果集里有多少个DMP文件
        public void DMPFile(String FileNameBase)
        {
            DMPs = new List<DMPFile>();
            String FileContent = "";
            //string FileNameBase = "E:\\CS\\TestStreamFile\\";
            foreach (string str in DMP)
            {
                //如果结果集有DMP文件，就把文件保存起来
                if (OrderResult.Contains(str))
                {
                    //发送指令到打印机获取到DMP文件文本
                    FileContent = frmMainmF.SendRawDataToPrinter("! U1 do \"file.type\" \"" + str + "\" ", SN);
                    //FileContent =  server.SendRawData(SN, "! U1 do \"file.type\" \"" + str + "\" ", 1500);
                    string labelID = DMPContentEstimate(FileContent);
                    DMPs.Add(new DMPFile { FileName = str, LaBelID = labelID });

                    if (FileContent != "")
                    {
                        //这里需要判断文件是否存在，有替换或取消的选项
                        StreamWriter streamWrite = new StreamWriter(FileNameBase + str);
                        streamWrite.Write(FileContent);//写入数据
                        streamWrite.Close();//关闭文件
                    }                  
                }                   
            }

        }

        //将文件从IN001.DMP到IN999.DMP保存到集合
        private void DMPList()
        {
            DMP = new List<string>();
            
            for (int i = 1; i <= 999; i++)
            {
                DMP.Add("IN" + i.ToString().PadLeft(3, '0') + ".DMP");
            }
            
        }

        //DMP文件内容做判断
        public string DMPContentEstimate(String FileContent)
        {
            String str = FileContent.Split('\r')[0];
            //string str2 = str.Split(new char[3] { '^','F','X'})[0];
            return str.Split('X')[1];
        }
    }

    public class DMPFile
    {
        public string FileName { get; set; }
        public string LaBelID { get; set; }
        public int FileSize{ get; set; }
    }
}
