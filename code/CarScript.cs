public sealed class CarScript : Component, Component.ITriggerListener {
	/// <summary>
	/// Player Stats is referenced to damage Player when car is hit
	/// </summary>
	[Property] PlayerStats Stats { get; set; }

	
	/// <summary>
	/// The sound that plays when the Player crashes into a car
	/// </summary>
	[Property] SoundEvent Crash { get; set; }

	public float Speed = 0f;

	public void OnTriggerEnter(Collider other) {
		if (other.Tags.Has("player") && Stats != null) {
			Stats.HurtPlayer();

			if (Crash != null) { Sound.Play( Crash ); }
		} else if (other.Tags.Has("traffic")) {
			float pos = other.GameObject.Transform.Position.x;

			if (pos > GameObject.Transform.Position.x) {
				float otherSpeed = other.GameObject.Components.Get<CarScript>().Speed;

				GameObject.Transform.Position += Vector3.Backward * 50f;
			}
		}
	}
}
