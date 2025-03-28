using UnityEngine;

public class ProxyTile : Tile
{
	public Tile bodyParent;

	public override Rigidbody2D body {
		get {
			if (bodyParent != null) {
				return bodyParent.body;
			}
			return null;
		}
	}

	public override SpriteRenderer sprite {
		get {
			if (bodyParent != null) {
				return bodyParent.sprite;
			}
			return null;
		}
	}
}
