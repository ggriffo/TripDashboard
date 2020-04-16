using System;
using System.Collections.Generic;
using System.Text;

namespace TripDataExtraction
{
    public class Location
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public override string ToString()
        {
            return Address + ";" +
                City.Trim().Replace("ã", "a") + ";" +
                Country;
        }
    }
}