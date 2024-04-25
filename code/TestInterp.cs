public sealed class TestInterp : Component {
	[Property] Transform Target { get; set; }

	[Property] float Speed { get; set; } = 1f;

	Transform Start;

	protected override void OnStart() {
		Start = AnimHelper.ExtractLocalGOTransform( GameObject );
	}

	protected override void OnFixedUpdate() {
		bool arrived = AnimHelper.InterpGameObjectToTransform(
			GameObject, Target, Speed
		);

		if (arrived) {
			GameObject.Transform.LocalPosition = Start.Position;
			GameObject.Transform.LocalRotation = Start.Rotation;
		}
	}
}
