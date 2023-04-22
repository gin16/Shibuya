using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Toggle visible or invisible by right-clicking
        // Freeze Cursor when invisible
        if (Input.GetMouseButtonDown(1)) {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = (Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked);
        }
    }
}
