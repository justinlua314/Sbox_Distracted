public sealed class UseItem : Component {
	/// <summary>
	/// The GameObject that this Item Interpolates to when using ceases
	/// </summary>
	[Property] GameObject GrabTarget { get; set; }


	/// <summary>
	/// The Transform that the Item Interpolates to when
	/// preparing to use, or during the down time of the use animation
	/// </summary>
	[Property] Transform UseOff { get; set; }


	/// <summary>
	/// The Transform that the Item Interpolates to when
	/// the use animation is at its Apex
	/// </summary>
	[Property] Transform UseOn { get; set; }


	/// <summary>
	/// The speed at which the user arms the Item to use
	/// </summary>
	[Property] float PickupSpeed { get; set; } = 1f;


	/// <summary>
	/// The speed at which the user Interpolates from UseOff to UseOn or vise versa
	/// </summary>
	[Property] float UseSpeed { get; set; } = 1f;


	/// <summary>
	/// The amount of time that the Use Animation is held at an Apex
	/// before switching to the other Animation Target
	/// </summary>
	[Property] float HoldTime { get; set; } = 3f;


	/// <summary>
	/// The Stat that is satiated by using this Item
	/// </summary>
	[Property] string StatEffected { get; set; }


	/// <summary>
	/// How much the stat is effected when using this item
	/// </summary>
	[Property] float StatStrength { get; set; } = 1f;


	/// <summary>
	/// Sound that plays when item is held at Apex
	/// </summary>
	[Property] SoundEvent UseSound { get; set; }


	/// <summary>
	/// Should UseSound play to completion even if Use is not at Apex?
	/// </summary>
	[Property] bool StackSounds { get; set; } = false;

	SoundHandle UseHandle;


	// Referenced by Finding Parent of Items Parent
	// Used for interfacing with PlayerFacing and PlayerInventory
	GameObject Player;

	// The PlayerFacing Component, referenced to toggle Usable
	PlayerFacing Facing;

	// PlayerInventory referenced to determine if we are holding this Item or not
	PlayerInventory Inventory;

	// PlayerStats referenced to nourish StatEffected when Item is used
	PlayerStats Stats;

	// Controls flow of Animation Targets
	bool Armed = false;
	bool Using = true;
	bool Idle = true;

	// Is the Animation at an Apex
	bool AnimationApex = false;

	// Timer to measure how long we've held the Apex of an animation
	float HoldTimer = 0f;

	protected override void OnStart() {
		GameObject items = GameObject.Parent;
		if (items != null) { Player = items.Parent; }

		if (Player == null) { return; }

		Facing = Player.Components.Get<PlayerFacing>();
		Inventory = Player.Components.Get<PlayerInventory>();
		Stats = Player.Components.Get<PlayerStats>();
	}

	protected override void OnFixedUpdate() {
		// To Use, Player must exit, be Facing forward, and have this Item in their Inventory
		if (Player == null) { return; }
		if (Facing == null || !Facing.LookingForward) { return; }
		if (Inventory == null || !(Inventory.Holding == GameObject)) { return; }

		if (Input.Down("attack1")) {
			Idle = false;

			if (Armed) {
				if (!AnimationApex) {
					if (Using) {
						AnimationApex = AnimHelper.InterpGameObjectToTransform(
							GameObject, UseOn, UseSpeed
						);

						if (UseSound != null && AnimationApex) {
							UseHandle = Sound.Play( UseSound );
						}
					} else {
						AnimationApex = AnimHelper.InterpGameObjectToTransform(
							GameObject, UseOff, UseSpeed
						);
					}
				} else {
					HoldTimer += Time.Delta;

					if (Using && Stats != null) {
						Stats.Nourish(StatEffected, StatStrength);
					}

					if (HoldTimer >= HoldTime) {
						HoldTimer = 0f;
						AnimationApex = false;
						Using = !Using;
						if (!StackSounds && UseHandle != null) { UseHandle.Stop(); }
					}
				}
			} else {
				Armed = AnimHelper.InterpGameObjectToTransform(
					GameObject, UseOff, PickupSpeed	
				);
			}
		} else if (!Idle) {
			AnimationApex = false;
			HoldTimer = 0f;
			Idle = AnimHelper.InterpGameObjects( GameObject, GrabTarget, PickupSpeed );

			if (UseHandle != null && !UseHandle.IsStopped) { UseHandle.Stop(); }

			if (Idle) {
				Armed = false;
				Using = false;
			}
		}
	}
}
