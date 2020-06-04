using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class UploadedImage
    {
        public byte[] image { get; set; }

        public int count { get; set; }

        public UploadedImage(byte[] image, int count)
        {
            this.image = image;
            this.count = count;
        }
    }
}
