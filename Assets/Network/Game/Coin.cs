using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Coin : NetworkBehaviour
{
    [SerializeField] Transform body;
    [SerializeField] GameObject textObject;
    [SerializeField] TMPro.TextMeshPro valueText;
    [Networked(OnChanged = nameof(OnChangedPlayerId))] public int PlayerId { get; private set; }
    [Networked(OnChanged = nameof(OnChangedPlayerId))] public int Value { get; private set; }

    public Vector3 Position { get { return transform.position; } }

    [SerializeField] HopCoin hopCoinPrefab;
    float lastHopTime;

    public override void Spawned() {
        Game.RegisterCoin(this);
    }

    public override void FixedUpdateNetwork() {
        body.Rotate(0f, 0f, 90f * Runner.DeltaTime);
        if (Object.HasStateAuthority) {
            Object.ReleaseStateAuthority();
        }
    }

    /// <summary>
    /// Called when PlayerId or Value changes.
    /// When claimed by a player(PlayerId >= 0), inactivate the body and activate text on the map.
    /// Before claimed (PlayerId == -1) activate the body and inactivate text.
    /// When useless (PlayerId == -2) inactivate the body and text.
    /// </summary>
    /// <param name="changed"></param>
    public static void OnChangedPlayerId(Changed<Coin> changed) {
        Coin coin = changed.Behaviour;
        int playerId = changed.Behaviour.PlayerId;
        coin.textObject.SetActive(playerId >= 0);
        coin.body.gameObject.SetActive(playerId == -1);
        if (playerId >= 0) {
            coin.valueText.color = Parameter.GetColor(playerId);
            coin.valueText.text = "" + changed.Behaviour.Value;
            if (coin.lastHopTime + 2f < Time.time) {
                Player player = PlayerSpawner.GetPlayer(playerId);
                if (player != null) {
                    coin.lastHopTime = Time.time;
                    coin.hopCoinPrefab.Instantiate(player.transform);
                }
            }
        }
        Debug.Log($"Coin Changed {coin.Position} {coin.PlayerId} {coin.Value}");
    }

    /// <summary>
    /// Called when Game starts.
    /// Set position randomly if active.
    /// </summary>
    /// <returns></returns>
    public IEnumerator SetRandomPosition(bool active) {
        Debug.Log($"{name} Set Random Position BEGIN");
        Object.RequestStateAuthority();
        for (float t = 0; !Object.HasStateAuthority; t += Time.deltaTime) {
            if (t > 4) {
                Debug.Log($"{name} Set Random Position TIME OUT");
                yield break;
            }
            yield return null;
        }
        PlayerId = (active ? -1 : -2);
        if (active) {
            transform.position = PositionManager.GetRandomPosition(1f);
        }
        Object.ReleaseStateAuthority();
        Debug.Log($"{name} Set Random Position END");
    }


    /// <summary>
    /// Called when a player hit coin.
    /// </summary>
    /// <returns>if no problem</returns>
    public bool Claim(Player player, int value) {
        if (!Object.HasStateAuthority) {
            Debug.LogError($"ERROR P{player.PlayerId} C{PlayerId} claim {Position} but no State Authority");
            return false;
        }
        if (PlayerId != -1) {
            Debug.LogError($"ERROR P{player.PlayerId} C{PlayerId} claim {Position} but already Claimed");
            return false;
        }
        if (!player.AddCoin(value)) {
            return false;
        }
        Value = value;
        PlayerId = player.PlayerId;
        return true;
    }

    /// <summary>
    /// Called by Game when Game is over.
    /// Show in map.
    /// </summary>
    public void Show() {
        if (PlayerId == -1) {
            valueText.color = Color.white;
            valueText.text = "0";
            textObject.SetActive(true);
            gameObject.SetActive(true);
        }
    }
}
