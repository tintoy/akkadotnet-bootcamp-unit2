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
			Receive<AddSeries>(
				message => OnAddSeries(message)
			);
        }

		#region Message handlers

		/// <summary>
		///		Called when the actor receives the <see cref="InitializeChart"/> message.
		/// </summary>
		/// <param name="request">
		///		The initialisation request.
		/// </param>
		void OnInitializeChart(InitializeChart request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			_chart.Series.Clear();

			// Only replace existing series if new ones are supplied.
			if (request.InitialSeries != null)
			{
				_seriesByName.Clear();
				foreach (var indexEntry in request.InitialSeries)
					_seriesByName[indexEntry.Key] = indexEntry.Value;
			}

			// Initialise chart state from our cached data series.
			foreach (var indexEntry in _seriesByName)
			{
				// Force both the chart and the internal index to use the same names
				indexEntry.Value.Name = indexEntry.Key;
				_chart.Series.Add(indexEntry.Value);
			}
		}

		/// <summary>
		///		Called when the actor receives the <see cref="AddSeries"/> message.
		/// </summary>
		/// <param name="request">
		///		The add-data-series request.
		/// </param>
		void OnAddSeries(AddSeries request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			Series existingSeries;
			if (_seriesByName.TryGetValue(request.Series.Name, out existingSeries))
			{
				_chart.Series.Remove(existingSeries);
				_seriesByName.Remove(request.Series.Name);
			}

			_seriesByName.Add(request.Series.Name, request.Series);
			_chart.Series.Add(request.Series);
		}

		#endregion // Message handlers

		#region Messages

		/// <summary>
		///		Tell the charting actor to initialise / reinitialise the chart.
		/// </summary>
		public sealed class InitializeChart
		{
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

				InitialSeries = initialSeries;
			}

			/// <summary>
			///		The initial data-series definitions for the chart.
			/// </summary>
			public IReadOnlyDictionary<string, Series> InitialSeries { get; }
		}

		/// <summary>
		///		Tell the charting actor to add a data series to its chart.
		/// </summary>
		public sealed class AddSeries
		{
			/// <summary>
			///		Create a new <see cref="AddSeries"/> request message. 
			/// </summary>
			/// <param name="series">
			///		The series to add to the chart.
			/// </param>
			public AddSeries(Series series)
			{
				if (series == null)
					throw new ArgumentNullException(nameof(series));

				Series = series;
			}

			/// <summary>
			///		The series to add to the chart.
			/// </summary>
			public Series Series { get; }
		}

		#endregion // Messages
	}
}
