using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya.Shared
{
    class CrimeReportParams
    {
        public List<UploadedImage> listImage { get; set; }

        public string descLaporan { get; set; }

        public string lat { get; set; }

        public string lng { get; set; }

        public string judulLaporan { get; set; }

        public string tglLaporan { get; set; }

        public string waktuLaporan { get; set; }
        public string alamatLaporan { get; set; }

        public string valueKategoriKejadian { get; set; }

    }
}
