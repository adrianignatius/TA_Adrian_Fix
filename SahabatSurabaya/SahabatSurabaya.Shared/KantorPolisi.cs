using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class KantorPolisi
    {
        public int id_kantor_polisi { get; set; }

        public string nama_kantor_polisi { get; set; }

        public string alamat_kantor_polisi { get; set; }

        public string lat_kantor_polisi { get; set; }

        public string lng_kantor_polisi { get; set;}

        public string notelp_kantor_polisi { get; set; }
        
        public double distance { get; set; }

        public KantorPolisi(int id_kantor_polisi, string nama_kantor_polisi, string alamat_kantor_polisi, string lat_kantor_polisi, string lng_kantor_polisi, string notelp_kantor_polisi)
        {
            this.id_kantor_polisi = id_kantor_polisi;
            this.nama_kantor_polisi = nama_kantor_polisi;
            this.alamat_kantor_polisi = alamat_kantor_polisi;
            this.lat_kantor_polisi = lat_kantor_polisi;
            this.lng_kantor_polisi = lng_kantor_polisi;
            this.notelp_kantor_polisi = notelp_kantor_polisi;
            this.distance = 0;
        }
    }
}
