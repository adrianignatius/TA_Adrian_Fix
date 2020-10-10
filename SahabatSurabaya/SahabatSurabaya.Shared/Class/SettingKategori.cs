using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class SettingKategori
    {
        public int id_kategori { get; set; }

        public string nama_kategori { get; set; }

        public string file_gambar_kategori { get; set; }

        public SettingKategori(int id_kategori, string nama_kategori, string file_gambar_kategori)
        {
            this.id_kategori = id_kategori;
            this.nama_kategori = nama_kategori;
            this.file_gambar_kategori = file_gambar_kategori;
        }
    }
}
