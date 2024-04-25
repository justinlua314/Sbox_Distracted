/*
 * A lot of this is probably already in the S&box standard library
 * but hacking without documentation I understand is less organized
 * so we go manual
*/

using System;

public static class AnimHelper {
	// home approaches target at a constant speed
	// returns homes new target
	public static float InterpFloat(float home, float target, float speed) {
		float difference = Math.Abs(home - target);

		if (difference < speed) {
			return target;
		} else {
			return home + speed * (home < target ? 1 : -1);
		}
	}


	// Interpolate homes position and rotation to match target at a constant speed
	public static Transform InterpTransform(Transform home, Transform target, float speed) {
		Vector3 newPosition;

		if ( Vector3.DistanceBetween( home.Position, target.Position ) < speed ) {
			newPosition = target.Position;
		} else {
			newPosition = new(
				InterpFloat( home.Position.x, target.Position.x, speed ),
				InterpFloat( home.Position.y, target.Position.y, speed ),
				InterpFloat( home.Position.z, target.Position.z, speed )
			);
		}

		Angles oldAngles = home.Rotation.Angles();
		Angles targetAngles = target.Rotation.Angles();

		Angles newAngles = new(
			InterpFloat(oldAngles.pitch, targetAngles.pitch, speed),
			InterpFloat(oldAngles.yaw, targetAngles.yaw, speed),
			InterpFloat(oldAngles.roll, targetAngles.roll, speed)
		);

		return new Transform( newPosition, newAngles.ToRotation() );
	}


	// Convert GameTransforms into Transforms
	public static Transform ExtractGOTransform( GameObject scan ) {
		return new Transform( scan.Transform.Position, scan.Transform.Rotation );
	}

	public static Transform ExtractLocalGOTransform( GameObject scan ) {
		return new Transform( scan.Transform.LocalPosition, scan.Transform.LocalRotation );
	}


	// Interpolate one gameobject to another at a constant speed
	// returns true if homes new Position matches targets Position
	public static bool InterpGameObjects( GameObject home, GameObject target, float speed ) {
		Transform dest = InterpTransform(
			ExtractLocalGOTransform( home ),
			ExtractLocalGOTransform( target ),
			speed
		);

		home.Transform.LocalPosition = dest.Position;
		home.Transform.LocalRotation = dest.Rotation;

		return (
			dest.Position == target.Transform.LocalPosition &&
			home.Transform.LocalRotation == target.Transform.LocalRotation
		);
	}


	// Interpolate a gameobject to a Transform at a constant speed
	// returns true if homes new Position matches targets Position
	public static bool InterpGameObjectToTransform( GameObject home, Transform target, float speed ) {
		Transform dest = InterpTransform(
			ExtractLocalGOTransform( home ),
			target, speed
		);

		home.Transform.LocalPosition = dest.Position;
		home.Transform.LocalRotation = dest.Rotation;

		return (dest.Position == target.Position && home.Transform.LocalRotation == target.Rotation);
	}
}
