using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Dynamic;
using SahabatSurabaya.Shared;
using SahabatSurabaya.Shared.Class;

namespace SahabatSurabaya
{
    class Session
    {
        public static FilterParams filterParams { get; set; }
        public static ConfirmReportParams confirmReportParam { get; set; }
        public static string allReportParam { get; set; }

        public static string tokenAuthorization { get; set; }
        public static User userLogin { get; set; }
        public static KantorPolisi kantorPolisiSelected { get; set; }

        public static LaporanLostFound laporanLostFoundSelected { get; set; }

        public static LaporanKriminalitas laporanKriminalitasSelected { get; set; }
        public static ReportDetailPageParams reportDetailPageParam { get; set; }
        public static CrimeReportParams crimeReportParam { get; set; }
        public static LostFoundReportParams lostFoundReportParam { get; set; }

        public static ChatPageParams chatPageParam { get; set; }
        public readonly static string API_URL = "http://adrian-webservice.ta-istts.com/";
        public readonly static string URL_WEBVIEW = "http://adrian-webview.ta-istts.com/";
        public readonly static string URL_ASSETS = "http://adrian-assets.ta-istts.com/";
        public readonly static string URL_GAMBAR_LAPORAN = "http://adrian-webservice.ta-istts.com/public/uploads/";

        public FilterParams getFilterParams()
        {
            return filterParams;
        }

        public void setFilterParams(FilterParams param)
        {
            filterParams = param;
        }

        public void setTokenAuthorization(string token)
        {
            tokenAuthorization=token;
        }

        public string getTokenAuthorization()
        {
            return tokenAuthorization;
        }

        public ConfirmReportParams getConfirmReportParams()
        {
            return confirmReportParam;
        }

        public void setConfirmreportParam(ConfirmReportParams param)
        {
            confirmReportParam = param;
        }
        public void setAllReportParam(string param)
        {
            allReportParam = param;
        }

        public string getAllReportParam()
        {
            return allReportParam;
        }

        public LaporanKriminalitas getLaporanKriminalitasSelected()
        {
            return laporanKriminalitasSelected;
        }

        public void setLaporanKriminalitasSelected(LaporanKriminalitas lk)
        {
            laporanKriminalitasSelected = lk;
        }

        public LaporanLostFound getLaporanLostFoundSelected()
        {
            return laporanLostFoundSelected;
        }

        public void setLaporanLostFoundSelected(LaporanLostFound lf)
        {
            laporanLostFoundSelected = lf;
        }

        public KantorPolisi getKantorPolisi()
        {
            return kantorPolisiSelected;
        }
        public void setKantorPolisi(KantorPolisi kantorPolisi)
        {
            kantorPolisiSelected = kantorPolisi;
        }
        public ChatPageParams getChatPageParams()
        {
            return chatPageParam;
        }

        public void setChatPageParams(ChatPageParams param)
        {
            chatPageParam = param;
        }

        public LostFoundReportParams getLostFoundReportParams()
        {
            return lostFoundReportParam;
        }

        public void setLostFoundReportDetailPageParams(LostFoundReportParams param)
        {
            lostFoundReportParam = param;
        }
        public void setCrimeReportDetailPageParams(CrimeReportParams param)
        {
            crimeReportParam = param;
        }

        public CrimeReportParams getCrimeReportParams()
        {
            return crimeReportParam;
        }

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

        public string getUrlGambarLaporan()
        {
            return URL_GAMBAR_LAPORAN;
        }

        public string getUrlWebView()
        {
            return URL_WEBVIEW;
        }

        public string getUrlAssets()
        {
            return URL_ASSETS;
        }
    }
}
