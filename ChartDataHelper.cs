using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace ChartApp
{
	/// <summary>
	///		Helper class for creating random data for chart plots
	/// </summary>
	public static class ChartDataHelper
	{
		/// <summary>
		///		Create a pseudo-random <see cref="Series">data series</see>.
		/// </summary>
		/// <param name="seriesName">
		///		The name of the data series to produce.
		/// </param>
		/// <param name="type">
		///		The type of chart used to represent the data series.
		/// </param>
		/// <param name="numberOfPoints">
		///		The number of points that the data series should contain.
		/// </param>
		/// <returns>
		///		The configured <see cref="Series"/>.
		/// </returns>
		public static Series RandomSeries(string seriesName, SeriesChartType type = SeriesChartType.Line, int numberOfPoints = 100)
		{
			if (String.IsNullOrWhiteSpace(seriesName))
				throw new ArgumentException("Argument cannot be null, empty, or entirely componsed of whitespace: 'seriesName'.", nameof(seriesName));

			if (numberOfPoints < 0)
				throw new ArgumentOutOfRangeException(nameof(numberOfPoints), numberOfPoints, "Number of data points cannot be less than 0.");

			Series series = new Series(seriesName)
			{
				ChartType = type,
				BorderWidth = 3
			};

			Random random = new Random();
			for (int dataPointX = 0; dataPointX < numberOfPoints; dataPointX++)
			{
				// Pseudo-random data point.
				double fudgeFactor = random.NextDouble();
				double dataPointY = 2.0 * Math.Sin(fudgeFactor) + Math.Sin(fudgeFactor / 4.5);
                series.Points.Add(
					new DataPoint(dataPointX, dataPointY)
				);
			}

			return series;
		}
	}
}
