using System;
using System.Collections.Generic;
using System.Text;

namespace TripDataExtraction
{
    public class Trip
    {
        public string Id { get; set; }
        public Location LocationFrom { get; set; }
        public Location LocationTo { get; set; }
        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }
        public int DistanceMiles { get; set; }
        public Price Price { get; set; }

        public override string ToString()
        {
            return Id + ";" +
                LocationFrom?.ToString() + ";" +
                LocationTo?.ToString() + ";" +
                Departure.ToString("dd-MM-yyyy HH:mm") + ";" +
                Arrival.ToString("dd-MM-yyyy HH:mm") + ";" +
                DistanceMiles.ToString() + ";" +
                Price?.ToString();
        }
    }
}
