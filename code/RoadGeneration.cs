public sealed class RoadGeneration : Component {
	/// <summary>
	/// The GameObject that will be cloned and made a child of the GameObject this component is attached to
	/// </summary>
	[Property] GameObject MasterRoadSegment { get; set; }


	/// <summary>
	/// PlayerMovement's public float Speed is used
	/// to move the roads at the correct speed
	/// </summary>
	[Property] PlayerMovement PlyMovement { get; set; }


	/// <summary>
	/// The amount of road segments spawned in at any given time
	/// Each road segment is 1024 units long
	/// </summary>
	[Property] int RoadLength { get; set; } = 32;


	/// <summary>
	/// TrafficController referenced to Spawn Car when a road segment despawns
	/// </summary>
	[Property] TrafficController Traffic { get; set; }

	float BehindCamera;

	// Spawn the first road segment at Vector3.Zero
	// Spawn every next one at the position of the previous with 1024 added to x
	void SpawnSegment() {
		if (MasterRoadSegment == null) { return; }

		Vector3 targetPosition = Vector3.Zero;

		if (GameObject.Children.Count > 0) {
			GameObject lastChild = GameObject.Children.Last();

			if (lastChild != null) {
				targetPosition = lastChild.Transform.LocalPosition;
				targetPosition.x += 1024f;
			}
		}

		Transform targetTransform = AnimHelper.ExtractLocalGOTransform(
			MasterRoadSegment
		);

		targetTransform.Position = targetPosition;
		MasterRoadSegment.Clone(targetTransform, GameObject);
	}

	protected override void OnStart() {
		for (int i = 0;  i < RoadLength; i++) {
			SpawnSegment();
		}

		// Get x position where road segments should be despawned and replaced with new ones
		CameraComponent Camera = Scene.Camera;

		if (Camera != null) {
			BehindCamera = Camera.Transform.Position.x - 1024f;
		}
	}

	protected override void OnFixedUpdate() {
		if (GameObject.Children.Count == 0) { return; }
		if (PlyMovement == null) { return; }

		Vector3 roadMovement = Vector3.Backward * PlyMovement.Speed;

		foreach (GameObject segment in GameObject.Children) {
			segment.Transform.LocalPosition += roadMovement;
			segment.Transform.ClearInterpolation();
		}

		if (GameObject.Children[0].Transform.LocalPosition.x <= BehindCamera) {
			GameObject.Children[0].Destroy();
			GameObject.Children.RemoveAt( 0 );
			SpawnSegment();

			if (Traffic != null && Traffic.Rnd.Next(0, 6) == 2) {
				Traffic.SpawnCar();
			}
		}
	}
}
