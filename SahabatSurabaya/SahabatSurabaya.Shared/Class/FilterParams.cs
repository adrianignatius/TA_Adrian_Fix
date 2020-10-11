using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya.Shared.Class
{
    class FilterParams
    {
        public string tanggal_awal { get; set; }

        public string tanggal_akhir { get; set; }

        public List<int> listIdBarang { get; set; }

        public List<int> listIdKecamatan { get; set; }

        public FilterParams(string tanggal_awal, string tanggal_akhir, List<int> listIdBarang, List<int> listIdKecamatan)
        {
            this.tanggal_awal = tanggal_awal;
            this.tanggal_akhir = tanggal_akhir;
            this.listIdBarang = listIdBarang;
            this.listIdKecamatan = listIdKecamatan;
        }
    }
}
