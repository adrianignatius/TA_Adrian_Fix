using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Dynamic;

namespace SahabatSurabaya
{
    class Session
    {
       
        public static User userLogin { get; set; }
        public static ReportDetailPageParams reportDetailPageParam { get; set; }
        public readonly static string API_URL = "http://adrian-webservice.ta-istts.com/";
        public readonly static string URL_WEBVIEW = "http://adrian-webview.ta-istts.com/";

        public void setReportDetailPageParams(ReportDetailPageParams param)
        {
            reportDetailPageParam = param;
        }

        public ReportDetailPageParams getReportDetailPageParams()
        {
            return reportDetailPageParam;
        }


        public void setUserLogin (User user)
        {
            userLogin = user;
        }

        public User getUserLogin()
        {
            return userLogin;
        }

        public string getApiURL()
        {
            return API_URL;
        }
        
        public string getUrlWebView()
        {
            return URL_WEBVIEW;
        }
    }
}
