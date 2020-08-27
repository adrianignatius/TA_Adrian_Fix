using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya.Shared
{
    class DisplayHeaderChat
    {
        public string nama_display { get; set; }

        public string pesan_display { get; set; }

        public string waktu_chat { get; set; }

        public DisplayHeaderChat(string nama_display, string pesan_display, string waktu_chat)
        {
            this.nama_display = nama_display;
            this.pesan_display = pesan_display;
            this.waktu_chat = waktu_chat;
        }
    }
}
