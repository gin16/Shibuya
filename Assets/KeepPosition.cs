using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To Keep only specific coordinates (y) regardless of parent's position
/// </summary>
public class KeepPosition : MonoBehaviour
{
    public enum KeepPositionType { None, Local, Global, }

    [SerializeField] KeepPositionType yType;
    [SerializeField] float yPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (yType == KeepPositionType.Local) {
            Vector3 position = transform.localPosition;
            position.y = yPosition;
            transform.localPosition = position;
        }
        if (yType == KeepPositionType.Global) {
            Vector3 position = transform.position;
            position.y = yPosition;
            transform.position = position;
        }
    }
}
