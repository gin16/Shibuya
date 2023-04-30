using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{

    private NetworkTransform networkTransform;
    [HideInInspector] [Networked] public int PlayerId {get; set; } = -2;
    [HideInInspector] [Networked(OnChanged = nameof(OnChangedName))] public string PlayerName {get; set; } = "";
    [Networked] private TickTimer lifeTimer { get; set; }

    [Networked, Capacity(256)]
    NetworkArray<int> collections => default;
    [Networked] private int collectionsSize { get; set; }
    [Networked] public int Score { get; private set; }
    const int TrailSize = 64;
    [Networked] private int trailsSize { get; set; }
    [Networked, Capacity(TrailSize)]
    NetworkArray<Vector3> trailData => default;
    List<Trail> trails = new List<Trail>();
    bool trailTeleported;
    [SerializeField] Renderer[] renderers;
    [SerializeField] TMPro.TextMeshPro nameText;
    [SerializeField] Transform positionPlane;
    [SerializeField] bool useGravity = true;
    float gravityVelocity = 0f;
    float jumpingTime;
    float jumpExtensionTime;

    bool registered = false;

    [Networked] private NetworkButtons _buttonsPrevious { get; set; }

    /// <summary>
    /// For displaying in inspector view
    /// </summary>
    [Tooltip("For displaying in inspector view")] [SerializeField] int InputAuthorityPlayerId;

    /// <summary>
    /// For displaying in inspector view
    /// </summary>
    [Tooltip("For displaying in inspector view")] [SerializeField] int StateAuthorityPlayerId;

    void Awake()
    {

    }

    public override void Spawned() {
        PlayerSpawner.RegisterPlayer(this);
        if (!Object.HasStateAuthority) return;
        networkTransform = GetComponent<NetworkTransform>();

        PlayerId = Runner.LocalPlayer.PlayerId;
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 8f);

        CameraManager.SetMainCameraParent(transform);
        CameraManager.SetActiveCamera(true);
    }

    public override void FixedUpdateNetwork()
    {
        InputAuthorityPlayerId = Object.InputAuthority.PlayerId;
        StateAuthorityPlayerId = Object.StateAuthority.PlayerId;

        if (!registered) {
            if (PlayerId < 0) return;
            foreach (Renderer renderer in renderers) {
                renderer.material.color = Parameter.GetColor(PlayerId, 0.75f);
            }
            registered = true;
        }
        if (nameText.text != PlayerName) {
            nameText.text = PlayerName;
        }
        
        if (lifeTimer.Expired(Runner)) {
            Debug.Log($"Despawn {PlayerId}");
            // Runner.Despawn(this.GetComponent<NetworkObject>());
            gameObject.SetActive(false);
        }

        // Generate little by little so that the load does not become heavy
        while (trails.Count < TrailSize) {
            Trail trail = PlayerSpawner.RegisterTrail(Parameter.GetColor(PlayerId, 0.5f));
            trails.Add(trail);
        }

        if (!Object.HasStateAuthority) return;

        if (lifeTimer.RemainingTime(Runner) < 4f) {
            lifeTimer = TickTimer.CreateFromSeconds(Runner, 8f);
            // Debug.Log($"Update {PlayerId}");
        }
        // PlayerName = MainNet.Main.GetName();

        MoveCamera();
        RotateWithMouseCursor();

        if (useGravity) {
            MoveWithGravity();
        }
        
        // Check coin
        if (Game.Main.Phase == GamePhase.Game && !Game.Main.Busy) {
            Collider[] coinColliders = Physics.OverlapSphere(transform.position, 1f, Parameter.CoinLayer);
            foreach (Collider collider in coinColliders) {
                Coin coin = collider.GetComponent<Coin>();
                if (coin != null && coin.PlayerId == -1) {
                    StartCoroutine(Game.Main.ClaimCoin(this, coin));
                }
            }
        }

        // GetInput() can only be called from NetworkBehaviours.
        // In SimulationBehaviours, either TryGetInputForPlayer<T>() or GetInputForPlayer<T>() has to be called.
        // This will only return true on the Client with InputAuthority for this Object and the Host.
        if (Runner.TryGetInputForPlayer<LocalInput>(Object.InputAuthority, out var input))
        {
            float speed = Parameter.MoveSpeed;
            Move(new Vector3(input.HorizontalInput, 0f, input.VerticalInput) * speed);

            if (input.Buttons.WasPressed(_buttonsPrevious, LocalButtons.Space)) {
                jumpExtensionTime = Parameter.JumpExtensionTime;
            } else if (jumpExtensionTime > 0f) {
                jumpExtensionTime -= Runner.DeltaTime;
            }

            if (jumpExtensionTime > 0f && OnGround()) {
                jumpExtensionTime = 0f;
                jumpingTime = Parameter.JumpTime;
            }
            if (input.Buttons.WasReleased(_buttonsPrevious, LocalButtons.Space)) {
                jumpingTime = 0f;
            } else if (jumpingTime > 0f) {
                jumpingTime -= Runner.DeltaTime;
                gravityVelocity = Parameter.JumpVelociy;
            }

            if (input.Buttons.WasPressed(_buttonsPrevious, LocalButtons.Return) && !CanvasManager.IsInputing()) {
                transform.position = PositionManager.GetRandomPosition(1, true);
                trailTeleported = true;
            }

            _buttonsPrevious = input.Buttons;
        }
        SaveTrail();
    }

    private void RotateWithMouseCursor() {
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up, mouseX * Parameter.HorizontalSensitivity * Runner.DeltaTime, Space.World);

        float mouseY = Input.GetAxis("Mouse Y");
        CameraManager.RotateMainCamera(mouseY * Parameter.VerticalSensitivity * Runner.DeltaTime);
    }

    private void MoveWithGravity() {
        gravityVelocity -= Parameter.GravityAcceleration * Runner.DeltaTime;
        Vector3 velocity = Vector3.up * gravityVelocity;
        if (Physics.SphereCast(transform.position, 1f, velocity, out RaycastHit hit, Mathf.Abs(gravityVelocity), Parameter.GroundLayer)) {
            gravityVelocity = 0f;
        } else {
            transform.Translate(velocity, Space.World);
        }
    }

    private bool OnGround() {
        return (Physics.SphereCast(transform.position, 1f, Vector3.down, out RaycastHit hit, 1f, Parameter.GroundLayer));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>if moved</returns>
    private bool Move(Vector3 velocity) {
        velocity.y = 0f;
        if (velocity.sqrMagnitude > 0f) {
            if (velocity.sqrMagnitude > 1f) {
                velocity.Normalize();
            }
            velocity.y = Parameter.InclineTolerance;
            velocity *= Parameter.MoveSpeed * Runner.DeltaTime;
            float magnitude = velocity.magnitude;
            if (Physics.SphereCast(transform.position, 1f, transform.TransformDirection(velocity), out RaycastHit hit, magnitude, Parameter.GroundLayer)) {
                for (int theta = 15; theta <= 60; theta += 15) {
                    Vector3 rotated = velocity.Rotated(theta);
                    if (!Physics.SphereCast(transform.position, 1f, transform.TransformDirection(rotated), out RaycastHit hitRight, magnitude, Parameter.GroundLayer)) {
                        transform.Translate(rotated);
                        return true;
                    }
                    rotated = velocity.Rotated(-theta);
                    if (!Physics.SphereCast(transform.position, 1f, transform.TransformDirection(rotated), out RaycastHit hitLeft, magnitude, Parameter.GroundLayer)) {
                        transform.Translate(rotated);
                        return true;
                    }
                }
            } else {
                transform.Translate(velocity);
                return true;
            }
        }
        
        return false;
    }

    /// <summary>
    /// move camera to avoid obstacles
    /// </summary>
    private void MoveCamera() {
        Vector3 cameraPosition = CameraManager.GetMainCameraPosition();
        Vector3 delta = transform.position - cameraPosition;
        if (Physics.Raycast(cameraPosition, delta, delta.magnitude, Parameter.GroundLayer)) {
            CameraManager.ForwardMainCamera(Parameter.CameraSpeed * Runner.DeltaTime);
        } else if (delta.sqrMagnitude < Parameter.CameraDistance * Parameter.CameraDistance) {
            CameraManager.ForwardMainCamera(-Parameter.CameraSpeed * Runner.DeltaTime);
        }
    }

    /// <summary>
    /// Save position and rotation
    /// </summary>
    private void SaveTrail() {
        if (Game.Main.Phase == GamePhase.Game) {
            if (trailsSize < TrailSize && trailsSize < Game.Main.GameTimeProgress * TrailSize) {
                // Don't save if it hasn't moved much (distance 10 or less) since the last time
                if (trailsSize > 0) {
                    Vector3 delta = transform.position - trailData[trailsSize - 1];
                    delta.y = 0f;
                    if (delta.sqrMagnitude < 100) {
                        return;
                    }
                }
                Vector3 data = transform.position;
                data.y = transform.rotation.eulerAngles.y;
                trailData.Set(trailsSize, data);
                trailsSize++;
                SetTrail(true);
                trailTeleported = false;
            }

        }
    }

    public void SetTrail(bool active) {
        // Debug.Log($"{PlayerId} SetTrail {trailsSize} {trails.Count} {active}");
        for (int i = 0; i < trails.Count; i++) {
            trails[i].SetActive(i < trailsSize);
            if (i < trailsSize) {
                if (i == 0 || trailTeleported) {
                    trails[i].SetPositionAndRotation(trailData[i]);
                } else {
                    trails[i].SetPositionAndRotation(trailData[i], trailData[i - 1]);
                }
            }
        }
    }

    public void Initialize() {
        if (!Object.HasStateAuthority) return;
        collectionsSize = 0;
        Score = 0;
        trailsSize = 0;
        trailTeleported = true;
    }

    public bool AddCoin(int value) {
        if (!Object.HasStateAuthority) return false;
        if (collectionsSize >= collections.Length) {
            Debug.LogError($"ERROR: Out of Range, collectionsSize[{collectionsSize}] >= collections.Length[{collections.Length}]");
            return false;
        }
        collections.Set(collectionsSize, value);
        collectionsSize++;
        Score += value;
        return true;
    }

    public void LookName(Vector3 position) {
        Vector3 delta = transform.position - position;
        delta.y = 0f;
        nameText.transform.rotation = Quaternion.LookRotation(delta, Vector3.up);
    }

    public static void OnChangedName(Changed<Player> changed) {
        if (Game.Main != null && Game.Main.Phase == GamePhase.Top) {
            Game.Main.ShowResult();
        }
    }
}
