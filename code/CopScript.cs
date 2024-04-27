using System;

public sealed class CopScript : Component {
	/// <summary>
	/// How often the cop lights flash
	/// </summary>
	[Property] float LightSpeed { get; set; } = 1f;


	/// <summary>
	/// Player Movement is referenced to pull Player over if they stop while intoxicated
	/// </summary>
	[Property] PlayerMovement PlyMove { get; set; }


	/// <summary>
	/// Game Over Screen referenced to Enable when Player pulls over for police
	/// </summary>
	[Property] GameObject GameOverScreen { get; set; }


	/// <summary>
	/// Minimum time before cop switches to different siren
	/// </summary>
	[Property] int SirenMinFrequency { get; set; } = 1;


	/// <summary>
	/// Maximum time before cop switches to different siren
	/// </summary>
	[Property] int SirenMaxFrequency { get; set; } = 5;


	/// <summary>
	/// First Siren sound that Cop alternates between playing
	/// </summary>
	[Property] SoundEvent SirenOne { get; set; }


	/// <summary>
	/// First Siren sound that Cop alternates between playing
	/// </summary>
	[Property] SoundEvent SirenTwo { get; set; }

	// Cop Light colors
	List<Color> colors = new() {
		new Color(1f, 0f, 0f),
		new Color(0f, 0f, 1f)
	};

	// Whether the Cop Lights are flashing or not
	bool Flashing = false;

	// Switches between true and false to create flashing effect
	bool LightState = true;

	// Used for alternating siren
	Random Rnd = new();
	float SirenTimer = 0, SirenLimit;
	bool SirenOnePlaying = false;

	float FlashTimer = 0f;

	public void StartFlashing() {
		for (int i = 0; i < 2; i++) {
			SpotLight light = GameObject.Children[i].Components.Get<SpotLight>( true );

			light.LightColor = colors[i];
			light.Enabled = true;
		}

		Flashing = true;
	}

	void ResetSiren() {
		SirenLimit = Rnd.Next( SirenMinFrequency, SirenMaxFrequency );

		if (!Flashing) { return; }

		SirenOnePlaying = !SirenOnePlaying;
		Sound.StopAll(0f);

		if (SirenOnePlaying && SirenOne != null) {
			Sound.Play( SirenOne );
		} else if (SirenTwo != null) {
			Sound.Play( SirenTwo );
		}

		if (PlyMove != null && PlyMove.Speed > 0f) {
			Sound.Play( PlyMove.Gas );
		}
	}

	protected override void OnStart() {
		ResetSiren();
	}

	protected override void OnFixedUpdate() {
		if (!Flashing) { return; }

		if (PlyMove != null && PlyMove.Speed <= 0f && GameOverScreen != null) {
			GameOverScreen.Enabled = true;
			GameOverScreen.Components.Get<GameOver>().EndGame( "arrested" );
			Flashing = false;
		}

		FlashTimer += Time.Delta;

		if (FlashTimer >= LightSpeed) {
			FlashTimer -= LightSpeed;
			LightState = !LightState;

			if (LightState) {
				GameObject.Children[0].Components.Get<SpotLight>().LightColor = colors[0];
				GameObject.Children[1].Components.Get<SpotLight>().LightColor = colors[1];
			} else {
				GameObject.Children[0].Components.Get<SpotLight>().LightColor = colors[1];
				GameObject.Children[1].Components.Get<SpotLight>().LightColor = colors[0];
			}
		}

		SirenTimer += Time.Delta;

		if (SirenTimer > SirenLimit) {
			SirenTimer -= SirenLimit;
			ResetSiren();
		}
	}
}
