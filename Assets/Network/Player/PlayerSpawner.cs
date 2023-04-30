using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerSpawner : NetworkBehaviour
{
    /// <summary>
    /// singleton,
    /// must not be null, please
    /// </summary>
    private static PlayerSpawner Main;

    [SerializeField] private NetworkPrefabRef playerPrefab = NetworkPrefabRef.Empty;
    public List<Player> players {get; private set; } = new List<Player>();
    public Player LocalPlayer { get; private set; }

    [SerializeField] Trail trailPrefab;
    public override void Spawned() {
        Main = this;
        Vector3 spawnPosition = Random.insideUnitCircle * 1f;
        spawnPosition.z = spawnPosition.y;
        spawnPosition.y = 20f;

        var playerObject = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);
        Runner.SetPlayerObject(Runner.LocalPlayer, playerObject);
        LocalPlayer = playerObject.GetComponent<Player>();
    }

    public override void FixedUpdateNetwork() {
        players.RemoveAll(p => p == null);
        foreach (Player player in players) {
            if (player != LocalPlayer) {
                player.LookName(CameraManager.GetMainCameraPosition());
            }
        }
    }


    /// <summary>
    /// Called by Players when the player is spawned.
    /// Register the player to List
    /// </summary>
    public static void RegisterPlayer(Player player) {
        Main.players.Add(player);
    }

    public static Player GetPlayer(int playerId) {
        return Main.players.Find(p => p.PlayerId == playerId);
    }

    public static Trail RegisterTrail(Color color) {
        Trail trail = Instantiate(Main.trailPrefab, Main.transform);
        trail.SetColor(color);
        trail.SetActive(false);
        return trail;
    }

    public void SetTrail(bool active) {
        foreach (Player player in players) {
            player.SetTrail(active);
        }
    }
}
