using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// temporary effect
/// </summary>
public class HopCoin : MonoBehaviour
{
    [SerializeField] float velocity;

    void Awake()
    {
        transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * velocity * Time.deltaTime);
        velocity -= 9.8f * 2f * Time.deltaTime;
        transform.Rotate(0f, 360f * 4f * Time.deltaTime, 0f);
        if (transform.localPosition.y < 0f) {
            Destroy(gameObject);
        }
    }

    public void Instantiate(Transform parent) {
        Instantiate(this, parent.position, Quaternion.identity, parent);
    }
}
