using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class Chat
    {
        public int id_chat { get; set; }

        public int id_user_pengirim { get; set; }

        public int id_user_penerima { get; set; }

        public string isi_chat { get; set; }


        public string waktu_chat { get; set; }

        public Chat(int id_chat, int id_user_pengirim, int id_user_penerima, string isi_chat, string waktu_chat)
        {
            this.id_chat = id_chat;
            this.id_user_pengirim = id_user_pengirim;
            this.id_user_penerima = id_user_penerima;
            this.isi_chat = isi_chat;
            this.waktu_chat = waktu_chat;
        }
    }
}
