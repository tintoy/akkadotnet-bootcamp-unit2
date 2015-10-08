namespace ChartApp.Messages
{
	/// <summary>
	///		Tell the performance-counter monitor to gather performance-counter metrics.
	/// </summary>
	public sealed class GatherMetrics
	{
		/// <summary>
		///		Singleton instance of the <see cref="GatherMetrics"/> message.
		/// </summary>
		public static readonly GatherMetrics Instance = new GatherMetrics();

		/// <summary>
		///		Create a new <see cref="GatherMetrics"/> message.
		/// </summary>
		GatherMetrics()
		{
		}
	}
}
