using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MeasurementSampler.Models;

namespace MeasurementSampler
{
    public class MeasurementFileParser
    {
        public static List<Measurement> LoadMeasurementsFromFile(string path)
        {
            return ParseMeasurements(ReadFileContents(path));
        }

        private static string ReadFileContents(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            using var fileStream = new StreamReader(filePath);

            return fileStream.ReadToEnd();
        }

        private static List<Measurement> ParseMeasurements(string data)
        {
            const string pattern = @"{(?<time>[^\r\n]+),(?<type>[^\r\n]+),(?<value>[^\r\n]+)}";

            return Regex.Matches(data, pattern)
                .Select(m => new Measurement(
                    DateTime.Parse(m.Groups["time"].Value),
                    double.Parse(m.Groups["value"].Value),
                    ParseMeasurementType(m.Groups["type"].Value.Trim()))
                ).ToList();
        }

        private static MeasurementType ParseMeasurementType(string type)
        {
            return (MeasurementType) Enum.Parse(typeof(MeasurementType), type);
        }
    }
}