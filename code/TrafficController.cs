using System;

public sealed class TrafficController : Component {
	/*
	/// <summary>
	/// The Minimum time we wait before spawning another car
	/// </summary>
	[Property] float SpawnTimeMin { get; set; } = 20f;


	/// <summary>
	/// The Maximum time we wait before spawning another car
	/// </summary>
	[Property] float SpawnTimeMax { get; set; } = 100f;
	*/

	/// <summary>
	/// The Car that is cloned whenever SpawnCar is called
	/// </summary>
	[Property] GameObject MasterCar { get; set; }


	/// <summary>
	/// PlayerMovement is referenced to get Speed at which Player is moving
	/// </summary>
	[Property] PlayerMovement PlyMove { get; set; }


	// The floats in this list represent the valid lanes that cars can spawn in
	List<float> ValidYPositions = new() {
		775f, 518f, 260f, -20f, -300f
	};

	public Random Rnd = new();

	float BehindCamera;

	public void SpawnCar() {
		if (MasterCar == null) { return; }

		Transform targetTransform = new();
		targetTransform.Rotation = MasterCar.Transform.LocalRotation;

		Vector3 targetPosition = new(
			18432f,
			ValidYPositions[Rnd.Next(0, ValidYPositions.Count)],
			// 260f,
			50f
		);

		targetTransform.Position = targetPosition;
		MasterCar.Clone( targetTransform, GameObject );

		GameObject.Children[GameObject.Children.Count - 1]
			.Components.Get<CarScript>()
			.Speed = (float)Rnd.Next( 20, 60 ) * 0.02f;
	}

	protected override void OnStart() {
		// Get x position where road segments should be despawned and replaced with new ones
		CameraComponent Camera = Scene.Camera;

		if (Camera != null) {
			BehindCamera = Camera.Transform.Position.x - 1024f;
		}
	}

	protected override void OnFixedUpdate() {
		if (PlyMove == null) { return; }

		if (GameObject.Children.Count == 0) { return; }

		foreach (GameObject car in GameObject.Children) {
			CarScript script = car.Components.Get<CarScript>();

			car.Transform.Position += Vector3.Backward * script.Speed * Math.Max(PlyMove.Speed, 1f);
		}

		if (GameObject.Children[0].Transform.Position.x <= BehindCamera) {
			GameObject.Children[0].Destroy();
			GameObject.Children.RemoveAt( 0 );
		}
	}
}
