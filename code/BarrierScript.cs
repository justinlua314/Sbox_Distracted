using System;

public sealed class BarrierScript : Component, Component.ITriggerListener {
	/// <summary>
	/// The amount that the Players y Position is changed when a barrier is hit
	/// </summary>
	[Property] float Pushback { get; set; } = 50f;


	/// <summary>
	/// The amount that the Players yaw Angle is changed when a barrier is hit
	/// </summary>
	[Property] float YawChange { get; set; } = 50f;


	/// <summary>
	/// Player Movement referenced to dampen speed when barrier is hit
	/// </summary>
	[Property] PlayerMovement PlyMove { get; set; }


	/// <summary>
	/// Player Stats are referenced to decrement health when barrier is hit
	/// </summary>
	[Property] PlayerStats Stats { get; set; }


	/// <summary>
	/// How much Speed is dampened when a Barrier is hit
	/// </summary>
	[Property] float SpeedTax { get; set; } = 50f;

	/// <summary>
	/// The sound that plays when the Player hits a barrier
	/// </summary>
	[Property] SoundEvent Crash { get; set; }

	public void OnTriggerEnter( Collider other ) {
		if (other.Tags.Has( "player" )) {
			GameObject target = other.GameObject;

			Angles angle = target.Transform.LocalRotation.Angles();
			angle.yaw += YawChange;

			target.Transform.LocalPosition += Vector3.Left * Pushback;
			target.Transform.LocalRotation = angle.ToRotation();

			if (PlyMove != null) {
				PlyMove.Speed = Math.Max( PlyMove.Speed - SpeedTax, 0f );
			}

			if (Stats != null) { Stats.HurtPlayer(); }
			if (Crash != null) { Sound.Play( Crash ); }
		}
	}
}
