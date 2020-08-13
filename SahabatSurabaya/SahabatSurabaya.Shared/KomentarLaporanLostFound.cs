using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class KomentarLaporanLostFound
    {

        public int id_komentar { get; set; }

        public string id_laporan { get; set; }

        public string isi_komentar { get; set; }

        public string tanggal_komentar { get; set; }

        public string waktu_komentar { get; set; }

        public string email_user { get; set; }

        public KomentarLaporanLostFound(int id_komentar, string id_laporan, string isi_komentar, string tanggal_komentar, string waktu_komentar, string email_user)
        {
            this.id_komentar = id_komentar;
            this.id_laporan = id_laporan;
            this.isi_komentar = isi_komentar;
            this.tanggal_komentar = tanggal_komentar;
            this.waktu_komentar = waktu_komentar;
            this.email_user = email_user;
        }
    }
}
