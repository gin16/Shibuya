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

    [SerializeField] Slider sensitivitySlider;
    public static float Sensitivity { get { return Mathf.Pow(2, main?.sensitivitySlider.value ?? 8); } }
    
    public static int GroundLayer { get; private set; }
    public static int CoinLayer { get; private set; }

    void Awake()
    {
        main = this;
        GroundLayer = LayerMask.GetMask("Ground");
        CoinLayer = LayerMask.GetMask("Coin");
    }
}
