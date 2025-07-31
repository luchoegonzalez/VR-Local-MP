using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for all the HUD Panels
/// </summary>
public class HUDController : MonoBehaviour
{
    [SerializeField] IPanelController[] panels;

    private void Start()
    {
        LocalXRINetworkGameManager gameManager = FindFirstObjectByType<LocalXRINetworkGameManager>();

        if (panels == null || gameManager == null)
        {
            Debug.LogError("HUDController: Missing references");
            return;
        }

        gameManager.onGameStarted += Activate;
        // gameManager.onGameFinished += Deactivate;

        Deactivate();
    }

    public void Activate()
    {
        this.gameObject.SetActive(true);
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].Initiate();
        }
    }

    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

}
