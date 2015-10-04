using Akka.Actor;
using Serilog;
using System;
using System.Threading;
using System.Windows.Forms;

namespace ChartApp
{
	/// <summary>
	///		Petabridge bootcamp for Akka.NET, Unit 2.
	/// </summary>
	static class Program
	{
		/// <summary>
		///		The main application entry point.
		/// </summary>
		[STAThread]
		static void Main()
		{
			ConfigureLogging();

			SynchronizationContext.SetSynchronizationContext(
				new SynchronizationContext()
			);

			using (ActorSystem chartActors = ActorSystem.Create("ChartActors"))
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(
					new MainWindow(chartActors)
				);
			}
		}

		/// <summary>
		///		Configure logging for the application.
		/// </summary>
		static void ConfigureLogging()
		{
			Log.Logger =
				new LoggerConfiguration()
					.MinimumLevel.Information()
					.Enrich.FromLogContext()
					.Enrich.WithThreadId()
					.WriteTo.Trace(outputTemplate: "[{Level}] {Message}{NewLine}{Exception}")
					.CreateLogger();
		}
	}
}
