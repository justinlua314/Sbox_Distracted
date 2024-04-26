using System;

public sealed class SteeringWheel : Component {
	// PlayerMovement is referenced to keep wheel properties tuned the same as the cars properties
	PlayerMovement Movement;

	// Used for ABS Punch
	Random Rnd = new();

	float ABSPunchFactor, Steer;

	// Rotate wheel in a way similar to how the car rotates in PlayerMovement
	void SteerRotate( float delta ) {
		Angles curang = GameObject.Transform.LocalRotation.Angles();
		curang.pitch = Math.Max(Math.Min( curang.pitch + (delta * 8f), 80f ), -80f);

		GameObject.Transform.LocalRotation = curang.ToRotation();
	}

	// Punch the rotation of a wheel randomly left or right to simulate an ABS
	void ABSPunch( float strength ) {
		strength *= (Rnd.Next( 1, 3 ) == 1) ? -1f : 1f;
		SteerRotate( strength * ABSPunchFactor );
	}

	protected override void OnStart() {
		Movement = GameObject.Parent.Components.Get<PlayerMovement>();

		if (Movement == null) {
			ABSPunchFactor = 1f;
		} else {
			ABSPunchFactor = Movement.ABSPunchFactor;
		}
	}

	protected override void OnFixedUpdate() {
		if (Movement == null) { return; }
		float speedRatio = Movement.Speed / Movement.MaxSpeed;

		if (Input.Down("Jump")) {
			ABSPunch( Math.Max( speedRatio - 0.2f, 0f ) );
		}

		Steer = Input.AnalogMove.y * speedRatio;
		SteerRotate( Movement.TurnSpeed * Steer );
	}
}
