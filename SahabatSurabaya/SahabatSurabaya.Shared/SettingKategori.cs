using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class SettingKategori
    {

        [JsonProperty("nama_kategori")]
        public string nama_kategori { get; set; }

        [JsonProperty("file_gambar_kategori")]
        public string file_gambar_kategori { get; set; }

        public SettingKategori(string nama_kategori, string file_gambar_kategori)
        {
            this.nama_kategori = nama_kategori;
            this.file_gambar_kategori = file_gambar_kategori;
        }
    }
}
