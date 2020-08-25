using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class User
    {
        
        [JsonProperty("id_user")]
        public int id_user { get; set; }

        [JsonProperty("email_user")]
        public string email_user { get; set; }

        [JsonProperty("nama_user")]
        public string nama_user { get; set; }

        [JsonProperty("password_user")]
        public string password_user { get; set; }

        [JsonProperty("telpon_user")]
        public string telpon_user { get; set; }

        public int status_user { get; set; }

        public string credit_card_token { get; set; }

        public string premium_available_until { get; set; }

        public User(int id_user, string email_user, string nama_user, string password_user, string telpon_user, int status_user, string credit_card_token, string premium_available_until)
        {
            this.id_user = id_user;
            this.email_user = email_user;
            this.nama_user = nama_user;
            this.password_user = password_user;
            this.telpon_user = telpon_user;
            this.status_user = status_user;
            this.credit_card_token = credit_card_token;
            this.premium_available_until = premium_available_until;
        }
    }

}
