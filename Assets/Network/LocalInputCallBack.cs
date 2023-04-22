using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class LocalInputCallBack : MonoBehaviour, INetworkRunnerCallbacks
{

    // The INetworkRunnerCallbacks of this LocalInputPoller are automatically detected
    // because the script is located on the same object as the NetworkRunner and
    // NetworkRunnerCallbacks scripts.

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        LocalInput localInput = new LocalInput();

        localInput.HorizontalInput = Input.GetAxis("Horizontal");
        localInput.VerticalInput = Input.GetAxis("Vertical");
        localInput.Buttons.Set(LocalButtons.ClickLeft, Input.GetMouseButton(0));
        localInput.Buttons.Set(LocalButtons.ClickRight, Input.GetMouseButton(1));
        localInput.Buttons.Set(LocalButtons.Space, Input.GetKey(KeyCode.Space));
        localInput.Buttons.Set(LocalButtons.Return, Input.GetKey(KeyCode.Return));

        input.Set(localInput);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
        byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

}