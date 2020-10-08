using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class CrimeReportParams
    {
        public UploadedImage imageLaporan { get; set; }

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

        public CrimeReportParams(string judulLaporan, string lat, string lng, string descLaporan, string tglLaporan, string waktuLaporan, string alamatLaporan, string displayKategoriKejadian, string valueKategoriKejadian, UploadedImage imageLaporan,string namaFileGambar)
        {
            this.judulLaporan = judulLaporan;
            this.lat = lat;
            this.lng = lng;
            this.descLaporan = descLaporan;
            this.tglLaporan = tglLaporan;
            this.waktuLaporan = waktuLaporan;
            this.alamatLaporan = alamatLaporan;
            this.displayKategoriKejadian = displayKategoriKejadian;
            this.valueKategoriKejadian = valueKategoriKejadian;
            this.imageLaporan = imageLaporan;
            this.namaFileGambar = namaFileGambar;
        }
    }
}
