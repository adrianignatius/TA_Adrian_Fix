using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya.Shared.Class
{
    class FilterParams
    {

        public int status_cari { get; set; }
        public string tanggal_awal { get; set; }

        public string tanggal_akhir { get; set; }

        public List<int> list_jenis_laporan { get; set; }

        public List<int> list_id_barang { get; set; }

        public List<int> list_id_kecamatan { get; set; }

        public FilterParams(int status_cari,string tanggal_awal, string tanggal_akhir, List<int> listJenisLaporan, List<int> listIdBarang, List<int> listIdKecamatan)
        {
            this.status_cari = status_cari;
            this.tanggal_awal = tanggal_awal;
            this.tanggal_akhir = tanggal_akhir;
            this.list_jenis_laporan = listJenisLaporan;
            this.list_id_barang = listIdBarang;
            this.list_id_kecamatan = listIdKecamatan;
        }
    }
}
