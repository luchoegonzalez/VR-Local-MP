using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRMultiplayer;

public class GameChanger : MonoBehaviour
{
    // trigger this function to change the game
    [ContextMenu("Change Name and Color")]
    public void ChangeNameAndColor()
    {
        string[] names = new string[] { "John", "Jane", "Jack", "Jill", "James", "Jenny", "Jasper" };
        XRINetworkGameManager.LocalPlayerName.Value = names[Random.Range(0, names.Length)];

        Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.white };
        XRINetworkGameManager.LocalPlayerColor.Value = colors[Random.Range(0, colors.Length)];
    }

}
