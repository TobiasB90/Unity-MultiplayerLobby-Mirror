using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField = null;
    public PlayerSettings settings;

    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        nameInputField.text = defaultName;

        if(nameInputField.text != "") SavePlayerName();
    }

    public void SavePlayerName()
    {
        PlayerSettings.playerName = nameInputField.text;
        PlayerPrefs.SetString(PlayerPrefsNameKey, PlayerSettings.playerName);
    }
}
