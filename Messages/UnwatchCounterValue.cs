using System;

namespace ChartApp.Messages
{
	/// <summary>
	///		Request removal of a subscription to notifications about a performance counter's value.
	/// </summary>
	public sealed class UnwatchCounterValue
	{
		/// <summary>
		///		Create a new <see cref="UnwatchCounterValue"/> request message.
		/// </summary>
		/// <param name="counterType">
		///		The type of target counter.
		/// </param>
		public UnwatchCounterValue(CounterType counterType)
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
