using Limilabs.Mail;
using Limilabs.Mail.MSG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TripDataExtraction
{
    class Program
    {
        private static List<HotelInformation> hotels = new List<HotelInformation>();
        private static List<Trip> trips = new List<Trip>();
            
        static void Main(string[] args)
        {
            ReadAllEmails();
        }

        private static void ReadAllEmails()
        {
            ProcessFile processFile = new ProcessFile();

            //Pattern 1
            foreach (string file in Directory.EnumerateFiles(@"C:\Users\g.griffo\Documents\Projects\Private\TripDashboard\emails\", "*Confirmado*.msg"))
            {
                using MsgConverter converter = new MsgConverter(file);
                if (converter.Type == MsgType.Note)
                {
                    IMail email = converter.CreateMessage();

                    string msgAsText = email.GetBodyAsText();

                    AddTrips(processFile.ProcessPortugueseFilePattern1(msgAsText));
                }
            }

            ////Pattern2
            foreach (string file in Directory.EnumerateFiles(@"C:\Users\g.griffo\Documents\Projects\Private\TripDashboard\emails\", "*Confirmação da reserva *.msg"))
            {
                using MsgConverter converter = new MsgConverter(file);
                if (converter.Type == MsgType.Note)
                {
                    IMail email = converter.CreateMessage();

                    string msgAsText = email.GetBodyAsText();

                    AddTrips(processFile.ProcessPortugueseFilePattern2(msgAsText, file));
                }
            }

            //Booking Confirmation
            foreach (string file in Directory.EnumerateFiles(@"C:\Users\g.griffo\Documents\Projects\Private\TripDashboard\emails\", "*Booking Confirmation*.msg"))
            {
                using MsgConverter converter = new MsgConverter(file);
                if (converter.Type == MsgType.Note)
                {
                    IMail email = converter.CreateMessage();

                    string msgAsText = email.GetBodyAsText();

                    AddTrips(processFile.ProcessEnglishFilePattern1(msgAsText, file));
                }
            }

            trips.Sort(delegate (Trip p1, Trip p2)
            {
                return p1.Departure.CompareTo(p2.Departure);
            });

            //before your loop
            var csv = new StringBuilder();
            foreach (Trip trip in trips)
            {
                csv.AppendLine(trip.ToString());
            }
            //after your loop
            File.WriteAllText(@"C:\Users\g.griffo\Documents\Projects\Private\TripDashboard\Trips.csv", csv.ToString());
        }

        private static void AddTrips(List<Trip> pattern1)
        {
            foreach (Trip trip in pattern1)
            {
                if (!TripExits(trip))
                {
                    trips.Add(trip);
                }
            }
        }

        private static bool TripExits(Trip trip)
        {
            return trips.Exists(x => x.Id == trip.Id && x.LocationFrom.City == trip.LocationFrom.City && trip.LocationTo.City == trip.LocationTo.City && x.Departure == trip.Departure && x.Arrival == trip.Arrival);
        }
    }
}
