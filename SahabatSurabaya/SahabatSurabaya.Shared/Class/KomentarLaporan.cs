using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class KomentarLaporan
    {
        public int id_komentar { get; set; }

        public string id_laporan { get; set; }

        public string isi_komentar { get; set; }

        public string tanggal_komentar { get; set; }

        public string waktu_komentar { get; set; }

        public string nama_user_komentar { get; set; }

        public KomentarLaporan(int id_komentar, string id_laporan, string isi_komentar, string tanggal_komentar, string waktu_komentar, string nama_user_komentar)
        {
            this.id_komentar = id_komentar;
            this.id_laporan = id_laporan;
            this.isi_komentar = isi_komentar;
            this.tanggal_komentar = tanggal_komentar;
            this.waktu_komentar = waktu_komentar;
            this.nama_user_komentar = nama_user_komentar;
        }
    }
}
