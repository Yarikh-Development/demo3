using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo
{
	public class Binding
	{
        public Binding()
        {
            ShowList = new ObservableCollection<CShowTag>();
        }

        public ObservableCollection<CShowTag> ShowList
        {
            set;
            get;
        }

        public void LoadData()
        {
            for (int i = 0; i < 10; i++)
            {
                CShowTag tag = new CShowTag()
                {
                    Id = (i + 1).ToString(),
                    Qr = "33333",
                    QrStart = "1111",
                    Res = "0",
                    Span = "33"
                };
                ShowList.Insert(0, tag);
            }

        }
    }


    public class CShowTag
    {


		public string Id
        {
            get => id;
            set => id = value;
        }

        private string id;


        private string qr;

        private string res;

        public string QrStart
        {
            get; set;
        }


        public string Qr
        {
            get => qr;
            set => qr = value;
        }

        public string Res
        {
            get => res;
            set => res = value;
        }

        public string Span
        {
            get => span;
            set => span = value;
        }

        private string span;
    }






}
