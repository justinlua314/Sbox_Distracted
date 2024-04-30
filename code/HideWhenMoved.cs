public sealed class HideWhenMoved : Component {
	protected override void OnFixedUpdate() {
		if (Input.Pressed("Forward")) {
			foreach (GameObject hintObject in GameObject.Children) {
				UseHint hint = hintObject.Components.Get<UseHint>();

				if (hint != null) {
					hint.Hiding = true;
				}
			}

			Enabled = false;
		}
	}
}
