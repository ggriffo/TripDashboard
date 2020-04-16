using System;
using System.Collections.Generic;
using System.Text;

namespace TripDataExtraction
{
    public class HotelInformation
    {
        public int Id { get; set; }
        public string Hotel { get; set; }
        public Location Location { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public double Price { get; set; }
    }
}
