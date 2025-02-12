using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] TMP_InputField nameField;
    [SerializeField] Button connectButton;
    [SerializeField] int minNameLength = 4;
    [SerializeField] int maxNameLength = 20;

    public const string PlayerNameKey = "PlayerName";

    // Start is called before the first frame update
    void Start()
    {
        if(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);

        HandleNameChanged();
    }

    public void HandleNameChanged()
    {
        connectButton.interactable = (nameField.text.Length >= minNameLength) && (nameField.text.Length <= maxNameLength);
    }
    

    public void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey, nameField.text);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
