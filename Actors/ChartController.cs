using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;

namespace ChartApp.Actors
{
	/// <summary>
	///		The actor responsible for configuring / populating the chart.
	/// </summary>
	public class ChartController
		: ReceiveActor
    {
		/// <summary>
		///		The chart that is managed by the actor.
		/// </summary>
        readonly Chart				_chart;

		/// <summary>
		///		Chart series, keyed by <see cref="Series.Name">series name</see>.
		/// </summary>
		Dictionary<string, Series>	_seriesByName;

		/// <summary>
		///		Create a new <see cref="ChartController"/>.
		/// </summary>
		/// <param name="chart">
		///		The chart to be managed by the actor.
		/// </param>
		public ChartController(Chart chart)
			: this(chart, new Dictionary<string, Series>())
        {
        }

		/// <summary>
		///		Create a new <see cref="ChartController"/>.
		/// </summary>
		/// <param name="chart">
		///		The chart to be managed by the actor.
		/// </param>
		/// <param name="seriesByName">
		///		Chart series, keyed by <see cref="Series.Name">series name</see>.
		/// </param>
		public ChartController(Chart chart, Dictionary<string, Series> seriesByName)
        {
			if (chart == null)
				throw new ArgumentNullException(nameof(chart));

			if (seriesByName == null)
				throw new ArgumentNullException(nameof(seriesByName));

			_chart = chart;
            _seriesByName = seriesByName;

			Receive<InitializeChart>(
				message => OnInitializeChart(message)
			);
        }

		#region Message handlers

		/// <summary>
		///		Called when the actor receives the <see cref="InitializeChart"/> message.
		/// </summary>
		/// <param name="initializeChart">
		///		The initialisation request.
		/// </param>
		void OnInitializeChart(InitializeChart initializeChart)
		{
			if (initializeChart == null)
				throw new ArgumentNullException(nameof(initializeChart));

			if (initializeChart.InitialSeries != null)
				_seriesByName = new Dictionary<string, Series>(initializeChart.InitialSeries.Count); // Only replace existing series if new ones are supplied.

			_chart.Series.Clear();
			if (_seriesByName.Count > 0)
			{
				foreach (var indexEntry in _seriesByName)
				{
					// Force both the chart and the internal index to use the same names
					indexEntry.Value.Name = indexEntry.Key;
					_chart.Series.Add(indexEntry.Value);
				}
			}
		}

		#endregion // Message handlers

		#region Messages

		/// <summary>
		///		Tell the charting actor to initialise / reinitialise the chart.
		/// </summary>
		public class InitializeChart
		{
			/// <summary>
			///		The initial data-series definitions for the chart.
			/// </summary>
			readonly IReadOnlyDictionary<string, Series> _initialSeries;

			/// <summary>
			///		Create a new <see cref="InitializeChart"/> request message.
			/// </summary>
			/// <param name="initialSeries">
			///		The initial data-series definitions for the chart.
			/// </param>
			public InitializeChart(IReadOnlyDictionary<string, Series> initialSeries)
			{
				if (initialSeries == null)
					throw new ArgumentNullException(nameof(initialSeries));

				_initialSeries = initialSeries;
			}

			/// <summary>
			///		The initial data-series definitions for the chart.
			/// </summary>
			public IReadOnlyDictionary<string, Series> InitialSeries => _initialSeries;
		}

		#endregion // Messages
	}
}
