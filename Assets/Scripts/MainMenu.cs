using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_Text queueStatusText;
    [SerializeField] TMP_Text queueTimer;
    [SerializeField] TMP_Text findMatchButtonText;
    [SerializeField] TMP_InputField joinCodeField;

    bool isMatchMaking;
    bool isCancelling;

    private void Start()
    {
        if (ClientSingleton.Instance == null) return;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        queueStatusText.text = string.Empty;
        queueTimer.text = string.Empty;
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }

    public async void FindMatchPressed()
    {
        if (isCancelling) return;

        if (isMatchMaking)
        {
            queueStatusText.text = "Cancelling...";
            isCancelling = true;
            await ClientSingleton.Instance.GameManager.CancelMatchmaking();
            isCancelling = false;
            findMatchButtonText.text = "Find Match";
            queueStatusText.text = string.Empty;
            return;

        }

        ClientSingleton.Instance.GameManager.MatchmakeAsync(OnMatchMade);
        findMatchButtonText.text = "Cancel";
        queueStatusText.text = "Searching for match...";
        isMatchMaking = true;
    }

    void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                queueStatusText.text = "Match Found! Connecting...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                queueStatusText.text = "Ticket Creation Error!";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                queueStatusText.text = "Ticket Retrieval Error!";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                queueStatusText.text = "Match Assignment Error!";
                break;
        }
    }
}
