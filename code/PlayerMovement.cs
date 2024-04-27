using System;

public sealed class PlayerMovement : Component {
	/// <summary>
	/// How quickly the car goes from one side of the road to the other when
	/// steering wheel is turned all the way
	/// </summary>
	[Property] float TurnPower { get; set; } = 300f;


	/// <summary>
	/// The speed at which the car turns when the steering wheel is turned
	/// </summary>
	[Property] public float TurnSpeed { get; set; } = 1f;


	/// <summary>
	/// Max Speed that car can drive
	/// </summary>
	[Property] public float MaxSpeed { get; set; } = 150f;


	/// <summary>
	/// The rate at which the car's Speed increases
	/// </summary>
	[Property] float Acceleration { get; set; } = 0.5f;


	/// <summary>
	/// The rate at which the car slows down abscent of brake usage
	/// Brakes are added to Friction when applied
	/// </summary>
	[Property] float Friction { get; set; } = 0.05f;


	/// <summary>
	/// The rate at which the normal brake (Backward) slows the car down
	/// </summary>
	[Property] float BrakePower { get; set; } = 0.4f;


	/// <summary>
	/// The rate at which the emergency brake (Jump) slows the car down
	/// </summary>
	[Property] float EmergencyBrakePower { get; set; } = 1f;


	/// <summary>
	/// How much does the steering jiggle around when the emergency brake is pulled
	/// </summary>
	[Property] public float ABSPunchFactor { get; set; } = 1f;


	/// <summary>
	/// The sound that plays when the Player is accelerating
	/// </summary>
	[Property] public SoundEvent Gas { get; set; }


	/// <summary>
	/// The sound that plays when the Player is idle
	/// </summary>
	[Property] SoundEvent Idle { get; set; }


	// Sound Control
	float IdleTimer = 0f, AccelerateTimer = 0f;


	// Stats is referenced to control Score while driving
	PlayerStats Stats;

	// The speed that the car is moving foward
	public float Speed = 0f;

	float FowardYaw, Steer;

	// Used for ABS Punch
	Random Rnd = new();

	// Used to swap pedals randomly when Player is Intoxicated
	bool PedalSwap = false;
	bool PedalPressed = false;

	protected override void OnStart() {
		FowardYaw = GameObject.Transform.LocalRotation.Angles().yaw;
		Stats = GameObject.Components.Get<PlayerStats>();
	}

	float CalculateTurnMovement() {
		float turnDifference = GameObject.Transform.LocalRotation.Angles().yaw - FowardYaw;
		float turnRatio = turnDifference / 80f;

		return TurnPower * turnRatio;
	}

	// Rotate the car by some amount
	void SteerRotate(float delta) {
		Angles curang = GameObject.Transform.LocalRotation.Angles();
		curang.yaw += delta;

		GameObject.Transform.LocalRotation = curang.ToRotation();
	}

	// Punch the rotation of a car randomly left or right to simulate an ABS
	void ABSPunch(float strength) {
		strength *= (Rnd.Next( 1, 3 ) == 1) ? -1f : 1f;
		SteerRotate(strength * ABSPunchFactor);
	}

	void Accelerate(float speedRatio) {
		Speed = Math.Min(
			Speed + Math.Max( Acceleration * (1f - speedRatio), 0.1f ), MaxSpeed
		);
	}

	void PressBrake() {
		Speed = Math.Max( Speed - BrakePower, 0f );
	}

	protected override void OnFixedUpdate() {
		if (Stats != null && (Stats.Hearts <= 0 || !Stats.Playing)) { return; }

		// Log.Info( Speed );
		float speedRatio = Speed / MaxSpeed;

		// Only one movement input at a time is intentional
		if ( Input.Down( "Jump" ) ) {
			Speed = Math.Max( Speed - EmergencyBrakePower, 0f );
			ABSPunch( Math.Max( speedRatio - 0.2f, 0f ) );
		} else if ( Input.Down( "Forward" ) ) {
			if (!PedalPressed && Stats.Drunk.Intoxication > 50f) {
				PedalPressed = true;
				PedalSwap = Rnd.Next( 0, 2 ) == 0;
			}

			if (PedalSwap) {
				PressBrake();
			} else {
				Accelerate( speedRatio );

				if (Input.Pressed( "Forward" )) {
					Sound.StopAll(1f);
					if (Gas != null) { Sound.Play( Gas ); }
					IdleTimer = 0f;
				}

				AccelerateTimer += Time.Delta;

				if (AccelerateTimer >= 6f) {
					Sound.StopAll( 1f );
					if (Gas != null) { Sound.Play( Gas ); }
					AccelerateTimer -= 6f;
				}
			}
		} else if ( Input.Down( "Backward" ) ) {
			if (!PedalPressed && Stats.Drunk.Intoxication > 50f) {
				PedalPressed = true;
				PedalSwap = Rnd.Next( 0, 2 ) == 0;
			}

			if (PedalSwap) {
				Accelerate( speedRatio );
			} else {
				PressBrake();
			}
		} else {
			if (IdleTimer == 0f) {
				PedalSwap = false;
				PedalPressed = false;
				AccelerateTimer = 0f;
				Sound.StopAll( 100f );
				if (Idle != null) { Sound.Play( Idle ); }
			}

			IdleTimer += Time.Delta;

			if (IdleTimer >= 6f) {
				Sound.StopAll( 1f );
				if (Idle != null) { Sound.Play( Idle ); }
				IdleTimer -= 6f;
			}
		}

		Speed = Math.Max(Speed - Friction, 0f);

		Steer = Input.AnalogMove.y * speedRatio;
		SteerRotate( TurnSpeed * Steer );

		// We move left and right, but never actually move forward.
		// The RoadGeneration Component creates the illusion of moving forward
		GameObject.Transform.LocalPosition += Vector3.Left * CalculateTurnMovement() * speedRatio;

		if (Stats != null && Stats.Drunk != null) {
			// Math hackery
			int drunkFactor = Math.Max( (int)(Stats.Drunk.Intoxication / 10f), 1 );
			int earned = ((int)Speed * drunkFactor - 50) / 20;

			Stats.Score = Math.Max( Stats.Score + earned, 0 );
		}
	}
}
