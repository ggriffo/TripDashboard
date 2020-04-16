using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TripDataExtraction
{
    public class ProcessFile
    {
        private List<HotelInformation> hotels = new List<HotelInformation>();
        private List<Trip> trips = null;

        private Trip trip;

        public List<Trip> ProcessPortugueseFilePattern1(string msg)
        {
            trips = new List<Trip>();

            msg = Replaces(msg);
            string[] lines = TrimArray(msg.Split("\r"));

            string id = null;
            Location locationFrom = null;
            Location locationTo = null;
            DateTime? departure = null;
            DateTime? arrival = null;
            Price price = null;

            foreach (string line in lines)
            {
                id ??= GetID(line);
                if (id != null)
                {
                    locationFrom ??= GetLocationPattern1(line, "De ");
                    if (locationFrom != null)
                    {
                        locationTo ??= GetLocationPattern1(line, "Para ");
                        if (locationTo != null)
                        {
                            departure ??= GetDatePattern1(line, "Saída");
                            if (departure.HasValue)
                            {
                                arrival ??= GetDatePattern1(line, "Chegada");
                                if (arrival != null)
                                {
                                    trip = new Trip
                                    {
                                        Id = id,
                                        LocationFrom = locationFrom,
                                        LocationTo = locationTo,
                                        Arrival = arrival.Value,
                                        Departure = departure.Value
                                    };

                                    trips.Add(trip);


                                    locationFrom = null;
                                    locationTo = null;
                                    departure = null;
                                    arrival = null;
                                }
                            }
                        }
                    }

                    price ??= GetPrice(line);
                    if (price != null)
                    {
                        trips.FindAll(d => d.Id == id).ForEach(delegate (Trip s)
                        {
                            s.Price = price;
                        });
                    }
                }
            }

            return trips;
        }

        internal List<Trip> ProcessEnglishFilePattern1(string msg, string fileName)
        {
            trips = new List<Trip>();
            string[] lines = TrimArray(msg.Split("\r"));

            string id = null;
            Location locationFrom = null;
            Location locationTo = null;
            DateTime? departure = null;
            DateTime? arrival = null;
            Price price = null;
            int? miles = null;

            foreach (string line in lines)
            {
                id ??= GetID(line);
                if (id != null)
                {
                    locationFrom ??= GetLocationPattern2(line, "Depart: ");
                    if (locationFrom != null)
                    {
                        departure ??= GetDatePattern2(line, "Depart: ", fileName);
                        locationTo ??= GetLocationPattern2(line, "Arrive: ");
                        if (locationTo != null)
                        {
                            arrival ??= GetDatePattern2(line, "Arrive: ", fileName);
                            if (arrival != null)
                            {
                                miles ??= GetMilesPattern2(line);

                                if (miles != null || line == "AÉREO" || line == "AIR" || line.Contains("_______________________"))
                                {
                                    trip = new Trip
                                    {
                                        Id = id,
                                        LocationFrom = locationFrom,
                                        LocationTo = locationTo,
                                        Arrival = arrival.Value,
                                        Departure = departure.Value,
                                        DistanceMiles = (miles != null ? miles.Value : 0)
                                    };

                                    trips.Add(trip);

                                    locationFrom = null;
                                    locationTo = null;
                                    departure = null;
                                    arrival = null;
                                    miles = null;
                                }
                            }
                        }
                    }

                    price ??= GetPrice(line);
                    if (price != null)
                    {
                        trips.FindAll(d => d.Id == id).ForEach(delegate (Trip s)
                        {
                            s.Price = price;
                        });
                    }
                }
            }

            return trips;
        }

        internal List<Trip> ProcessPortugueseFilePattern2(string msg, string fileName)
        {
            trips = new List<Trip>();
            string[] lines = TrimArray(msg.Split("\r"));

            string id = null;
            Location locationFrom = null;
            Location locationTo = null;
            DateTime? departure = null;
            DateTime? arrival = null;
            Price price = null;
            int? miles = null;

            foreach (string line in lines)
            {
                id ??= GetID(line);
                if (id != null)
                {
                    locationFrom ??= GetLocationPattern2(line, "Saída: ");
                    if (locationFrom != null)
                    {
                        departure ??= GetDatePattern2(line, "Saída: ", fileName);
                        locationTo ??= GetLocationPattern2(line, "Chegada: ");
                        if (locationTo != null)
                        {
                            arrival ??= GetDatePattern2(line, "Chegada: ", fileName);
                            if (arrival != null)
                            {
                                miles ??= GetMilesPattern2(line);

                                if (miles != null || line == "AÉREO" || line == "AIR" || line.Contains("_______________________"))
                                {
                                    trip = new Trip
                                    {
                                        Id = id,
                                        LocationFrom = locationFrom,
                                        LocationTo = locationTo,
                                        Arrival = arrival.Value,
                                        Departure = departure.Value,
                                        DistanceMiles = (miles != null ? miles.Value : 0)
                                    };

                                    trips.Add(trip);

                                    locationFrom = null;
                                    locationTo = null;
                                    departure = null;
                                    arrival = null;
                                    miles = null;
                                }
                            }
                        }
                    }

                    price ??= GetPrice(line);
                    if (price != null)
                    {
                        trips.FindAll(d => d.Id == id).ForEach(delegate (Trip s)
                        {
                            s.Price = price;
                        });
                    }
                }
            }

            return trips;
        }

        private int? GetMilesPattern2(string line)
        {
            if (line.Contains("Milhas: "))
            {
                return Convert.ToInt32(line.Substring(line.Length-4).Trim());
            }

            return null;
        }

        private DateTime? GetDatePattern2(string line, string identifier, string filename)
        {
            if (line.Contains(identifier))
            {
                filename = filename.Substring(filename.LastIndexOf(@"\") + 1);
                int year = Convert.ToInt32(filename.Substring(0, 4));

                string newLine = ReplaceWeekDays(line);
                newLine = newLine.Replace(identifier, "").Trim();

                string dateStr = newLine.Substring(0, 12).Trim();
                string[] dateArr = dateStr.Split(" ");

                if (int.TryParse(dateArr[0], out _) &&
                    int.TryParse(dateArr[2].Split(":")[0], out int hour) &&
                    int.TryParse(dateArr[2].Split(":")[1], out int min))
                {
                    DateTime date = new DateTime(year, GetMonthByName(dateArr[1]), Convert.ToInt32(dateArr[0]), hour, min, 0);
                    return date;
                }
                else if(dateArr.Length > 2 && (int.TryParse(dateArr[1], out _) &&
                    int.TryParse(dateArr[2].Split(":")[0], out int hours) &&
                    int.TryParse(dateArr[2].Split(":")[1], out int mins)))
                {
                    DateTime date = new DateTime(year, GetMonthByName(dateArr[0]), Convert.ToInt32(dateArr[1]), hours, mins, 0);
                    return date;
                }
                else
                {
                    dateStr = newLine.Substring(newLine.Length - 13);
                    dateStr = dateStr.Replace(",", "").Replace("(", "").Replace(")", "").Trim();
                    dateArr = dateStr.Split(" ");

                    if (int.TryParse(dateArr[0], out _) &&
                    int.TryParse(dateArr[2].Split(":")[0], out hour) &&
                    int.TryParse(dateArr[2].Split(":")[1], out min))
                    {
                        DateTime date = new DateTime(year, GetMonthByName(dateArr[1]), Convert.ToInt32(dateArr[0]), hour, min, 0);
                        return date;
                    }
                }
            }

            return null;
        }

        private string ReplaceWeekDays(string line)
        {
            return line.Replace("Segunda-feira, ", "").Replace("Terça-feira, ", "").Replace("Quarta-feira, ", "").Replace("Quinta-feira, ", "").Replace("Sexta-feira, ", "")
                    .Replace("Sábado, ", "").Replace("Domingo, ", "").Replace("Monday, ", "").Replace("Tuesday, ", "").Replace("Wednesday, ", "").Replace("Thursday, ", "")
                    .Replace("Friday, ", "").Replace("Saturday, ", "").Replace("Sunday, ", "");
        }

        private int GetMonthByName(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "jan":
                    return 1;
                case "fev":
                    return 2;
                case "feb":
                    return 2;
                case "mar":
                    return 3;
                case "abr":
                    return 4;
                case "apr":
                    return 4;
                case "mai":
                    return 5;
                case "may":
                    return 5;
                case "jun":
                    return 6;
                case "jul":
                    return 7;
                case "ago":
                    return 8;
                case "aug":
                    return 8;
                case "set":
                    return 9;
                case "sep":
                    return 9;
                case "out":
                    return 10;
                case "oct":
                    return 10;
                case "nov":
                    return 11;
                case "dez":
                    return 12;
                case "dec":
                    return 12;
                default:
                    return 13;
            }
        }

        private Location GetLocationPattern2(string line, string identifier)
        {
            if (line.Contains(identifier))
            {
                Location location = new Location
                {
                    City = line.Replace(identifier, "").Substring(0, line.Replace(identifier, "").IndexOf("("))
                };

                if (location.City.Contains("/"))
                {
                    location.City = location.City.Substring(0, location.City.IndexOf("/")).Trim();
                }

                location.City = ReplaceWeekDays(location.City);

                return location;
            }

            return null;
        }

        private string Replaces(string msgAsText)
        {
            msgAsText = msgAsText.Replace("[http://www.reserve.com.br/empresa/imagens/Servicos/11.gif]", "");
            msgAsText = msgAsText.Replace("[http://www.reserve.com.br/empresa/imagens/pointer.gif]", "");
            msgAsText = msgAsText.Replace("[http://www.reserve.com.br/empresa/logos/accenture/HV00001.gif]", "");
            msgAsText = msgAsText.Replace("[http://www.reserve.com.br/empresa/imagens/airlines/AA.gif]", "");
            msgAsText = msgAsText.Replace("[http://www.reserve.com.br/empresa/imagens/Servicos/12.gif]", "");

            for (int i = 0; i < 50; i++)
            {
                msgAsText = msgAsText.Replace("  ", " ");
            }

            return msgAsText;
        }

        private string[] TrimArray(string[] lines)
        {
            List<string> newArray = new List<string>();

            foreach (string line in lines)
            {
                if (line.Trim().Length > 0)
                {
                    newArray.Add(line.Trim());
                }
            }

            return newArray.ToArray();
        }

        private Price GetPrice(string line)
        {
            Price price = new Price();
            string value = "0";

            if (line.Contains("Tarifa aérea básica (por pessoa)") || line.Contains("Base Airfare (per person):"))
            {
                price.Currency = GetCurrency(line);
                value = line.Replace("Tarifa aérea básica (por pessoa)", "").Replace("BRL", "").Replace(":", "").Replace("Base Airfare (per person)", "").Replace("USD", "").Replace(":", "").Trim();
                price.Value = Convert.ToDouble(value);
            }
            else if (line.Contains("Tarifa"))
            {
                price.Currency = GetCurrency(line);
                if (line.Contains("US$"))
                {
                    value = line.Substring(line.IndexOf("US$") + 3).Trim();
                    value = value.Substring(0, value.IndexOf(" ") - 1).Replace(".", "").Replace(",", ".");
                }
                else if (line.Contains("R$"))
                {
                    value = line.Substring(line.IndexOf("R$") + 3).Trim();
                    value = value.Substring(0, value.IndexOf(" ") - 1).Replace(".", "").Replace(",", ".");
                }

                price.Value = Convert.ToDouble(value);
            }
            else if (line.Contains("Total do voo"))
            {
                price.Currency = GetCurrency(line);
                value = line.Substring(line.IndexOf(") ")).Replace("BRL", "");
                price.Value = Convert.ToDouble(value);
            }

            return (price.Value.HasValue ? price : null);
        }

        private string GetCurrency(string line)
        {
            if (line.Contains("R$") || line.Contains("BRL"))
                return "R$";
            else if (line.Contains("$") || line.Contains("USD"))
                return "$";

            return "null";
        }

        private DateTime? GetDatePattern1(string line, string identifier)
        {
            if (line.Contains(identifier))
            {
                string dateStr = line.Substring(line.IndexOf(" ")).Trim().Replace("h", "");
                IFormatProvider enUsDateFormat = new CultureInfo("pt-BR").DateTimeFormat;

                DateTime date = Convert.ToDateTime(dateStr, enUsDateFormat);
                return date;
            }
            return null;
        }

        private string GetID(string line)
        {
            Match match = Regex.Match(line, "(Pedido Reserve)+");
            if (match.Success)
            {
                return line.Substring(match.Index + match.Length, 7).Trim();
            }

            if (line.Contains("SABRE Nº do localizador:") || line.Contains("SABRE Record Locator #:"))
            {
                string id = line.Substring(line.IndexOf(":")+1, 6).Trim();
                if (id.Trim().Length == 6)
                {
                    return id;
                }
                else
                {
                    id = line.Replace("SABRE Nº do localizador:", "").Replace("SABRE Record Locator #:", "").Trim();
                    if (id.Length == 6)
                        return id;
                    else if (id.Contains("/"))
                    {
                        id = id.Substring(0, id.IndexOf("/"));
                        if (id.Length == 6)
                            return id;
                        else
                            throw new Exception("ID NOT FOUND");
                    }
                    else
                        throw new Exception("ID NOT FOUND");
                }
            }

            return null;
        }

        private Location GetLocationPattern1(string line, string identifier)
        {
            if (line.Contains(identifier))
            {
                Location location = new Location
                {
                    Country = line.Substring(line.IndexOf("[") + 1).Trim(),
                    City = line.Substring(line.IndexOf("]") + 1).Trim()
                };

                location.Country = location.Country.Substring(0, location.Country.IndexOf("]")).Trim();
                if (location.City.IndexOf("(") != -1)
                {
                    location.City = location.City.Substring(0, location.City.IndexOf("(")).Trim();
                }

                string city = location.City;
                if (city.StartsWith("Jan"))

                return location;
            }

            return null;
        }
    }
}
