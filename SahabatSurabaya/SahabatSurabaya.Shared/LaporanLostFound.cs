using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
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

        public int  id_user_pelapor { get; set; }

        public string nama_user_pelapor { get; set; }

        public int jumlah_komentar { get; set; }
    }
}
