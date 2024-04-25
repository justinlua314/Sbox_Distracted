using System;

public sealed class DrunkScript : Component {
	/// <summary>
	/// The Max Size the Cameras Blur will be set to when completely wasted
	/// </summary>
	[Property] float Intensity { get; set; } = 0.5f;


	/// <summary>
	/// The Speed at which the Camera Blur changes
	/// </summary>
	[Property] float BlurSpeed { get; set; } = 1f;


	/// <summary>
	/// How fast the Player Sobers up
	/// </summary>
	[Property] float SoberSpeed { get; set; } = 0.1f;

	Blur CamBlur;

	public float Intoxication = 0f;
	float BlurTarget = 0f;

	Random Rnd = new();

	protected override void OnStart() {
		CameraComponent camera = Scene.Camera;

		if (camera != null) {
			CamBlur = camera.Components.Get<Blur>();
		}
	}

	protected override void OnFixedUpdate() {
		if (Intoxication > 0f) {
			CamBlur.Size = AnimHelper.InterpFloat(
				CamBlur.Size, BlurTarget, BlurSpeed
			);

			if (CamBlur.Size == BlurTarget) {
				BlurTarget = (float)Rnd.Next( 0, (int)Intoxication ) / 100f;
			}
		} else if (CamBlur.Size > 0f) {
			CamBlur.Size = AnimHelper.InterpFloat(
				CamBlur.Size, 0f, BlurSpeed
			);
		}

		Intoxication = Math.Max( Intoxication - SoberSpeed, 0f );
	}
}
