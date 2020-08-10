using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya.Shared
{
    class LaporanLostFound
    {
        public string id_laporan { get; set; }

        public string judul_laporan { get; set; }

        public int jenis_laporan { get; set; }

        public string tanggal_laporan { get; set; }
        
        public string waktu_laporan { get; set; }
        public string alamat_laporan { get; set; }

        public string lat_laporan { get; set; }

        public string lng_laporan { get; set; }

        public string deskripsi_barang { get; set; }

        public string email_pelapor { get; set; }

        public int status_laporan { get; set; }
    }
}
