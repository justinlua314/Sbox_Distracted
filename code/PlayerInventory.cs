public sealed class PlayerInventory : Component {
	// The Item that the Player is prepared to use
	// Null means no Items being held
	public GameObject Holding;

	// Facing referenced to determine if we can use the Item or not
	// Items can only be used when the Player is facing the Road
	PlayerFacing Facing;

	protected override void OnStart() {
		Facing = GameObject.Components.Get<PlayerFacing>();
	}

	protected override void OnFixedUpdate() {
		if (Facing == null) { return; }
	}
}
