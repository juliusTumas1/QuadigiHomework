using MeasurementSampler.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace MeasurementSampler
{
    public class MeasurementDataHandler
    {
        private readonly string _dataFilePath;
        private readonly DateTime _sampleStartDate;

        private readonly MeasurementDataSampler _dataSampler;

        public MeasurementDataHandler()
        {
            _dataFilePath = ConfigurationManager.AppSettings["DataFilePath"];
            _sampleStartDate = DateTime.Parse(ConfigurationManager.AppSettings["SampleStartDate"]);

            var sampleIntervalMinutes = double.Parse(ConfigurationManager.AppSettings["SampleIntervalMinutes"]);
            _dataSampler = new MeasurementDataSampler(sampleIntervalMinutes);
        }

        public void ProcessMeasurements()
        {
            var unsampledMeasurements = MeasurementFileParser.LoadMeasurementsFromFile(_dataFilePath);

            var sampledData = _dataSampler.Sample(_sampleStartDate, unsampledMeasurements);

            PrintSampledMeasurements(sampledData);
        }


        private static void PrintSampledMeasurements(Dictionary<MeasurementType, List<Measurement>> sampledMeasurements)
        {
            foreach (var typeMeasurements in sampledMeasurements)
            {
                Console.WriteLine($"Type:{typeMeasurements.Key}");
                foreach (var measurement in typeMeasurements.Value)
                {
                    Console.WriteLine(
                        $"Time:{measurement.MeasurementTime}, Value:{measurement.MeasurementValue}, Type:{measurement.Type}");
                }
            }
        }
    }
}