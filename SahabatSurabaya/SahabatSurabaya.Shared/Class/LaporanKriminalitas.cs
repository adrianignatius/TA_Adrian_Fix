using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class LaporanKriminalitas
    {
        public string id_laporan { get; set; }

        public string judul_laporan { get; set; }

        public string jenis_kejadian { get; set; }

        public string deskripsi_kejadian { get; set; }

        public string tanggal_laporan { get; set; }

        public string waktu_laporan { get; set; }

        public string alamat_laporan { get; set; }

        public string lat_laporan { get; set; }

        public string lng_laporan { get; set; }

        public int id_user_pelapor { get; set; }

        public string nama_user_pelapor { get; set; }

        public int jumlah_komentar { get; set; }

        public string thumbnail_gambar { get; set; }

        public int status_laporan { get; set; }

        public LaporanKriminalitas(string id_laporan, string judul_laporan, string jenis_kejadian, string deskripsi_kejadian, string tanggal_laporan, string waktu_laporan, string alamat_laporan, string lat_laporan, string lng_laporan, int id_user_pelapor, string nama_user_pelapor, int jumlah_komentar, string thumbnail_gambar, int status_laporans)
        {
            this.id_laporan = id_laporan;
            this.judul_laporan = judul_laporan;
            this.jenis_kejadian = jenis_kejadian;
            this.deskripsi_kejadian = deskripsi_kejadian;
            this.tanggal_laporan = tanggal_laporan;
            this.waktu_laporan = waktu_laporan;
            this.alamat_laporan = alamat_laporan;
            this.lat_laporan = lat_laporan;
            this.lng_laporan = lng_laporan;
            this.id_user_pelapor = id_user_pelapor;
            this.nama_user_pelapor = nama_user_pelapor;
            this.jumlah_komentar = jumlah_komentar;
            this.thumbnail_gambar = thumbnail_gambar;
            this.status_laporan = status_laporan;
        }
    }
}
