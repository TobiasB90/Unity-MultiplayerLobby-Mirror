using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGameStateManager : NetworkBehaviour
{
    public int round;
    public float countdownTime;

    public static List<GameObject> playerObjects = new List<GameObject>();

    [Server]
    public void ServerStartRound()
    {
        RpcStartRound();
        ServerStartRoundTimer();
    }

    private void Awake()
    {
        NetworkGameSpawnPlayers.OnAllPlayersSpawned += ServerStartRound;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    [Server]
    public void ServerStartRoundTimer()
    {
        StartCoroutine(RoundStartTimer());
    }

    [ClientRpc]
    public void RpcStartRound()
    {
        StartCoroutine(RoundStartTimer());
    }

    public IEnumerator RoundStartTimer()
    {
        float timer = countdownTime;

        while (timer >= 0)
        {
            Debug.Log(timer);

            yield return new WaitForSeconds(1);
            timer--;
        }

        if(isServer) ServerUnFreezePlayers();
    }

    [Server]
    public void ServerUnFreezePlayers()
    {
        foreach (GameObject playerObject in playerObjects)
        {
            playerObject.GetComponent<PlayerController>().RpcUnFreezePlayer();
        }
    }
}
