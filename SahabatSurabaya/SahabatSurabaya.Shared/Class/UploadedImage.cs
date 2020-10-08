using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class UploadedImage
    {

        public string file_name { get; set; }
        public byte[] image { get; set; }

        public int count { get; set; }

        public UploadedImage(string file_name,byte[] image, int count)
        {
            this.file_name = file_name;
            this.image = image;
            this.count = count;
        }
    }
}
