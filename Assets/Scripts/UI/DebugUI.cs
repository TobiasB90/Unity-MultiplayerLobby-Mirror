using Mirror;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [SerializeField] NetworkRoomManagerExtended roomMng;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void ToggleDebugMode()
    {
        NetworkManagerHUD debugHud = roomMng.gameObject.GetComponent<NetworkManagerHUD>();
        debugHud.enabled = !debugHud.enabled;
        roomMng.showRoomGUI = !roomMng.showRoomGUI;
    }
}
