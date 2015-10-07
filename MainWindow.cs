using Akka.Actor;
using ChartApp.Actors;
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
		///		The actor system used to subscribe to performance counters and publish their data to the chart.
		/// </summary>
		readonly ActorSystem	_chartActors;

		/// <summary>
		///		The number of series displayed on the chart.
		/// </summary>
		/// <remarks>
		///		AtomicCounter seems redundant to me (pretty sure <see cref="Interlocked.Increment(ref int)"/> is sufficient).
		/// </remarks>
		int						_seriesCount;

		/// <summary>
		///		The actor used to manage the chart.
		/// </summary>
		IActorRef				_chartController;

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

			_chartActors = actorSystem;

			InitializeComponent();
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
			_chartController = _chartActors.ActorOf(
				Props.Create(() => new ChartController(sysChart)),
				name:  "charting"
			);
			Series dataSeries = ChartDataHelper.RandomSeries("FakeSeries" + Interlocked.Increment(ref _seriesCount));
			_chartController.Tell(new ChartController.InitializeChart(
				new Dictionary<string, Series>
				{
					[dataSeries.Name] = dataSeries
				}
			));
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
			_chartActors.Shutdown();
		}

		/// <summary>
		///		Called when the "add series" button is clicked.
		/// </summary>
		/// <param name="sender">
		///		The event sender.
		/// </param>
		/// <param name="args">
		///		The event arguments.
		/// </param>
		void btnAddSeries_Click(object sender, EventArgs args)
		{
			AddFakeSeries();
        }

		#endregion // Event handlers
	}
}
