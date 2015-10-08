using Akka.Actor;
using ChartApp.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ChartApp.Actors
{
	/// <summary>
	///		Actor that monitors a specific performance counter.
	/// </summary>
	public sealed class PerformanceCounterMonitor
		: ReceiveActor
	{
		/// <summary>
		///		Actors subscribed to notifications about value of the performance counter.
		/// </summary>
		readonly HashSet<IActorRef>			_subscribers = new HashSet<IActorRef>();

		/// <summary>
		///		A <see cref="ICancelable"/> used to cancel publishing of performance counter data.
		/// </summary>
		readonly ICancelable				_cancelPublishing;

		/// <summary>
		///		The name of the data series that tracks the performance counter's value.
		/// </summary>
		readonly string						_seriesName;

		/// <summary>
		///		A factory delegate that creates the <see cref="PerformanceCounter"/> to monitor.
		/// </summary>
		readonly Func<PerformanceCounter>	_performanceCounterFactory;

		/// <summary>
		///		The performance counter being monitored.
		/// </summary>
		PerformanceCounter					_performanceCounter;

		/// <summary>
		///		Create a new <see cref="PerformanceCounterMonitor"/> actor.
		/// </summary>
		/// <param name="seriesName">
		///		The name of the data series that tracks the performance counter's value.
		/// </param>
		/// <param name="performanceCounterFactory">
		///		A factory delegate that creates the <see cref="PerformanceCounter"/> to monitor.
		/// </param>
		public PerformanceCounterMonitor(string seriesName, Func<PerformanceCounter> performanceCounterFactory)
		{
			if (String.IsNullOrWhiteSpace(seriesName))
				throw new ArgumentException("Argument cannot be null, empty, or entirely componsed of whitespace: 'seriesName'.", nameof(seriesName));

			if (performanceCounterFactory == null)
				throw new ArgumentNullException(nameof(performanceCounterFactory));

			_seriesName = seriesName;
			_performanceCounterFactory = performanceCounterFactory;
			_cancelPublishing = new Cancelable(Context.System.Scheduler);

			Receive<GatherMetrics>(_ =>
			{
				Metric metric = new Metric(
					_seriesName,
					_performanceCounter.NextValue()
				);
				foreach (IActorRef subscriber in _subscribers)
					subscriber.Tell(metric);
			});
			Receive<SubscribePerformanceCounter>(request =>
			{
				_subscribers.Add(request.Subscriber);
			});
			Receive<UnsubscribePerformanceCounter>(request =>
			{
				_subscribers.Remove(request.Subscriber);
			});
		}

		#region Lifecycle

		/// <summary>
		///		Called when the actor is starting.
		/// </summary>
		protected override void PreStart()
		{
			_performanceCounter = _performanceCounterFactory();
			if (_performanceCounter == null)
				throw new InvalidOperationException("Performance counter factory returned null.");

			Context.System.Scheduler.ScheduleTellRepeatedly(
				initialDelay: TimeSpan.FromMilliseconds(250),
				interval: TimeSpan.FromMilliseconds(250),
				sender: Self,
				message: GatherMetrics.Instance,
				receiver: Self,
				cancelable: _cancelPublishing
			);
		}

		/// <summary>
		///		Called when the actor is stopping.
		/// </summary>
		protected override void PostStop()
		{
			try
			{
				_cancelPublishing.Cancel(
					throwOnFirstException: false
				);
				_performanceCounter.Dispose();
			}
			catch (ObjectDisposedException)
			{
			}
			finally
			{
				base.PostStop();
			}
		}

		#endregion // Lifecycle
	}
}
