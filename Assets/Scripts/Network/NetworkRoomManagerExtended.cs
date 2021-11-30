using Mirror;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NetworkRoomManagerExtended : NetworkRoomManager
{
    [Header("Custom UI")]
    public TMP_InputField ipAdress;
    public Transform lobbyPlayerParent;

    [Header("Game")]
    public GameObject SpawnPoints;
    public GameObject spawnPlayerSystem;
    public GameObject gameStateSystem;

    [SerializeField] public List<Color> possiblePlayerColors = new List<Color>();
    [NonSerialized] public List<Color> availablePlayerColors = null;

    public static event Action OnAvailableColorsChanged;
    public static event Action<NetworkConnection> OnServerReadied;

    #region Lobby UI

    // HOST BUTTON
    public void HostServer()
    {
        if (PlayerSettings.playerName == "") return;
        StartHost();
    }

    // JOIN BUTTON
    public void JoinServer()
    {
        if (PlayerSettings.playerName == "") return;
        // get the ip-adress from the Inputfield and try to join the server
        networkAddress = ipAdress.text;
        StartClient();        
    }

    // START BUTTON
    public void StartGame()
    {
        // Switch to the Gameplay-Scene if all players are ready
        if(allPlayersReady)
        {
            ServerChangeScene(GameplayScene);
        }        
    }

    // LEAVE BUTTON
    public void LeaveLobby()
    {
        if (NetworkClient.isHostClient)
        {
            // TODO: Assign other host here
            StopHost();
        }
        else
        {
            StopClient();
        }
    }

    // READY BUTTON
    public void ToggleReadyState()
    {
        foreach (NetworkRoomPlayerExtended roomPlayer in roomSlots)
        {
            if (roomPlayer.isLocalPlayer)
            {
                bool isReady = !roomPlayer.readyToBegin;
                roomPlayer.CmdChangeReadyState(isReady);                
            }
        }
    }
    #endregion

    #region Callbacks

    // Apply RoomPlayer information to GamePlayer object
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
        gamePlayer.GetComponent<GamePlayer>().playerName = roomPlayer.GetComponent<NetworkRoomPlayerExtended>().playerName;
        gamePlayer.GetComponent<GamePlayer>().playerColor = roomPlayer.GetComponent<NetworkRoomPlayerExtended>().playerColor;

        return true;
    }

    public override void OnStartClient()
    {      
        base.OnStartClient();
    }

    public override void OnRoomServerPlayersReady()
    {
        // Automatically start the game if the server is dedicated
        #if UNITY_SERVER
            base.OnRoomServerPlayersReady();
        #endif
    }

    public void OnPlayerColorChanged()
    {
        OnAvailableColorsChanged?.Invoke();
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        // Replace Roomplayer Connection with Gameplayer
        base.OnServerReady(conn);

        // Broadcast the OnServerReadied event for everyone subscribed to it as long as we are in gameplay
        if(SceneManager.GetActiveScene().path == GameplayScene) OnServerReadied?.Invoke(conn);
    }

    public override void OnRoomClientEnter()
    {
        base.OnRoomClientEnter();

        // Spawn RoomPlayerUI
        foreach (NetworkRoomPlayerExtended roomPlayer in roomSlots)
        {
            if (!roomPlayer.roomPlayerUIActive)
            {
                roomPlayer.SpawnLobbyPlayerUI(lobbyPlayerParent);
                roomPlayer.roomPlayerUIActive = true;
            }
        }

        OnPlayerColorChanged();
    }

    public override void OnRoomClientExit()
    {
        base.OnRoomClientExit();

        OnPlayerColorChanged();
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        // Deactivate every roomPlayerUI as we enter gameplay
        if (SceneManager.GetActiveScene().path == GameplayScene)
        {
            foreach (NetworkRoomPlayerExtended roomPlayer in roomSlots)
            {
                roomPlayer.roomPlayerUIActive = false;
            }
        }

        base.OnClientSceneChanged(conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if(sceneName == GameplayScene)
        {
            NetworkServer.Spawn(GameObject.Instantiate(spawnPlayerSystem));
            NetworkServer.Spawn(GameObject.Instantiate(gameStateSystem));
        }
    }

    #endregion
}
