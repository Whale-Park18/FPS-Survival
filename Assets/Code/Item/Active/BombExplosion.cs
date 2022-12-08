using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] explosionAudioClips;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource= GetComponent<AudioSource>();
    }

    private void Start()
    {
        int index = Random.Range(0, explosionAudioClips.Length);
        audioSource.clip = explosionAudioClips[index];
        audioSource.Play();
    }
}
