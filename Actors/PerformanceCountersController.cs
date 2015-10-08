using Akka.Actor;
using ChartApp.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace ChartApp.Actors
{
	/// <summary>
	///		Actor that controls / coordinates the <see cref="PerformanceCounterMonitor"/> actors for various performance counters.
	/// </summary>
	public sealed class PerformanceCountersController
		: ReceiveActor
	{
		#region Static configuration

		/// <summary>
		///		The names of the data series that track well-known performance counter types.
		/// </summary>
		public static readonly IReadOnlyDictionary<CounterType, string> SeriesNames =
			new Dictionary<CounterType, string>
			{
				[CounterType.Processor] = "CPU",
				[CounterType.Memory] = "Memory",
				[CounterType.Disk] = "Disk"
			};

		/// <summary>
		///		Factory delegates for well-known performance counter types.
		/// </summary>
		static readonly IReadOnlyDictionary<CounterType, Func<PerformanceCounter>> PerformanceCounterFactories =
			new Dictionary<CounterType, Func<PerformanceCounter>>
			{
				[CounterType.Processor] = () => new PerformanceCounter("Processor", "% Processor Time", "_Total", true),
				[CounterType.Memory] = () => new PerformanceCounter("Memory", "% Committed Bytes In Use", true),
				[CounterType.Disk] = () => new PerformanceCounter("Processor", "% Disk Time", "_Total", true),
				[CounterType.Unknown] = () => { throw new InvalidOperationException("Unknown performance counter type."); }
			};

		/// <summary>
		///		Factory delegates for chart data series corresponding to well-known performance counter types.
		/// </summary>
		static readonly IReadOnlyDictionary<CounterType, Func<Series>> SeriesFactories =
			new Dictionary<CounterType, Func<Series>>
			{
				[CounterType.Processor] = () => new Series
				{
					Name = SeriesNames[CounterType.Processor],
                    ChartType = SeriesChartType.SplineArea,
					Color = Color.DarkGreen
				},
				[CounterType.Memory] = () => new Series
				{
					Name = SeriesNames[CounterType.Memory],
					ChartType = SeriesChartType.FastLine,
					Color = Color.MediumBlue
				},
				[CounterType.Disk] = () => new Series
				{
					Name = SeriesNames[CounterType.Disk],
					ChartType = SeriesChartType.SplineArea,
					Color = Color.DarkRed
				},
				[CounterType.Unknown] = () => { throw new InvalidOperationException("Unknown performance counter type."); }
			};

		#endregion // Static configuration

		/// <summary>
		///		The actor that controls the chart used to plot the performance counter data.
		/// </summary>
		readonly IActorRef							_chartController;

		/// <summary>
		///		References to the actors used to monitor performance counters (keyed by counter type).
		/// </summary>
		readonly Dictionary<CounterType, IActorRef>	_counterMonitors;

		/// <summary>
		///		Create a new <see cref="PerformanceCountersController"/> actor.
		/// </summary>
		/// <param name="chartController">
		///		The actor that controls the chart used to plot the performance counter data.
		/// </param>
		public PerformanceCountersController(IActorRef chartController)
			: this(chartController, new Dictionary<CounterType, IActorRef>())
		{
		}

		/// <summary>
		///		Create a new <see cref="PerformanceCountersController"/> actor.
		/// </summary>
		/// <param name="chartController">
		///		The actor that controls the chart used to plot the performance counter data.
		/// </param>
		/// <param name="counterMonitors">
		///		The actors used to monitor performance counters (keyed by counter type).
		/// </param>
		public PerformanceCountersController(IActorRef chartController, Dictionary<CounterType, IActorRef> counterMonitors)
		{
			if (chartController == null)
				throw new ArgumentNullException(nameof(chartController));

			if (counterMonitors == null)
				throw new ArgumentNullException(nameof(counterMonitors));

			_chartController = chartController;
			_counterMonitors = counterMonitors;

			Receive<WatchCounterValue>(request =>
			{
				Func<PerformanceCounter> performanceCounterFactory = PerformanceCounterFactories[request.CounterType];
                
				IActorRef counterMonitor;
				if (!_counterMonitors.TryGetValue(request.CounterType, out counterMonitor))
				{
					counterMonitor = Context.ActorOf(Props.Create(() =>
						new PerformanceCounterMonitor(
							SeriesNames[CounterType.Processor],
							performanceCounterFactory
						)
					));
					_counterMonitors.Add(request.CounterType, counterMonitor);
				}

				Series series = SeriesFactories[request.CounterType]();
				_chartController.Tell(
					new ChartController.AddSeries(series)
				);

				counterMonitor.Tell(
					new SubscribePerformanceCounter(request.CounterType, _chartController)
				);
			});

			Receive<UnwatchCounterValue>(request =>
			{
				IActorRef counterMonitor;
				if (!_counterMonitors.TryGetValue(request.CounterType, out counterMonitor))
					return; // Nothing to do.

				counterMonitor.Tell(
					new UnsubscribePerformanceCounter(request.CounterType, _chartController)
				);

				_chartController.Tell(
					new ChartController.RemoveSeries(SeriesNames[request.CounterType])
				);
            });
		}
	}
}
