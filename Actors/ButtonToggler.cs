using Akka.Actor;
using ChartApp.Messages;
using System;
using System.Windows.Forms;

namespace ChartApp.Actors
{
	/// <summary>
	///		Actor that toggles the state of a <see cref="Button"/>.
	/// </summary>
	public sealed class ButtonToggler
		: ReceiveActor
	{
		/// <summary>
		///		The actor that controls the <see cref="PerformanceCounterMonitor"/>s for various performance counters.
		/// </summary>
		readonly IActorRef		_performanceCountersController;

		/// <summary>
		///		The button controlled by the <see cref="ButtonToggler"/>.
		/// </summary>
		readonly Button			_button;

		/// <summary>
		///		The type of performance counter represented by the button.
		/// </summary>
		readonly CounterType	_counterType;

		/// <summary>
		///		Is the button currently toggled ("On")?
		/// </summary>
		bool _isToggled;

		/// <summary>
		///		Create a new <see cref="ButtonToggler"/> actor.
		/// </summary>
		/// <param name="performanceCountersController">
		///		The actor that controls the <see cref="PerformanceCounterMonitor"/>s for various performance counters.
		/// </param>
		/// <param name="button">
		///		The button controlled by the <see cref="ButtonToggler"/>.
		/// </param>
		/// <param name="counterType">
		///		The type of performance counter represented by the button.
		/// </param>
		/// <param name="isToggled">
		///		Is the button currently toggled ("On")?
		/// </param>
		public ButtonToggler(IActorRef performanceCountersController, Button button, CounterType counterType, bool isToggled = false)
		{
			if (performanceCountersController == null)
				throw new ArgumentNullException("performanceCountersController");

			if (button == null)
				throw new ArgumentNullException("button");

			if (counterType == CounterType.Unknown)
				throw new ArgumentOutOfRangeException(nameof(counterType), counterType, "Invalid performance counter type.");

			_performanceCountersController = performanceCountersController;
			_button = button;
			_counterType = counterType;
			_isToggled = isToggled;

			Receive<Toggle>(_ =>
			{
				if (Flip())
				{
					_performanceCountersController.Tell(
						new WatchCounterValue(_counterType)
					);
				}
				else
				{
					_performanceCountersController.Tell(
						new UnwatchCounterValue(_counterType)
					);
				}
			});
		}

		/// <summary>
		///		Flip the state of the button.
		/// </summary>
		/// <returns>
		///		<c>true</c>, if the button's toggle state is now On; false, if it is Off.
		/// </returns>
		bool Flip()
		{
			_isToggled = !_isToggled;

			string toggleText = _isToggled ? "ON" : "OFF";
			string seriesName = PerformanceCountersController.SeriesNames[_counterType];

			_button.Text = $"{seriesName} ({toggleText})";

			return _isToggled;
		}

		#region Messages

		/// <summary>
		///		Request a <see cref="ButtonToggler"/> to toggle its button on / off.
		/// </summary>
		public sealed class Toggle
		{
			/// <summary>
			///		The singleton instance of the <see cref="Toggle"/> request message.
			/// </summary>
			public static readonly Toggle Instance = new Toggle();

			/// <summary>
			///		Create a new <see cref="Toggle"/> request message.
			/// </summary>
			Toggle()
			{
			}
		}

		#endregion // Messages
	}
}
