using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriyemnayaKomissiya
{
    class AbiturientDGItem
    {
        public int ID { get; }
        public int Num { get; }
        public string FIO { get; }
        public string Lgoti { get; set; }
        public string Stati { get; set; }
        public string Vladelec { get; }
        public string Date { get; }
        public string Status { get; set; }
        public bool Hide { get; set; }

        public AbiturientDGItem(int num,int id, string fio, string vladelec, string date)
        {
            Num = num;
            ID = id;
            FIO = fio;
            Vladelec = vladelec;
            Date = date;
            Hide = false;
        }
    }
}
