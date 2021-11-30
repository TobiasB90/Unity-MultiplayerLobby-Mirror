using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkRoomPlayerExtended : NetworkRoomPlayer
{
    [Header("Player Info")]
    [SyncVar(hook = nameof(PlayerNameChanged))] public string playerName;
    [SyncVar(hook = nameof(PlayerColorChanged))] public Color playerColor;
    
    [Header("Lobby UI")]
    public GameObject roomPlayerUIPrefab;    
    public bool roomPlayerUIActive;

    public static event Action<NetworkRoomPlayerExtended, bool> OnRoomPlayerChanged;

    // Spawn the Lobby UI for every Roomplayer, this is not possible with NetworkServer.Spawn because the parent-object gets lost...
    public void SpawnLobbyPlayerUI(Transform parent)
    {        
        var playerobj = Instantiate(roomPlayerUIPrefab, parent);
        playerobj.GetComponent<RoomPlayerUI>().roomPlayerConn = connectionToServer;
        playerobj.GetComponent<RoomPlayerUI>().playerName.text = playerName;
        playerobj.GetComponent<RoomPlayerUI>().playerColor.GetComponent<Image>().color = playerColor;
    }

    // Hook to broadcast ReadyState, call Command on Client to set for everyone
    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        base.ReadyStateChanged(oldReadyState, newReadyState);

        OnRoomPlayerChanged?.Invoke(this, false);
    }

    // Hook to broadcast PlayerName, call Command on Client to set for everyone
    public void PlayerNameChanged(string oldName, string newName)
    {
        OnRoomPlayerChanged?.Invoke(this, false);
    }

    // Hook to broadcast PlayerColor, call Command on Client to set for everyone
    public void PlayerColorChanged(Color oldColor, Color newColor)
    {
        if (NetworkManager.singleton is NetworkRoomManagerExtended manager)
        {
            if (manager.possiblePlayerColors.Contains(oldColor)) manager.availablePlayerColors.Add(oldColor);
            manager.availablePlayerColors.Remove(newColor);

            manager.OnPlayerColorChanged();
        }
         
        OnRoomPlayerChanged?.Invoke(this, false);
    }

    // Broadcast playerName
    [Command]
    private void CmdSetPlayerName(string name)
    {
        playerName = name;
    }

    // Broadcast playerColor
    [Command]
    public void CmdSetPlayerColor(Color newColor)
    {
        playerColor = newColor;
        if (NetworkManager.singleton is NetworkRoomManagerExtended manager)
        {
            manager.OnPlayerColorChanged();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        //If this is our player, broadcast our name to everyone else
        if (isLocalPlayer)
        {
            playerName = PlayerSettings.playerName;
            CmdSetPlayerName(playerName);
        }

        if (NetworkManager.singleton is NetworkRoomManagerExtended manager)
        {
            if (isLocalPlayer)
            {                
                CmdSetPlayerColor(manager.availablePlayerColors[0]);
            }
        }

        OnRoomPlayerChanged?.Invoke(this, false);
    }

    // If the Client disconnects, destroy its UI (true)
    public override void OnStopClient()
    {
        base.OnStopClient();
        
        // Check if current scene is the GameplayScene - do not try to destroy Lobby UI then (since there is no Lobby UI)
        if (NetworkManager.singleton is NetworkRoomManagerExtended manager)
        {
            if(!manager.availablePlayerColors.Contains(playerColor)) manager.availablePlayerColors.Add(playerColor);
            manager.OnPlayerColorChanged();

            if (SceneManager.GetActiveScene().path == manager.GameplayScene)
            {
                return;
            }
        }

        OnRoomPlayerChanged?.Invoke(this, true);
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
    }
}
