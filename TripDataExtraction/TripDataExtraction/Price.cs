using System;
using System.Collections.Generic;
using System.Text;

namespace TripDataExtraction
{
    public class Price
    {
        public string Currency { get; set; }
        public double? Value { get; set; }

        public override string ToString()
        {
            if (Value != null)
                return Currency + ";" + Value?.ToString();
            else
                return "";
        }
    }
}
