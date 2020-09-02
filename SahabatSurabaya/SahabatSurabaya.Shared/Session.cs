using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace SahabatSurabaya
{
    public class Session
    {
        public readonly static string API_URL = "http://adrian-webservice.ta-istts.com/";
        public static int coba = 1;

        public string getApiURL()
        {
            return API_URL;
        }
    }
}
