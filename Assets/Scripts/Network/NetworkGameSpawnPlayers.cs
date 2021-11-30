using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGameSpawnPlayers : NetworkBehaviour
{
    public GameObject SpawnPointsPrefab;
    public GameObject playerPrefab;
    public GameObject SpawnPointParent;
    private static List<Transform> SpawnPoints = new List<Transform>();
    public static event Action OnAllPlayersSpawned;
    private int nextIndex = 0;

    public static void AddSpawnPoint(Transform t)
    {
        SpawnPoints.Add(t);
    }
    
    public override void OnStartServer() => NetworkRoomManagerExtended.OnServerReadied += SpawnPlayer;
    public override void OnStopServer() => NetworkRoomManagerExtended.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        GameObject playerObj = Instantiate(playerPrefab, SpawnPoints[nextIndex].position, SpawnPoints[nextIndex].rotation);
        
        NetworkServer.Spawn(playerObj, conn);
        playerObj.GetComponent<PlayerController>().playerColor = conn.identity.GetComponent<GamePlayer>().playerColor;
        playerObj.GetComponent<PlayerController>().RpcFreezePlayer();
        conn.identity.GetComponent<GamePlayer>().playerObject = playerObj;
        nextIndex++;

        //TODO Remove player from objects if he leaves
        NetworkGameStateManager.playerObjects.Add(playerObj);

        if (NetworkManager.singleton is NetworkRoomManagerExtended manager)
        {
            if (nextIndex == manager.roomSlots.Count)
            {
                OnAllPlayersSpawned?.Invoke();                
            }
        }
    }
}
