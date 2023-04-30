using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField] Renderer[] renderers;
    [SerializeField] Transform body;
    [SerializeField] Transform tail;

    public void SetActive(bool value) {
        gameObject.SetActive(value);
    }

    public void SetColor(Color color) {
        foreach (var r in renderers) {
            r.material.color = color;
        }
    }

    /// <summary>
    /// Set Trail position and rotation
    /// </summary>
    /// <param name="positionAndRotation">x, z are position and y is rotation</param>
    public void SetPositionAndRotation(Vector3 positionAndRotation) {
        Vector3 bodyPosition = positionAndRotation;
        bodyPosition.y = 0f;
        body.localPosition = bodyPosition;
        body.rotation = Quaternion.Euler(0f, 180f + positionAndRotation.y, 0f);
        tail.localPosition = bodyPosition;
        tail.localScale = Vector3.one * 4f;
        tail.rotation = Quaternion.Euler(0f, positionAndRotation.y, 0f);
    }

    /// <summary>
    /// Set Trail position and rotation
    /// </summary>
    /// <param name="positionAndRotation">x, z are position and y is rotation</param>
    public void SetPositionAndRotation(Vector3 positionAndRotation, Vector3 previous) {
        Vector3 bodyPosition = positionAndRotation;
        bodyPosition.y = 0f;
        previous.y = 0f;
        body.localPosition = bodyPosition;
        body.rotation = Quaternion.Euler(0f, 180f + positionAndRotation.y, 0f);
        tail.localPosition = (bodyPosition + previous) / 2;
        Vector3 delta = bodyPosition - previous;
        tail.localScale = new Vector3(delta.magnitude / 10f, 1f, 1f);
        delta.Normalize();
        tail.rotation = Quaternion.Euler(0f, -Mathf.Atan2(delta.z, delta.x) * Mathf.Rad2Deg, 0f);
    }
}
