using System;

namespace ChartApp.Messages
{
	/// <summary>
	///		Represents the value of a performance counter at the time it was read.
	/// </summary>
	public sealed class Metric
	{
		/// <summary>
		///		Create a new <see cref="Metric"/> message.
		/// </summary>
		/// <param name="seriesName">
		///		The name of the data series that tracks the performance counter.
		/// </param>
		/// <param name="counterValue">
		///		The value of the performance counter at the time it was read.
		/// </param>
		public Metric(string seriesName, float counterValue)
		{
			if (String.IsNullOrWhiteSpace(seriesName))
				throw new ArgumentException("Argument cannot be null, empty, or entirely componsed of whitespace: 'series'.", nameof(seriesName));

			SeriesName = seriesName;
			CounterValue = counterValue;
		}

		/// <summary>
		///		The name of the data series that tracks the performance counter.
		/// </summary>
		public string SeriesName { get; }

		/// <summary>
		///		The value of the performance counter at the time it was read.
		/// </summary>
		public float CounterValue { get; }
	}
}
