using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;
using TMPro;

public class Entry : MonoBehaviour
{
    // [SerializeField] string sceneName;

    NetworkRunner runner = null;
    LocalInputCallBack localInputCallBack = null;

    public static bool Ready { get; private set; }

    void Start()
    {
        Ready = false;
        DontDestroyOnLoad(this);
        StartGame("RoomName");
    }

    async void StartGame(string roomName) {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs() {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });
        // _runnerInstance.SetActiveScene(sceneName);
        localInputCallBack = gameObject.AddComponent<LocalInputCallBack>();
        Ready = true;
    }
}
