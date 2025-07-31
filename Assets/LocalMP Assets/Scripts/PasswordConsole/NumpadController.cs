using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the numpad buttons
/// </summary>
/// <remarks>
/// This class invokes an action when a button is clicked
/// </remarks>
public class NumpadController : MonoBehaviour
{
    [SerializeField] private NumpadButton[] numpadButtons;

    public Action<char> OnNumpadButtonPressed;

    private void Start()
    {
        foreach (var button in numpadButtons)
        {
            button.Button.onClick.AddListener(() => OnNumpadButtonPressed?.Invoke(button.Value));
        }
    }

    private void OnDestroy()
    {
        foreach (var button in numpadButtons)
        {
            button.Button.onClick.RemoveAllListeners();
        }
    }

    public void DeactivateButtons()
    {
        foreach (var button in numpadButtons)
        {
            button.Button.interactable = false;
        }
    }

    public NumpadButton[] NumpadButtons => numpadButtons;
}

[Serializable]
public class NumpadButton
{
    [SerializeField] private Button m_Button;
    [SerializeField] private char m_Value;

    public NumpadButton(Button button, char value)
    {
        m_Button = button;
        m_Value = value;
    }

    public Button Button => m_Button;
    public char Value => m_Value;
}
