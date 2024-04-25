using Sandbox;

public sealed class GrabItem : Component {
	/// <summary>
	/// The speed at which this Item moves when grabbed
	/// </summary>
	[Property] float GrabSpeed { get; set; } = 1f;


	/// <summary>
	/// The Action (KeyPress) that the User must hit to grab/put back this Item
	/// </summary>
	[Property] string GrabAction { get; set; } = "";

	/// <summary>
	/// The GameObject that this Item Interpolates to when grabbed
	/// </summary>
	[Property] GameObject GrabTarget { get; set; }


	// Referenced by Finding Parent of Items Parent
	// Used for interfacing with PlayerFacing and PlayerInventory
	GameObject Player;

	// The PlayerFacing Component, referenced to toggle Moving
	PlayerFacing Facing;

	// PlayerInventory referenced to determine if we are holding this Item or not
	PlayerInventory Inventory;

	// The original location of the Item for when we want to put it back
	Transform PutBackTarget;

	// Is the Item interpolating from one place to another
	bool Moving = false;

	// Used to determine which direction the animation goes when Moving is true
	bool PickingUp;

	protected override void OnStart() {
		GameObject items = GameObject.Parent;
		if (items != null ) { Player = items.Parent; }

		if (Player == null) { return; }

		PutBackTarget = AnimHelper.ExtractLocalGOTransform( GameObject );

		Facing = Player.Components.Get<PlayerFacing>();
		Inventory = Player.Components.Get<PlayerInventory>();
	}

	protected override void OnFixedUpdate() {
		if (Player == null || Inventory == null) { return; }

		if (Moving) {
			if (PickingUp ) {
				if (GrabTarget != null) {
					Moving = !AnimHelper.InterpGameObjects(
						GameObject, GrabTarget, GrabSpeed
					);
				}
			} else {
				// VS says Transform PutBackTarget is never null. We'll see..
				Moving = !AnimHelper.InterpGameObjectToTransform(
					GameObject, PutBackTarget, GrabSpeed
				);
			}
			
		}

		if (Facing == null) { return; }

		if (!Facing.LookingForward && Input.Pressed(GrabAction)) {
			if (Inventory.Holding == GameObject) {
				PickingUp = false;
				Inventory.Holding = null;
			} else if (Inventory.Holding == null) { 
				PickingUp = true;
				Inventory.Holding = GameObject;
			}
			
			Moving = true;
		}
	}
}
