public sealed class Oobscript : Component, Component.ITriggerListener {
	/// <summary>
	/// PlayerStats is referenced to kill the player instantly if they go out of bounds
	/// </summary>
	[Property] PlayerStats Stats { get; set; }

	/// <summary>
	/// The sound that plays when the Player goes outside the map
	/// </summary>
	[Property] SoundEvent Crash { get; set; }

	public void OnTriggerEnter(Collider other) {
		if (other.Tags.Has("player") && Stats != null) {
			while (Stats.Hearts > 0) {
				Stats.HurtPlayer();
				if (Crash != null) { Sound.Play( Crash ); }
			}
		}
	}
}
