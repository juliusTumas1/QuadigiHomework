using MeasurementSampler.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasurementSampler
{
    public class MeasurementDataSampler
    {
        private readonly double _sampleIntervalMinutes;

        public MeasurementDataSampler(double sampleIntervalMinutes)
        {
            _sampleIntervalMinutes = sampleIntervalMinutes;
        }

        public Dictionary<MeasurementType, List<Measurement>> Sample(
            DateTime startOfSampling, List<Measurement> unsampledMeasurements)
        {
            if (!unsampledMeasurements.Any())
            {
                throw new ArgumentException("No measurements for sampling");
            }

            ;

            var resultData = new Dictionary<MeasurementType, List<Measurement>>();

            foreach (var typeMeasurements in unsampledMeasurements.OrderBy(t => t.MeasurementTime).GroupBy(t => t.Type))
            {
                var sampleDate = startOfSampling;

                var sampledMeasurements = new List<Measurement>();
                var intervalMeasurements = new List<Measurement>();

                foreach (var measurement in typeMeasurements)
                {
                    if (measurement.MeasurementTime > sampleDate)
                    {
                        if (intervalMeasurements.Any())
                        {
                            sampledMeasurements.Add(intervalMeasurements.Last());
                            intervalMeasurements.Clear();
                        }

                        sampleDate = GetDateInterval(measurement.MeasurementTime, sampleDate);
                    }

                    intervalMeasurements.Add(new Measurement(sampleDate, measurement.MeasurementValue,
                        typeMeasurements.Key));
                }

                if (intervalMeasurements.Count > 0)
                {
                    sampledMeasurements.Add(intervalMeasurements.Last());
                }

                resultData.Add(typeMeasurements.Key, sampledMeasurements);
            }

            return resultData;
        }

        private DateTime GetDateInterval(DateTime date, DateTime intervalStartDate)
        {
            var result = intervalStartDate;

            if (date > intervalStartDate)
            {
                result = GetDateInterval(date, AddIntervalValue(intervalStartDate));
            }

            return result;
        }

        private DateTime AddIntervalValue(DateTime startDate)
        {
            return startDate.AddMinutes(_sampleIntervalMinutes);
        }
    }
}