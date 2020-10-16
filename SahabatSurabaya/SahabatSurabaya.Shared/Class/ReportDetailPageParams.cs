using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class ReportDetailPageParams
    {

        public int id_user_pelapor { get; set; }

        public string nama_user_pelapor { get; set; }

        public string id_laporan { get; set; }

        public string alamat_laporan { get; set; }

        public string tanggal_laporan { get; set; }

        public string waktu_laporan { get; set; }

        public string judul_laporan { get; set; }

        public string jenis_laporan { get; set; }

        public string deskripsi_laporan { get; set; }

        public string lat_laporan { get; set; }

        public string lng_laporan { get; set; }
        public string tag { get; set; }

        public string thumbnail_gambar { get; set; }

        public int status_laporan { get; set; }

        public int? jumlah_konfirmasi { get; set; }

        public ReportDetailPageParams(int id_user_pelapor, string nama_user_pelapor, string id_laporan, string alamat_laporan, string tanggal_laporan, string waktu_laporan, string judul_laporan, string jenis_laporan, string deskripsi_laporan, string lat_laporan, string lng_laporan, string tag, string thumbnail_gambar,int status_laporan,int? jumlah_konfirmasi)
        {
            this.id_user_pelapor = id_user_pelapor;
            this.nama_user_pelapor = nama_user_pelapor;
            this.id_laporan = id_laporan;
            this.alamat_laporan = alamat_laporan;
            this.tanggal_laporan = tanggal_laporan;
            this.waktu_laporan = waktu_laporan;
            this.judul_laporan = judul_laporan;
            this.jenis_laporan = jenis_laporan;
            this.deskripsi_laporan = deskripsi_laporan;
            this.lat_laporan = lat_laporan;
            this.lng_laporan = lng_laporan;
            this.tag = tag;
            this.thumbnail_gambar = thumbnail_gambar;
            this.status_laporan = status_laporan;
            this.jumlah_konfirmasi = jumlah_konfirmasi;
        }
    }
}
