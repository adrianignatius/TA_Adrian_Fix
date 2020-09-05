using System;
using System.Collections.Generic;
using System.Text;

namespace SahabatSurabaya
{
    class AutocompleteAddress
    {
        public string description { get; set; }

        public string place_id { get; set; }

        public AutocompleteAddress(string description, string place_id)
        {
            this.description = description;
            this.place_id = place_id;
        }

        public override string ToString()
        {
            return place_id;
        }
    }
}
