using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya.Shared
{
    class UpdateProfileParams
    {
        public int id_user { get; set; }
        public string nama_user { get; set; }

        public string lokasi_aktif_user { get; set; }

        public string lat_user { get; set; }

        public string lng_user { get; set; }

        public UpdateProfileParams(int id_user, string nama_user, string lokasi_aktif_user, string lat_user, string lng_user)
        {
            this.id_user = id_user;
            this.nama_user = nama_user;
            this.lokasi_aktif_user = lokasi_aktif_user;
            this.lat_user = lat_user;
            this.lng_user = lng_user;
        }
    }
}
