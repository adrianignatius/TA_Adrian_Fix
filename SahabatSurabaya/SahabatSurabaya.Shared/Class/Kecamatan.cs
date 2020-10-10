using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya.Shared
{
    class Kecamatan
    {
        public int id_kecamatan { get; set; }

        public string nama_kecamatan { get; set; }

        public List<int> listJenisLaporan { get; set; }

        public List<int> listIdKecamatan { get; set; }

        public Kecamatan(int id_kecamatan, string nama_kecamatan, List<int> listJenisLaporan, List<int> listIdKecamatan)
        {
            this.id_kecamatan = id_kecamatan;
            this.nama_kecamatan = nama_kecamatan;
            this.listJenisLaporan = listJenisLaporan;
            this.listIdKecamatan = listIdKecamatan;
        }
    }
}
