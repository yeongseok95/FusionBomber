using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class NetworkGameManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner runnerPrefab;
    public NetworkPrefabRef playerPrefab;
    private NetworkRunner _runner;

    async void Start()
    {
        _runner = Instantiate(runnerPrefab);
        _runner.AddCallbacks(this);

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "BomberRoom1",
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private List<PlayerController> _players = new List<PlayerController>();
    private bool _gameEnded = false;

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPosition = (player.RawEncoded % 2 == 0) ? new Vector3(1, 0, 1) : new Vector3(9, 0, 9);
            var playerObj = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
            _players.Add(playerObj.GetComponent<PlayerController>());
        }
    }

    private void Update()
    {
        if (_runner != null && _runner.IsServer && !_gameEnded)
        {
            CheckWinCondition();
        }
    }

    private void CheckWinCondition()
    {
        if (_players.Count < 2) return;

        PlayerController winner = null;
        int aliveCount = 0;

        foreach (var p in _players)
        {
            if (p != null && !p.IsDead)
            {
                aliveCount++;
                winner = p;
            }
        }

        if (aliveCount <= 1)
        {
            _gameEnded = true;
            if (winner != null) winner.ShowWinUI();
            Debug.Log("Game Over!");
        }
    }

    // 기타 필수 인터페이스 구현 (비워둠)
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}