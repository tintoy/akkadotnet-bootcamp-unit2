using Akka.Actor;
using ChartApp.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
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
		///		Maximum number of points in a series.
		/// </summary>
		public const int					MaxPoints = 250;

		/// <summary>
		///		The chart that is managed by the actor.
		/// </summary>
		readonly Chart						_chart;

		/// <summary>
		///		Chart series, keyed by <see cref="Series.Name">series name</see>.
		/// </summary>
		readonly Dictionary<string, Series>	_seriesByName;

		/// <summary>
		///		The current position on the X-axis of the chart.
		/// </summary>
		int									_chartX = 0;

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
			Receive<RemoveSeries>(
				message => OnRemoveSeries(message)
			);
			Receive<Metric>(
				notification => OnMetric(notification)
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

			ChartArea area = _chart.ChartAreas[0];
			area.AxisX.IntervalType = DateTimeIntervalType.Number;
			area.AxisY.IntervalType = DateTimeIntervalType.Number;
			SetChartBoundaries();

			// Initialise chart state from our cached data series.
			foreach (var indexEntry in _seriesByName)
			{
				// Force both the chart and the internal index to use the same names
				indexEntry.Value.Name = indexEntry.Key;
				_chart.Series.Add(indexEntry.Value);
			}
			SetChartBoundaries();
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
			SetChartBoundaries();
		}

		/// <summary>
		///		Called when the actor receives the <see cref="RemoveSeries"/> message.
		/// </summary>
		/// <param name="request">
		///		The remove-data-series request.
		/// </param>
		void OnRemoveSeries(RemoveSeries request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			Series existingSeries;
			if (!_seriesByName.TryGetValue(request.SeriesName, out existingSeries))
				return; // Nothing to do.

			_chart.Series.Remove(existingSeries);
			_seriesByName.Remove(request.SeriesName);
			SetChartBoundaries(); 
		}

		/// <summary>
		///		Called when the chart controller is notified of an updated performance-counter value.
		/// </summary>
		/// <param name="notification">
		///		The performance-counter value notification.
		/// </param>
		void OnMetric(Metric notification)
		{
			if (notification == null)
				throw new ArgumentNullException(nameof(notification));

			Series series;
			if (String.IsNullOrWhiteSpace(notification.SeriesName) || !_seriesByName.TryGetValue(notification.SeriesName, out series))
				return; // Invalid or outdated notification.

			// Add our new value to the chart.
			series.Points.AddXY(_chartX++, notification.CounterValue);

			// Slide the back of the window forwards, if required.
			while (series.Points.Count > MaxPoints)
				series.Points.RemoveAt(0);

			SetChartBoundaries();
        }

		#endregion // Message handlers

		#region Helpers

		/// <summary>
		///		Update the chart boundaries to reflect the sliding window of data from performance counters.
		/// </summary>
		void SetChartBoundaries()
		{
			double maxAxisX = _chartX;
			double minAxisX = _chartX - MaxPoints;

			DataPoint[] allPoints = _seriesByName.Values.SelectMany(series => series.Points).ToArray();
			double[] yValues = allPoints.SelectMany(point => point.YValues).ToArray();
			double maxYValue = yValues.DefaultIfEmpty().Max();
			double maxAxisY = yValues.Length > 0 ? Math.Ceiling(maxYValue) : 1.0d;
			double minAxisY = yValues.Length > 0 ? Math.Floor(maxYValue) : 0.0d;
			if (allPoints.Length > 2)
			{
				ChartArea area = _chart.ChartAreas[0];
				area.AxisX.Minimum = minAxisX;
				area.AxisX.Maximum = maxAxisX;
				area.AxisY.Minimum = minAxisY;
				area.AxisY.Maximum = maxAxisY;
			}
		}

		#endregion Helpers

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

		/// <summary>
		///		Tell the charting actor to remove a data series to its chart.
		/// </summary>
		public sealed class RemoveSeries
		{
			/// <summary>
			///		Create a new <see cref="RemoveSeries"/> request message. 
			/// </summary>
			/// <param name="seriesName">
			///		The name of the series to remove from the chart.
			/// </param>
			public RemoveSeries(string seriesName)
			{
				if (seriesName == null)
					throw new ArgumentNullException(nameof(seriesName));

				SeriesName = seriesName;
			}

			/// <summary>
			///		The name of the series to remove from the chart.
			/// </summary>
			public string SeriesName { get; }
		}

		#endregion // Messages
	}
}
