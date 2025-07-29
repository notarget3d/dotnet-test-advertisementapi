using System.Text;
using System.Collections.Frozen;

using AdvertisementApi.Core.Interfaces;

namespace AdvertisementApi.Core
{
    public sealed class AdvertisementDataParser : IAdvertisementDataParser
    {
        public bool TryParse(byte[] data, out string message, out List<AdvertisementData> dataList)
        {
            return TryParse(Encoding.UTF8.GetString(data), out message, out dataList);
        }

        public bool TryParse(string data, out string message, out List<AdvertisementData> dataList)
        {
            dataList = new List<AdvertisementData>(16);
            StringBuilder sb = new StringBuilder(128);
            int lineNumber = 1;

            using (StringReader reader = new StringReader(data))
            {
                string? line = reader.ReadLine();

                while (line != null)
                {
                    bool result = TryParseLine(line, out string platform, out string[] locations, out string msg);

                    if (!result)
                    {
                        sb.Append("Error while parsing data at line: ");
                        sb.AppendLine(lineNumber.ToString());
                        sb.AppendLine(msg);

                        line = reader.ReadLine();
                        lineNumber++;
                        continue;
                    }

                    if (!AddData(platform, locations, dataList, out msg))
                    {
                        sb.Append("Error while parsing location data at line: ");
                        sb.AppendLine(lineNumber.ToString());
                        sb.AppendLine(msg);
                    }

                    line = reader.ReadLine();
                    lineNumber++;
                }
            }

            if (dataList.Count == 0)
            {
                sb.AppendLine("File parse failed");
                message = sb.ToString();
                return false;
            }

            message = sb.ToString();
            return true;
        }

        private bool AddData(string platform, string[] locations, List<AdvertisementData> dataList, out string message)
        {
            foreach (var location in locations)
            {
                string[] locationsSeparated = SeparateLocationData(location).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                if (locationsSeparated.Length == 0)
                {
                    message = "Invalid location data";
                    return false;
                }

                dataList.Add(new AdvertisementData(platform, locationsSeparated));
            }

            message = string.Empty;
            return true;
        }

        private string[] SeparateLocationData(string locationData)
        {
            return locationData.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }

        private bool TryParseLine(string line, out string platform, out string[] locations, out string message)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                platform = string.Empty;
                locations = Array.Empty<string>();
                message = "Line is null or white space";
                return false;
            }

            int separator = line.IndexOf(':');

            if (separator == -1)
            {
                platform = string.Empty;
                locations = Array.Empty<string>();
                message = "\":\" is expected";
                return false;
            }

            if (separator == 0)
            {
                platform = string.Empty;
                locations = Array.Empty<string>();
                message = "No platform name specified";
                return false;
            }

            platform = line.Substring(0, separator);

            if (string.IsNullOrWhiteSpace(platform))
            {
                platform = string.Empty;
                locations = Array.Empty<string>();
                message = "Failed to parse platform name";
                return false;
            }

            locations = ParseLocationList(line.Substring(separator + 1));

            if (locations.Length == 0)
            {
                message = $"Failed to parse locations for {platform}";
                return false;
            }

            message = string.Empty;
            return true;
        }

        private string[] ParseLocationList(string locList)
        {
            return locList.Split([','], StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
