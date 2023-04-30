using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static CameraManager main;

    [SerializeField] Transform mainCameraParent;
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera mapCamera;
    [SerializeField] Camera miniCamera;
    [SerializeField] GameObject miniMapCanvas;
    [SerializeField] RectTransform compass;

    void Awake ()
    {
        main = this;
        SetActiveCamera(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.Main == null) return;
        if (Input.GetKeyDown(KeyCode.C) && !CanvasManager.IsInputing()) {
            SetActiveCamera(!mainCamera.enabled);
        }
        if (Input.GetKeyDown(KeyCode.F) && !CanvasManager.IsInputing()) {
            main.mainCameraParent.localRotation = Quaternion.identity;
        }
        compass.rotation = Quaternion.Euler(0f, 0f, miniCamera.transform.rotation.eulerAngles.y);
    }

    /// <summary>
    /// Activate one of the Main camera and Map camera, and deactivate the other.
    /// </summary>
    /// <param name="isMain">Activate Main camera or not/param>
    public static void SetActiveCamera(bool isMain) {
        main.mainCamera.enabled = isMain;
        main.mapCamera.enabled = !isMain;
        main.miniCamera.enabled = isMain;
        main.miniMapCanvas.SetActive(isMain);
    }

    /// <summary>
    /// Called by Player
    /// </summary>
    /// <param name="parent">player</param>
    public static void SetMainCameraParent(Transform parent) {
        main.mainCameraParent.parent = parent;
        main.mainCameraParent.localPosition = Vector3.zero;
        main.mainCamera.transform.localPosition = new Vector3(0f, 5f, -10f);
        main.miniCamera.transform.parent = parent;
        main.miniCamera.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Called by Player
    /// </summary>
    public static Vector3 GetMainCameraPosition() {
        return main.mainCamera.transform.position;
    }

    /// <summary>
    /// Called by Player
    /// </summary>
    public static void ForwardMainCamera(float amount) {
        if (amount > 0 && main.mainCamera.transform.localPosition.z > 0f) return;
        main.mainCamera.transform.Translate(Vector3.forward * amount);
    }

    /// <summary>
    /// Called by Player
    /// </summary>
    public static void RotateMainCamera(float amount) {
        float angleX = main.mainCameraParent.localEulerAngles.x;
        if (amount > 0 && angleX < 180f && angleX > 80f) return;
        if (amount < 0 && angleX > 180f && angleX < 330f) return;
        main.mainCameraParent.Rotate(amount, 0f, 0f);
    }
}
