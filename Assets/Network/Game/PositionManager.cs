using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To provide random position
/// </summary>
public class PositionManager : MonoBehaviour
{
    /// <summary>
    /// singleton,
    /// must not be null, please
    /// </summary>
    static PositionManager Main;

    [SerializeField] float minX;
    [SerializeField] float maxX;
    [SerializeField] float minZ;
    [SerializeField] float maxZ;

    void Awake() {
        Main = this;
    }

    /// <summary>
    /// Get a random position within a range.
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetRandomPosition(float radius) {
        for (float t = 0; t < 1; t += Time.deltaTime) {
            Vector3 randomPosition = new Vector3(Random.Range(Main.minX, Main.maxX), 100f, Random.Range(Main.minZ, Main.maxZ));
            if (Physics.SphereCast(randomPosition, radius, Vector3.down, out RaycastHit hit, Mathf.Infinity, Parameter.GroundLayer)) {
                randomPosition.y = hit.point.y + radius;
                if (randomPosition.y > 0f) return randomPosition;
            }
        }
        Debug.Log("ERROR Get Random Position");
        return new Vector3(Random.Range(Main.minX, Main.maxX), 100f, Random.Range(Main.minZ, Main.maxZ));
    }
}
