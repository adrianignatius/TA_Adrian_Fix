using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class CrimeReportParams
    {
        public User userLogin { get; set; }
        public List<UploadedImage> listImage { get; set; }

        public string descLaporan { get; set; }

        public string lat { get; set; }

        public string lng { get; set; }

        public string judulLaporan { get; set; }

        public string tglLaporan { get; set; }

        public string waktuLaporan { get; set; }

        public string alamatLaporan { get; set; }

        public string displayKategoriKejadian { get; set; }

        public string valueKategoriKejadian { get; set; }

        public string namaFileGambar { get; set; }

        public CrimeReportParams(User userLogin,string judulLaporan, string lat, string lng, string descLaporan, string tglLaporan, string waktuLaporan, string alamatLaporan, string displayKategoriKejadian, string valueKategoriKejadian, List<UploadedImage>listImage,string namaFileGambar)
        {
            this.userLogin = userLogin;
            this.judulLaporan = judulLaporan;
            this.lat = lat;
            this.lng = lng;
            this.descLaporan = descLaporan;
            this.tglLaporan = tglLaporan;
            this.waktuLaporan = waktuLaporan;
            this.alamatLaporan = alamatLaporan;
            this.displayKategoriKejadian = displayKategoriKejadian;
            this.valueKategoriKejadian = valueKategoriKejadian;
            this.listImage = listImage;
            this.namaFileGambar = namaFileGambar;
        }
    }
}
