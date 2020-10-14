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
        private static int? filterState { get; set; }
        private static FilterParams filterParams { get; set; }
        private static ConfirmReportParams confirmReportParam { get; set; }
        private static string allReportParam { get; set; }

        private static string tokenAuthorization { get; set; }
        private static User userLogin { get; set; }
        private static KantorPolisi kantorPolisiSelected { get; set; }

        private static LaporanLostFound laporanLostFoundSelected { get; set; }

        private static LaporanKriminalitas laporanKriminalitasSelected { get; set; }
        private static ReportDetailPageParams reportDetailPageParam { get; set; }
        private static CrimeReportParams crimeReportParam { get; set; }
        private static LostFoundReportParams lostFoundReportParam { get; set; }

        public static ChatPageParams chatPageParam { get; set; }
        private readonly static string API_URL = "http://adrian-webservice.ta-istts.com/";
        private readonly static string URL_WEBVIEW = "http://adrian-webview.ta-istts.com/";
        private readonly static string URL_ASSETS = "http://adrian-assets.ta-istts.com/";
        private readonly static string URL_GAMBAR_LAPORAN = "http://adrian-webservice.ta-istts.com/public/uploads/";

        public int? getFilterState()
        {
            return filterState;
        }

        public void setFilterState(int? state)
        {
            filterState = state;
        }

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
