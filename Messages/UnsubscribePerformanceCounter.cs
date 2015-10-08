using Akka.Actor;
using System;

namespace ChartApp.Messages
{
	/// <summary>
	///		Unsubscribe from notifications about the value of a performance counter.
	/// </summary>
	public sealed class UnsubscribePerformanceCounter
	{
		/// <summary>
		///		Create a new <see cref="UnsubscribePerformanceCounter"/> request message.
		/// </summary>
		/// <param name="counter">
		///		The type of performance counter to unsubscribe from.
		/// </param>
		/// <param name="subscriber">
		///		The actor to which notifications should no longer be sent.
		/// </param>
		public UnsubscribePerformanceCounter(CounterType counter, IActorRef subscriber)
		{
			if (counter == CounterType.Unknown)
				throw new ArgumentOutOfRangeException(nameof(counter), counter, "Invalid performance counter type.");

			if (subscriber == null)
				throw new ArgumentNullException(nameof(subscriber));

			Counter = counter;
			Subscriber = subscriber;
		}

		/// <summary>
		///		The type of performance counter to unsubscribe from.
		/// </summary>
		public CounterType Counter { get; }

		/// <summary>
		///		The actor to which notifications should no longer be sent.
		/// </summary>
		public IActorRef Subscriber { get; }
	}
}
