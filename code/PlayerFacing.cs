public sealed class PlayerFacing : Component {
	/// <summary>
	/// The Transform that the camera will Interpolate to when looking to the side
	/// </summary>
	[Property] Transform SideLook { get; set; }


	/// <summary>
	/// The Speed at which Look Direction Interpolates
	/// </summary>
	[Property] float LookSpeed { get; set; } = 1f;


	/// <summary>
	/// Screen that Player can control when facing forward
	/// </summary>
	[Property] PhoneScreen Screen { get; set; }


	/// <summary>
	/// GameObject that is the parent to Item Hints that we want to enable
	/// when the Player looks at the Items
	/// </summary>
	[Property] GameObject ItemHints { get; set; }

	// The Transform that the camera will Interpolate to when looking forward
	Transform ForwardLook;

	// The Camera attached to the Player
	CameraComponent Camera;

	// Whether the Player is currently looking forward or not
	public bool LookingForward = true;

	// Whether the cameras Look Position should be changed
	bool ChangingLook = false;

	// If we've shown the Item Hints already
	bool HintsShown = false;
	

	protected override void OnStart() {
		Camera = Scene.Camera;
		ForwardLook = AnimHelper.ExtractLocalGOTransform( Camera.GameObject );
	}

	protected override void OnFixedUpdate() {
		if (ChangingLook && Camera != null) {
			bool settled;

			if (LookingForward) {
				settled = AnimHelper.InterpGameObjectToTransform(
					Camera.GameObject, SideLook, LookSpeed
				);
			} else {
				settled = AnimHelper.InterpGameObjectToTransform(
					Camera.GameObject, ForwardLook, LookSpeed
				);
			}

			if (settled) {
				LookingForward = !LookingForward;
				ChangingLook = false;

				if (!LookingForward && !HintsShown && ItemHints != null) {
					foreach (GameObject hint in ItemHints.Children) {
						if (hint != null) {
							hint.Enabled = true;
						}
					}
				}
			}
		}

		if (Input.Pressed("flashlight")) {
			ChangingLook = true;
		}

		if (Screen != null && LookingForward && Input.Pressed("attack2")) {
			Screen.NextGif();
		}
	}
}
