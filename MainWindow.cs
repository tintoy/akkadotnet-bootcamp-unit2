using Akka.Actor;
using ChartApp.Actors;
using ChartApp.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ChartApp
{
	/// <summary>
	///		The main application window.
	/// </summary>
    public partial class MainWindow
		: Form
    {
		/// <summary>
		///		Actors for handling button-toggling (enable / disable performance counter series).
		/// </summary>
		readonly Dictionary<CounterType, IActorRef>	_buttonTogglers = new Dictionary<CounterType, IActorRef>();

		/// <summary>
		///		The actor system used to subscribe to performance counters and publish their data to the chart.
		/// </summary>
		readonly ActorSystem						_actorSystem;

		/// <summary>
		///		The number of series displayed on the chart.
		/// </summary>
		/// <remarks>
		///		AtomicCounter seems redundant to me (pretty sure <see cref="Interlocked.Increment(ref int)"/> is sufficient).
		/// </remarks>
		int											_seriesCount;

		/// <summary>
		///		The actor used to manage the chart.
		/// </summary>
		IActorRef									_chartController;

		/// <summary>
		///		The actor that manages the <see cref="PerformanceCounterMonitor"/> actors for various performance counters.
		/// </summary>
		IActorRef									_performanceCountersController;

		/// <summary>
		///		Create a new main window.
		/// </summary>
		/// <param name="actorSystem">
		///		The actor system used to subscribe to performance counters and publish their data to the chart.
		/// </param>
		public MainWindow(ActorSystem actorSystem)
        {
			if (actorSystem == null)
				throw new ArgumentNullException(nameof(actorSystem));

			InitializeComponent();

			_actorSystem = actorSystem;
		}

		/// <summary>
		///		Add a fake series to the chart.
		/// </summary>
		void AddFakeSeries()
		{
			int nextSeriesNumber = Interlocked.Increment(ref _seriesCount);
			Series series = ChartDataHelper.RandomSeries($"DummySeries{nextSeriesNumber}");
			_chartController.Tell(
				new ChartController.AddSeries(series)
			);
		}

		/// <summary>
		///		Toggle the state for a performance counter button.
		/// </summary>
		/// <param name="counterType">
		///		The type of performance counter whose corresponding button's state is to be toggled.
		/// </param>
		void ToggleCounterButton(CounterType counterType)
		{
			if (counterType == CounterType.Unknown)
				throw new ArgumentOutOfRangeException(nameof(counterType), counterType, "Invalid performance counter type.");

			_buttonTogglers[counterType].Tell(ButtonToggler.Toggle.Instance);
		}

		#region Event handlers
		
		/// <summary>
		///		Called when the main window is loaded.
		/// </summary>
		/// <param name="sender">
		///		The event sender.
		/// </param>
		/// <param name="args">
		///		The event arguments.
		/// </param>
		void Main_Load(object sender, EventArgs args)
		{
			const string uiThreadDispatcher = "akka.actor.synchronized-dispatcher";
			_chartController = _actorSystem.ActorOf(
				Props.Create(
					() => new ChartController(sysChart)
				)
				.WithDispatcher(uiThreadDispatcher),
				name: "chart-controller"
			);
			_chartController.Tell(
				new ChartController.InitializeChart(null) // No initial data.
			);

			_performanceCountersController = _actorSystem.ActorOf(
				Props.Create(
					() => new PerformanceCountersController(_chartController)
				),
				name: "performance-counters"
			);

			_buttonTogglers[CounterType.Processor] = _actorSystem.ActorOf(
				Props.Create(
					() => new ButtonToggler(_performanceCountersController, btnProcessor, CounterType.Processor, false)
				)
				.WithDispatcher(uiThreadDispatcher)
			);
			_buttonTogglers[CounterType.Memory] = _actorSystem.ActorOf(
				Props.Create(
					() => new ButtonToggler(_performanceCountersController, btnMemory, CounterType.Memory, false)
				)
				.WithDispatcher(uiThreadDispatcher)
			);
			_buttonTogglers[CounterType.Disk] = _actorSystem.ActorOf(
				Props.Create(
					() => new ButtonToggler(_performanceCountersController, btnDisk, CounterType.Disk, false)
				)
				.WithDispatcher(uiThreadDispatcher)
			);

			// CPU usage counter is on by default.
			ToggleCounterButton(CounterType.Processor);
		}

		/// <summary>
		///		Called when the main form is closing.
		/// </summary>
		/// <param name="sender">
		///		The event sender.
		/// </param>
		/// <param name="args">
		///		The event arguments.
		/// </param>
		void MainForm_FormClosing(object sender, FormClosingEventArgs args)
		{
			// Shut down the charting actor
			_chartController.Terminate();

			// Now shut down the entire actor system.
			_actorSystem.Shutdown();
		}

		#endregion // Event handlers

		/// <summary>
		///		Called when the CPU button is clicked.
		/// </summary>
		/// <param name="sender">
		///		The event sender.
		/// </param>
		/// <param name="args">
		///		The event arguments.
		/// </param>
		void btnProcessor_Click(object sender, EventArgs args)
		{
			ToggleCounterButton(CounterType.Processor);
		}

		/// <summary>
		///		Called when the Memory button is clicked.
		/// </summary>
		/// <param name="sender">
		///		The event sender.
		/// </param>
		/// <param name="args">
		///		The event arguments.
		/// </param>
		void btnMemory_Click(object sender, EventArgs args)
		{
			ToggleCounterButton(CounterType.Memory);
		}

		/// <summary>
		///		Called when the Disk button is clicked.,
		/// </summary>
		/// <param name="sender">
		///		The event sender.
		/// </param>
		/// <param name="args">
		///		The event arguments.
		/// </param>
		void btnDisk_Click(object sender, EventArgs args)
		{
			ToggleCounterButton(CounterType.Disk);
		}
	}
}
