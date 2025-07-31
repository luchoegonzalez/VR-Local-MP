using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

/// <summary>
/// Networked Controller for the password console
/// </summary>
/// <remarks>
/// This class listens to the numpad controller and checks if the password is correct
/// </remarks>
public class PasswordConsole : NetworkBehaviour
{
    [Header("Numpad")]
    [SerializeField] private NumpadController numpadController;

    [Header("Password")]
    [SerializeField] private TMP_Text passwordText;
    [SerializeField] private string password;
    private NetworkVariable<int> m_PasswordLength = new NetworkVariable<int>();
    private NetworkVariable<FixedString32Bytes> m_CurrentPassword = new NetworkVariable<FixedString32Bytes>("");

    [Header("UI")]
    [Tooltip("Color of the password text when the password is correct")]
    [SerializeField] private Color correctColor = Color.green;
    [Tooltip("Color of the password text when the password is wrong")]
    [SerializeField] private Color wrongColor = Color.red;

    [Header("Events")]
    [SerializeField] private UnityEvent onPasswordCorrect;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("Play a sound when the password is correct")]
    [SerializeField] private bool playCorrectSound = true;
    [SerializeField] private AudioClip correctSound;
    [Tooltip("Play a sound when the password is wrong")]
    [SerializeField] private bool playWrongSound = true;
    [SerializeField] private AudioClip wrongSound;

    private void Start()
    {
        if (numpadController == null || passwordText == null)
        {
            Debug.LogError("Numpad controller or password text not set");
            return;
        }

        if (numpadController.NumpadButtons.Length == 0)
        {
            Debug.LogError("No numpad buttons found");
        }

        if (PlayCorrectSoundEnabled && correctSound == null)
        {
            Debug.LogError("Correct sound not set");
        }

        if (PlayWrongSoundEnabled && wrongSound == null)
        {
            Debug.LogError("Wrong sound not set");
        }

        numpadController.OnNumpadButtonPressed += OnNumpadButtonPressedCallback;
        m_PasswordLength.Value = password.Length;

        m_CurrentPassword.OnValueChanged += OnValueChangeCallback;
    }

    private void OnNumpadButtonPressedCallback(char value)
    {
        UpdatePasswordRpc(value);
    }

    // Because the password is a NetworkVariable, we need to update it on the server
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void UpdatePasswordRpc(char value)
    {

        if (m_CurrentPassword.Value.Length == m_PasswordLength.Value)
        {
            HandleFullPassword(value);
            return;
        }

        m_CurrentPassword.Value += value.ToString();
    }

    // This method is called when the password is updated on the server
    private void OnValueChangeCallback(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        passwordText.color = Color.white;
        passwordText.text = newValue.Value;

        if (newValue.Value.Length < m_PasswordLength.Value)
        {
            return;
        }

        if (IsPasswordCorrect())
        {
            HandleCorrectPassword();
        }
        else
        {
            HandleIncorrectPassword();
        }
    }


    private bool IsPasswordCorrect()
    {
        return m_CurrentPassword.Value == password;
    }

    private void HandleCorrectPassword()
    {
        passwordText.color = correctColor;
        if (PlayCorrectSoundEnabled)
        {
            PlayCorrectSound();
        }

        numpadController.DeactivateButtons();
        onPasswordCorrect.Invoke();
    }

    private void HandleIncorrectPassword()
    {
        passwordText.color = wrongColor;
        if (PlayWrongSoundEnabled)
        {
            PlayWrongSound();
        }
    }

    private void HandleFullPassword(char value)
    {
        m_CurrentPassword.Value = value.ToString();
    }

    public override void OnDestroy()
    {
        numpadController.OnNumpadButtonPressed -= OnNumpadButtonPressedCallback;
    }

    public void Reset()
    {
        m_CurrentPassword.Value = "";
        passwordText.text = "";
    }

    public void SetPassword(string newPassword)
    {
        password = newPassword;
        m_PasswordLength.Value = password.Length;
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    public void PlayCorrectSound()
    {
        PlaySound(correctSound);
    }

    public void PlayWrongSound()
    {
        PlaySound(wrongSound);
    }

    public void SetAudioSource(AudioSource newAudioSource)
    {
        audioSource = newAudioSource;
    }

    public string GetPassword => password;
    public bool PlayCorrectSoundEnabled => playCorrectSound;
    public bool PlayWrongSoundEnabled => playWrongSound;
    public AudioSource GetAudioSource => audioSource;
    public AudioClip GetCorrectSound => correctSound;
    public AudioClip GetWrongSound => wrongSound;

}
