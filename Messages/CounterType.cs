namespace ChartApp.Messages
{
	/// <summary>
	///		Well-known counter types used in this demo.
	/// </summary>
	public enum CounterType
	{
		/// <summary>
		///		An unknown performance counter.
		/// </summary>
		Unknown		= 0,

		/// <summary>
		///		The CPU utilisation performance counter.
		/// </summary>
		Processor	= 1,

		/// <summary>
		///		The memory utilisation performance counter.
		/// </summary>
		Memory		= 2,

		/// <summary>
		///		The disk throughput performance counter.
		/// </summary>
		Disk = 3
	}
}
