using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField] Button startHost;
    [SerializeField] Button joinServer;
    [SerializeField] Button startGame;
    [SerializeField] Button leaveLobby;
    [SerializeField] Button changeReady;
    [SerializeField] Transform lobbyUI;
    [SerializeField] TMP_InputField ipInputField;
    [SerializeField] GameObject colorPicker;

    private void Start()
    {
        NetworkRoomManagerExtended.OnAvailableColorsChanged += UpdateColorPicker;

        if (NetworkManager.singleton is NetworkRoomManagerExtended manager)
        {
            startHost.onClick.AddListener(manager.HostServer);
            joinServer.onClick.AddListener(manager.JoinServer);
            startGame.onClick.AddListener(manager.StartGame);
            leaveLobby.onClick.AddListener(manager.LeaveLobby);
            changeReady.onClick.AddListener(manager.ToggleReadyState);
            manager.lobbyPlayerParent = lobbyUI;
            manager.ipAdress = ipInputField;

            if(manager.availablePlayerColors == null) manager.availablePlayerColors = new List<Color>(manager.possiblePlayerColors);
        }
    }

    public void ToggleColorPicker()
    {
        UpdateColorPicker();

        colorPicker.SetActive(!colorPicker.activeInHierarchy);
    }

    public void UpdateColorPicker()
    {
        if (NetworkManager.singleton is NetworkRoomManagerExtended manager)
        {
            int counter = 0;
            foreach(Transform child in colorPicker.transform)
            {
                if(manager.availablePlayerColors.Count > counter)
                {
                    Color color = manager.availablePlayerColors[counter];
                    child.GetComponent<Image>().color = color;
                    child.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                    child.gameObject.GetComponent<Button>().onClick.AddListener(() => ChooseColor(color));
                }
                else
                {
                    child.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                    child.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                    child.gameObject.GetComponent<Button>().interactable = false;
                }

                counter++;
            }
        }
    }

    private void OnDestroy()
    {
        NetworkRoomManagerExtended.OnAvailableColorsChanged -= UpdateColorPicker;
    }

    void ChooseColor(Color color)
    {
        if (NetworkManager.singleton is NetworkRoomManagerExtended manager)
        {
            foreach(NetworkRoomPlayerExtended roomPlayer in manager.roomSlots)
            {
                if (!roomPlayer.isLocalPlayer) continue;

                roomPlayer.CmdSetPlayerColor(color);
            }
        }
    }


}
