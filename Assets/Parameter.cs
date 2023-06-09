using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adjust parameters in the game.
/// Reference an instance that exists in the scene.
/// </summary>
public class Parameter : MonoBehaviour
{
    static Parameter main;

    [SerializeField] float moveSpeed = 32f;
    public static float MoveSpeed { get { return main?.moveSpeed ?? 0; }}
    [SerializeField] float jumpVelocity = 2f;
    public static float JumpVelociy { get { return main?.jumpVelocity ?? 0; }}
    [SerializeField] float jumpTime = 2f;
    public static float JumpTime { get { return main?.jumpTime ?? 0; }}
    [SerializeField] float jumpExtensionTime = 2f;
    public static float JumpExtensionTime { get { return main?.jumpExtensionTime ?? 0; }}
    [SerializeField] float inclineTolerance = 0.4f;
    public static float InclineTolerance { get { return main?.inclineTolerance ?? 0; }}
    [SerializeField] float gravityAcceleration = 9.8f;
    public static float GravityAcceleration { get { return main?.gravityAcceleration ?? 0; }}

    [SerializeField] float cameraDistance = 16f;
    public static float CameraDistance { get { return main?.cameraDistance ?? 0; }}
    [SerializeField] float cameraSpeed = 8f;
    public static float CameraSpeed { get { return main?.cameraSpeed ?? 0; }}

    [SerializeField] Color[] colors;
    public static Color GetColor(int index) {
        if (main == null || main.colors.Length == 0) return Color.white;
        return main.colors[index % main.colors.Length];
    }

    public static Color GetColor(int index, float alpha) {
        Color color = GetColor(index);
        color.a = alpha;
        return color;
    }

    [SerializeField] Slider horizontalSensitivitySlider;
    [SerializeField] Toggle horizontalSensitivityToggle;
    public static float HorizontalSensitivity { get { return Mathf.Pow(2, main?.horizontalSensitivitySlider.value ?? 8) * (main?.horizontalSensitivityToggle.isOn ?? true ? 1 : -1); } }

    [SerializeField] Slider verticalSensitivitySlider;
    [SerializeField] Toggle verticalSensitivityToggle;
    public static float VerticalSensitivity { get { return Mathf.Pow(2, main?.verticalSensitivitySlider.value ?? 8) * (main?.verticalSensitivityToggle.isOn ?? true ? 1 : -1); } }
    
    public static int GroundLayer { get; private set; }
    public static int CoinLayer { get; private set; }

    void Awake()
    {
        main = this;
        GroundLayer = LayerMask.GetMask("Ground");
        CoinLayer = LayerMask.GetMask("Coin");
    }
}
