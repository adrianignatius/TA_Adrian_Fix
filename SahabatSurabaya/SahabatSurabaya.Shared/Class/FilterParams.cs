using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya.Shared.Class
{
    class FilterParams
    {
        public string tanggal_awal { get; set; }

        public string tanggal_akhir { get; set; }

        public List<int> list_jenis_laporan { get; set; }

        public List<int> list_id_kategori { get; set; }

        public List<int> list_id_kecamatan { get; set; }

        public FilterParams(string tanggal_awal, string tanggal_akhir, List<int> listJenisLaporan, List<int> list_id_kategori, List<int> listIdKecamatan)
        {
            this.tanggal_awal = tanggal_awal;
            this.tanggal_akhir = tanggal_akhir;
            this.list_jenis_laporan = listJenisLaporan;
            this.list_id_kategori = list_id_kategori;
            this.list_id_kecamatan = listIdKecamatan;
        }

        public string getArrayJenisLaporan()
        {
            string s = "";
            for (int i = 0; i < list_jenis_laporan.Count; i++)
            {
                s += list_jenis_laporan[i].ToString() + ",";
            }
            return s.Remove(s.Length - 1);
        }

        public string getArrayIdKecamatan()
        {
            string s = "";
            for (int i = 0; i < list_id_kecamatan.Count; i++)
            {
                s += list_id_kecamatan[i].ToString() + ",";
            }
            return s.Remove(s.Length - 1);
        }

        public string getArrayIdKategori()
        {
            string s = "";
            for (int i = 0; i < list_id_kategori.Count; i++)
            {
                s += list_id_kategori[i].ToString() + ",";
            }
            return s.Remove(s.Length - 1);
        }
    }
}
