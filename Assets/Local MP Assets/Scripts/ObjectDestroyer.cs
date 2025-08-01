using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
/// <summary>
/// This class is used to destroy an object after a certain number of blows.
/// </summary>
public class ObjectDestroyer : MonoBehaviour
{
    private int count = 0;

    [Header("Blow Attributes")]
    [Tooltip("Number of blows needed to destroy the object")]
    [SerializeField] public int numberOfBlows;

    private AudioSource audioSource;
    [Header("Audio Clips")]
    [SerializeField] public AudioClip blowSound;
    [SerializeField] public AudioClip destroySound;

    [Header("Mesh Attributes")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;


    private void Start()
    {
        if (meshRenderer == null || meshCollider == null)
        {
            Debug.LogError("The mesh renderer or mesh collider component is missing");
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("The audio source component is missing");
        }
    }

    public void BlowMadeCallback()
    {
        count++;
        if (count == numberOfBlows)
        {
            DestroyObject();
        }
        else
        {
            audioSource.PlayOneShot(blowSound);
        }
    }

    private void DestroyObject()
    {
        audioSource.PlayOneShot(destroySound);
        // Destroy(this.gameObject);
        meshRenderer.enabled = false;
        meshCollider.enabled = false;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
