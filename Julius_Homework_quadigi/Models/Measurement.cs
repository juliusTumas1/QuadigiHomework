using System;

namespace MeasurementSampler.Models
{
    public class Measurement
    {
        public DateTime MeasurementTime { get; set; }
        public Double MeasurementValue { get; set; }
        public MeasurementType Type { get; set; }

        public Measurement(DateTime measurementTime, Double measurementValue, MeasurementType type)
        {
            MeasurementTime = measurementTime;
            MeasurementValue = measurementValue;
            Type = type;
        }
    }
}