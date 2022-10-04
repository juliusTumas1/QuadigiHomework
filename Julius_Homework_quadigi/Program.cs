namespace MeasurementSampler
{
    class Program
    {
        static void Main(string[] args)
        {
            var measurementsHandler = new MeasurementDataHandler();

            measurementsHandler.ProcessMeasurements();
        }
    }
}