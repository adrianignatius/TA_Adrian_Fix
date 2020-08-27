using System;
using System.Collections.Generic;
using System.Text;
using Uno;

namespace SahabatSurabaya.Shared
{
    class DisplayHeaderChat
    {
        public int id_chat { get; set; }
        public string nama_display { get; set; }

        public string pesan_display { get; set; }

        public DateTime waktu_chat { get; set; }

        public DisplayHeaderChat(int id_chat, string nama_display, string pesan_display)
        {
            this.id_chat = id_chat;
            this.nama_display = nama_display;
            this.pesan_display = pesan_display;
            this.waktu_chat = DateTime.Now;
        }
    }
}
