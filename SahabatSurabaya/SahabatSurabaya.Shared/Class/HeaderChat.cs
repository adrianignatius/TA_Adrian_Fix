using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya.Shared
{
    class HeaderChat
    {
        public int id_chat { get; set; }
        public int id_user_1 { get; set; }

        public int id_user_2 { get; set; }

        public string nama_user_1 { get; set; }
        

        public string nama_user_2 { get; set; }

        public HeaderChat(int id_chat, int id_user_1, int id_user_2, string nama_user_1, string nama_user_2)
        {
            this.id_chat = id_chat;
            this.id_user_1 = id_user_1;
            this.id_user_2 = id_user_2;
            this.nama_user_1 = nama_user_1;
            this.nama_user_2 = nama_user_2;
        }
    }
}
