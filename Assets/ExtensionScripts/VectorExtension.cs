using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class VectorExtension
{
    /// <summary>
    /// return Vector rotated around the y-axis
    /// </summary>
    /// <returns></returns>
    public static Vector3 Rotated(this Vector3 vector, float euler) {
		return Quaternion.AngleAxis(euler, Vector3.up) * vector;
    }
}