using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : NetworkBehaviour
{
    [SyncVar] public string playerName;
    [SyncVar] public int playerScore;
    [SyncVar] private int playerHealth;

    public Color playerColor;

    // Fill this one!
    public GameObject playerObject;
}
