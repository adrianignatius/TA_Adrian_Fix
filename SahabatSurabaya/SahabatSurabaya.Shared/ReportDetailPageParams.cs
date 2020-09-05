using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class ReportDetailPageParams
    {
        public User userLogin { get; set; }

        public LaporanLostFound laporanSelected { get; set; }

        public ReportDetailPageParams(User userLogin, LaporanLostFound laporanSelected)
        {
            this.userLogin = userLogin;
            this.laporanSelected = laporanSelected;
        }
    }
}
