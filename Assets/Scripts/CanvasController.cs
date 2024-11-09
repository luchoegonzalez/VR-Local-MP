using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

/// <summary>
/// Controller to toggle the visibility of the Canvas UI elements or GameObjects from the keyboard.
/// </summary>
/// <remarks>
/// This class is made to be used by the Host, which is the only one that uses the keyboard.
/// </remarks>
public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObjectKeyBinding[] gameObjectKeyBindings;
    private List<InputAction> inputActions = new List<InputAction>();

    private void OnEnable()
    {
        foreach (var binding in gameObjectKeyBindings)
        {
            AddGameObjectKeyBinding(binding.gameObject, binding.key);
        }
    }

    private void OnDisable()
    {
        foreach (var inputAction in inputActions)
        {
            inputAction.Disable();
        }
        inputActions.Clear();
    }

    private void ToggleGameObject(GameObject gameObject)
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.enabled = !canvas.enabled;
        }
        else
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }

    /// <summary>
    /// Add an input action to toggle the visibility of a GameObject or Canvas.
    /// </summary>
    public void AddGameObjectKeyBinding(GameObject gameObject, KeyEnum key)
    {
        var inputAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/" + key.ToString().ToLower());
        inputAction.performed += context => ToggleGameObject(gameObject);
        inputAction.Enable();
        inputActions.Add(inputAction);
    }

    public void RemoveGameObjectKeyBinding(GameObject gameObject, KeyEnum key)
    {
        var inputAction = inputActions.FirstOrDefault(action => action.bindings[0].path == "<Keyboard>/" + key.ToString().ToLower());
        if (inputAction != null)
        {
            inputAction.Disable();
            inputActions.Remove(inputAction);
        }
    }

    public void ClearGameObjectKeyBindings()
    {
        foreach (var inputAction in inputActions)
        {
            inputAction.Disable();
        }
        inputActions.Clear();
    }

    public GameObjectKeyBinding[] GetGameObjectKeyBindings()
    {
        return gameObjectKeyBindings;
    }

    public string[] getUsedKeys()
    {
        return gameObjectKeyBindings.Select(binding => binding.key.ToString()).ToArray();
    }
}

[System.Serializable]
public class GameObjectKeyBinding
{
    public GameObject gameObject;
    public KeyEnum key;
}

public enum KeyEnum
{
    A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
    RightShift, LeftShift, Space, Enter, Escape
}