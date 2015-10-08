using Akka.Actor;
using System;

namespace ChartApp.Messages
{
	/// <summary>
	///		Subscribe to notifications about the value of a performance counter.
	/// </summary>
	public sealed class SubscribePerformanceCounter
	{
		/// <summary>
		///		Create a new <see cref="SubscribePerformanceCounter"/> request message.
		/// </summary>
		/// <param name="counter">
		///		The type of performance counter to subscribe to.
		/// </param>
		/// <param name="subscriber">
		///		The actor to which notifications should be sent.
		/// </param>
		public SubscribePerformanceCounter(CounterType counter, IActorRef subscriber)
		{
			if (counter == CounterType.Unknown)
				throw new ArgumentOutOfRangeException(nameof(counter), counter, "Invalid performance counter type.");

			if (subscriber == null)
				throw new ArgumentNullException(nameof(subscriber));

			Counter = counter;
			Subscriber = subscriber;
		}

		/// <summary>
		///		The type of performance counter to subscribe to.
		/// </summary>
		public CounterType Counter { get; }

		/// <summary>
		///		The actor to which notifications should be sent.
		/// </summary>
		public IActorRef Subscriber { get; }
	}
}
