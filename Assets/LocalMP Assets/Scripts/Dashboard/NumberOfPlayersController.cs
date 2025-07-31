using UnityEngine;
using XRMultiplayer;

public class NumberOfPlayersController : MonoBehaviour
{

    [SerializeField] private TMPro.TextMeshProUGUI numberOfPlayersText;
    private XRINetworkGameManager gameManager;

    private void Start()
    {
        gameManager = XRINetworkGameManager.Instance;
        gameManager.playerStateChanged += UpdateNumberOfPlayers;
    }

    private void OnDestroy()
    {
        gameManager.playerStateChanged -= UpdateNumberOfPlayers;
    }

    private void UpdateNumberOfPlayers(ulong clientID, bool connected)
    {
        numberOfPlayersText.text = gameManager.m_CurrentPlayerIDs.Count.ToString();
    }

}
