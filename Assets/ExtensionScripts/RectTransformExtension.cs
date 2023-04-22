using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class RectTransformExtension
{
    /// <summary>
    /// Return if the recttransform contains position
    /// </summary>
    /// <returns>if rect contains position</returns>
    public static bool Contains (this RectTransform rectTransform, Vector2 position) {
		var corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);
		return corners[0].x <= position.x && position.x <= corners[2].x && corners[0].y <= position.y && position.y <= corners[2].y;
	}

    /// <summary>
    /// Return if the recttransform contains mouse cursor
    /// </summary>
    /// <returns>if rect contains mouse cursor</returns>
    public static bool ContainsMouseCursor (this RectTransform rectTransform) {
		return rectTransform.Contains(Input.mousePosition);
	}
}