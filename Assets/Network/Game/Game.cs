using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public enum GamePhase { Top, Ready, Game, }

/// <summary>
/// Manage whole game
/// </summary>
public class Game : NetworkBehaviour
{
    /// <summary>
    /// singleton,
    /// must not be null, please
    /// </summary>
    public static Game Main;

    [Networked(OnChanged = nameof(OnChangedPhase))]
    public GamePhase Phase { get; private set; } = GamePhase.Top;

    [SerializeField] GameOption coinNum;
    [SerializeField] GameOption gameTime;
    [Networked] TickTimer timer { get; set; }

    private List<Coin> coins = new List<Coin>();

    [Networked] private int NextCoin { get; set; }
    [SerializeField] private NetworkPrefabRef coinPrefab = NetworkPrefabRef.Empty;
    [SerializeField] CanvasManager canvasManager;
    [SerializeField] PlayerSpawner playerSpawner;


    public bool Busy { get; private set; }

    /// <summary>
    /// For displaying in inspector view
    /// </summary>
    [Tooltip("For displaying in inspector view")] [SerializeField] int AuthorityPlayerId;

    public override void Spawned() {
        Debug.Log("Game Spawned");
        Main = this;
        UpdatePhase();
    }

    public override void FixedUpdateNetwork() {
        if (Phase == GamePhase.Game) {
            // Display game progress
            canvasManager.UpdateGameText(playerSpawner.LocalPlayer.Score, NextCoin, CalcRemaining(), Mathf.CeilToInt(timer.RemainingTime(Runner) ?? 0));
            // Check game is over: time is expired or all coins have been found
            if ((timer.Expired(Runner) || NextCoin > coinNum.Value) && !Busy) {
                StartCoroutine(FinalizeGame());
            }
        }
        AuthorityPlayerId = Object.StateAuthority.PlayerId;
        if (!Object.HasStateAuthority) return;
    }

    private int CalcRemaining() {
        return coinNum.Value * (coinNum.Value + 1) / 2 - NextCoin * (NextCoin - 1) / 2;
    }


    public void OnChangedUserName() {
        if (Main == null || playerSpawner == null || playerSpawner.LocalPlayer == null) return;
        playerSpawner.LocalPlayer.PlayerName = canvasManager.UserName;
    }

    
    /// <summary>
    /// Called when Phase changed and call UpdatePhase
    /// </summary>
    /// <param name="changed"></param>
    public static void OnChangedPhase(Changed<Game> changed) {
        Debug.Log($"On Change Phase {changed.Behaviour.Phase}");
        changed.Behaviour.UpdatePhase();
    }

    /// <summary>
    /// Called by OnChangedPhase when Phase changed
    /// </summary>
    public void UpdatePhase() {
        Debug.Log($"Phase: {Phase}");
        if (Phase == GamePhase.Top) {
            CameraManager.SetActiveCamera(false);
            canvasManager.ShowResult(playerSpawner.players);
            foreach (Coin coin in coins) {
                coin.Show();
            }
        }
        if (Phase == GamePhase.Ready) {
            playerSpawner.LocalPlayer.PlayerName = canvasManager.UserName;
        }
        if (Phase == GamePhase.Game) {
            playerSpawner.LocalPlayer.Initialize();
            CameraManager.SetActiveCamera(true);
        }
        canvasManager.SetActive(Phase);
    }

    public void OnClickInitGame() {
        if (Main == null) return;
        if (Busy) return;
        StartCoroutine(InitializeGame());
    }

    IEnumerator InitializeGame() {
        Debug.Log($"{name} Initializa Game BEGIN");
        Busy = true;
        Object.RequestStateAuthority();
        for (float t = 0; !Object.HasStateAuthority; t += Time.deltaTime) {
            if (t > 1) {
                Debug.Log($"{name} Initializa Game TIME OUT");
                Busy = false;
                yield break;
            }
            yield return null;
        }
        Phase = GamePhase.Ready;
        NextCoin = 1;

        List<IEnumerator> runningCoroutines = new List<IEnumerator>();
        // Generate enough coins
        while (coins.Count < coinNum.Value) {
            Runner.Spawn(coinPrefab);
            yield return null;
        }
        // Run coroutines to initiate coins
        for (int i = 0; i < coins.Count; i++) {
            IEnumerator coroutine = coins[i].SetRandomPosition(i < coinNum.Value);
            StartCoroutine(coroutine);
            runningCoroutines.Add(coroutine);
        }

        // Wait until all coroutines complete
        while (runningCoroutines.Count > 0) {
            for (int i = runningCoroutines.Count - 1; i >= 0; i--) {
                IEnumerator coroutine = runningCoroutines[i];
                // remove completed coroutine from the list
                if (coroutine == null || !coroutine.MoveNext()) {
                    runningCoroutines.RemoveAt(i);
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);

        timer = TickTimer.CreateFromSeconds(Runner, gameTime.Value);
        Phase = GamePhase.Game;

        Busy = false;
        Object.ReleaseStateAuthority();
        Debug.Log($"{name} Initializa Game END");
    }

    IEnumerator FinalizeGame() {
        Debug.Log($"{name} Finalize Game BEGIN");
        Busy = true;
        Object.RequestStateAuthority();
        while (!Object.HasStateAuthority) {
            if (Phase != GamePhase.Game || (Object.StateAuthority.PlayerId != -1 && Object.StateAuthority.PlayerId != Runner.LocalPlayer.PlayerId)) {
                Debug.Log($"{name} Finalize Game TIME END a");
                Busy = false;
                yield break;
            }
            yield return null;
        }
        Phase = GamePhase.Top;
        Object.ReleaseStateAuthority();
        Busy = false;
        Debug.Log($"{name} Finalize Game END b");
    }

    /// <summary>
    /// Called by Coin when the coin is spawned.
    /// Register the coin to Game List
    /// </summary>
    /// <param name="coin"></param>
    public static void RegisterCoin(Coin coin) {
        Main.coins.Add(coin);
    }

    /// <summary>
    /// Called when a player hit coin.
    /// </summary>
    public IEnumerator ClaimCoin(Player player, Coin coin) {
        if (Busy || coin.PlayerId != -1) yield break;
        Debug.Log($"{name} {player.PlayerId} {coin} Claim Coin BEGIN");
        Busy = true;
        Object.RequestStateAuthority();
        coin.Object.RequestStateAuthority();
        for (float t = 0; !Object.HasStateAuthority || !coin.Object.HasStateAuthority; t += Time.deltaTime) {
            if (t > 1) {
                Debug.Log($"{name} {player.PlayerId} {coin} Claim Coin TIME OUT");
                Busy = false;
                yield break;
            }
            yield return null;
        }
        if (coin.Claim(player, NextCoin)) {
            NextCoin++;
        }
        Object.ReleaseStateAuthority();
        Busy = false;
        Debug.Log($"{name} {player.PlayerId} {coin} Claim Coin END");
    }
}
