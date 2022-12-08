using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.Character.Enemy;
using WhalePark18.Character.Player;
using WhalePark18.Objects;

public class NuclearExplosion : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] explosionAudioClips;        // 폭발 사운드 클립

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
