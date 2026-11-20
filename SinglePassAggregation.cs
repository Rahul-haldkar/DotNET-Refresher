using System;
using System.Collections.Generic;

namespace SinglePassAggregation
{
    public record TelemetryStats(
        int Count,
        double Sum,
        double Average,
        double Max);

    public static class TelemetryProcessor
    {
        public static TelemetryStats ComputeStats(IEnumerable<double> readings)
        {
            int count = 0;
            double sum = 0;
            double max = double.MinValue;

            foreach (double reading in readings)
            {
                count++;
                sum += reading;

                if (reading > max)
                {
                    max = reading;
                }
            }

            double average = count > 0 ? sum / count : 0;
            double finalMax = count > 0 ? max : 0;

            return new TelemetryStats(
                count,
                sum,
                average,
                finalMax);
        }
    }
}
