using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class ChatPageParams
    {
        public int id_user_1 { get; set; }
        public int id_user_2 { get; set; }
        
        public string email_user_1 { get; set; }

        public ChatPageParams(int id_user_1, int id_user_2, string email_user_1)
        {
            this.id_user_1 = id_user_1;
            this.id_user_2 = id_user_2;
            this.email_user_1 = email_user_1;
        }
    }
}
