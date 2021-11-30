using TMPro;
using Mirror;
using UnityEngine.UI;
using UnityEngine;

public class RoomPlayerUI : NetworkBehaviour
{
    public NetworkConnection roomPlayerConn;
    public TMP_Text playerName;
    public TMP_Text playerReady;
    public Button playerColor;

    private void Awake()
    {
        NetworkRoomPlayerExtended.OnRoomPlayerChanged += UpdatePlayerUI;
    }
    private void OnDestroy()
    {
        NetworkRoomPlayerExtended.OnRoomPlayerChanged -= UpdatePlayerUI;
    }

    private void UpdatePlayerUI(NetworkRoomPlayerExtended roomPlayer, bool isDestroyed)
    {
        if (roomPlayerConn != roomPlayer.connectionToServer) return;

        if (isDestroyed)
        {
            Destroy(gameObject);
        }

        playerColor.GetComponent<Image>().color = roomPlayer.playerColor;
        playerReady.text = roomPlayer.readyToBegin ? "READY" : "NOT READY";
        playerName.text = roomPlayer.playerName;
    }

    public override void OnStopServer()
    {
        //NetworkRoomPlayerExtended.OnRoomPlayerChanged -= UpdatePlayerUI;

        base.OnStopServer();
    }
}
