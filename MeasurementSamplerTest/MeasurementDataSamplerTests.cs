using System;
using System.Collections.Generic;
using FluentAssertions;
using MeasurementSampler;
using MeasurementSampler.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MeasurementSamplerTests
{
    [TestClass]
    public class MeasurementDataSamplerTests
    {
        [TestMethod]
        public void MeasurementTypeIsCreatedSuccessfully()
        {
            var startDate = DateTime.Parse("2000-01-01 01:00:00");
            double interval = 5;
            var measurement1 = new Measurement(DateTime.Parse("2000-01-01T01:00:01"), 1, MeasurementType.TEMP);
            var measurement2 = new Measurement(DateTime.Parse("2000-01-01T01:04:59"), 2, MeasurementType.TEMP);

            var measurementList = new List<Measurement> {measurement1, measurement2};
            var expectedMeasurement = new Measurement(startDate.AddMinutes(interval), 2, MeasurementType.TEMP);
            var expectedMeasurementList = new List<Measurement> {expectedMeasurement};
            var expectedResults = new Dictionary<MeasurementType, List<Measurement>>
            {
                {MeasurementType.TEMP, expectedMeasurementList}
            };

            var results = new MeasurementDataSampler(interval).Sample(startDate, measurementList);

            results.Keys.Should().BeEquivalentTo(expectedResults.Keys);
            results.Values.Should().BeEquivalentTo(expectedResults.Values);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "No measurements for sampling")]
        public void IncorrectMeasurementTypeInConstructor()
        {
            var sampler = new MeasurementDataSampler(1);
            var emptyList = new List<Measurement>();
            var startDate = DateTime.Parse("2000-01-01 01:01:01");

            sampler.Sample(startDate, emptyList);
        }
    }
}