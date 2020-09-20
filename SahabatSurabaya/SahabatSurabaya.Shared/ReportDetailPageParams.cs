using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class ReportDetailPageParams
    {

        public LaporanLostFound laporanLostfoundSelected { get; set; }
        public LaporanKriminalitas laporanKriminalitasSelected { get; set; }

        public string tag { get; set; }

        public string getTag()
        {
            return this.tag;
        }

        public void setTag(string tag)
        {
            this.tag = tag;
        }

        public void setLaporanLostFoundSelected(LaporanLostFound lf)
        {
            this.laporanLostfoundSelected = lf;
        }
        
        public void setLaporanKriminalitasSelected(LaporanKriminalitas lk)
        {
            this.laporanKriminalitasSelected = lk;
        }

        public ReportDetailPageParams(LaporanLostFound laporanLostfoundSelected, LaporanKriminalitas laporanKriminalitasSelected, string tag)
        {
            this.laporanLostfoundSelected = laporanLostfoundSelected;
            this.laporanKriminalitasSelected = laporanKriminalitasSelected;
            this.tag = tag;
        }
    }
}
