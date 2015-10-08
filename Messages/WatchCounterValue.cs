using System;

namespace ChartApp.Messages
{
	/// <summary>
	///		Request subscription to notifications about a performance counter's value.
	/// </summary>
	public sealed class WatchCounterValue
	{
		/// <summary>
		///		Create a new <see cref="WatchCounterValue"/> request message.
		/// </summary>
		/// <param name="counterType">
		///		The type of target counter.
		/// </param>
		public WatchCounterValue(CounterType counterType)
		{
			if (counterType == CounterType.Unknown)
				throw new ArgumentOutOfRangeException(nameof(counterType), counterType, "Invalid performance counter type.");

			CounterType = counterType;
		}

		/// <summary>
		///		The type of target counter.
		/// </summary>
		public CounterType CounterType { get; }
    }
}
