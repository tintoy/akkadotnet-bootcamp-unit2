using Akka.Actor;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChartApp
{
	/// <summary>
	///		Extension methods for Akka.NET types.
	/// </summary>
	public static class AkkaExtensions
	{
		/// <summary>
		///		Tell an actor to stop, by sending it a <see cref="PoisonPill"/>.
		/// </summary>
		/// <param name="target">
		///		An <see cref="IActorRef"/> representing the actor to terminate.
		/// </param>
		/// <remarks>
		///		The <see cref="PoisonPill"/> message is an ordinary queue message, and will be picked up by actor once it has finished processing any messages that are ahead of it in the queue.
		///		To receive notification that the actor has been terminated, Watch it before sending the poison pill, and wait for a corresponding <see cref="Terminated"/> message.
		/// </remarks>
		public static void Terminate(this IActorRef target)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));

			target.Tell(PoisonPill.Instance);
		}

		/// <summary>
		///		Tell an actor to stop (by sending it a <see cref="PoisonPill"/>) and subscribe to its life-cycle notifications.
		/// </summary>
		/// <param name="context">
		///		The context of the actor that is telling the actor to stop.
		/// </param>
		/// <param name="target">
		///		An <see cref="IActorRef"/> representing the actor to terminate.
		/// </param>
		/// <remarks>
		///		If the actor that calls <see cref="TerminateWithNotification"/> does not handle the <see cref="Terminated"/> message, it will crash with a <see cref="DeathPactException"/>.
		///		If you don't want to handle this message, use <see cref="Terminate"/> instead of <see cref="TerminateWithNotification"/>.
		/// 
		///		The <see cref="PoisonPill"/> message is an ordinary queue message, and will be picked up by actor once it has finished processing any messages that are ahead of it in the queue.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only intended for use from within an actor.")]
		public static void TerminateWithNotification(this IUntypedActorContext context, IActorRef target)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			if (target == null)
				throw new ArgumentNullException(nameof(target));

			context.Watch(target);

			target.Terminate();
		}
	}
}
