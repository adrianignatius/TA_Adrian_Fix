using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya.Shared
{
    class PendingContact
    {
        public int id_daftar_kontak { get; set; }

        public int id_user { get; set; }

        public string nama_user { get; set; }

        public string telpon_user { get; set; }

        public PendingContact(int id_daftar_kontak, int id_user, string nama_user, string telpon_user)
        {
            this.id_daftar_kontak = id_daftar_kontak;
            this.id_user = id_user;
            this.nama_user = nama_user;
            this.telpon_user = telpon_user;
        }
    }
}
