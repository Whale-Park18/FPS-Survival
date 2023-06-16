using UnityEngine;
using WhalePark18.Manager;

namespace WhalePark18.Item.Active
{
    public class BombExplosion : MonoBehaviour
    {
        [SerializeField]
        private AudioClip[] explosionAudioClips;

        private AudioData audioData;

        private void Awake()
        {
            audioData = new AudioData(GetComponent<AudioSource>(), Manager.AudioType.Item);
        }

        private void Start()
        {
            SoundManager.Instance.AddAudioSource(audioData);

            int index = Random.Range(0, explosionAudioClips.Length);
            audioData.audioSource.clip = explosionAudioClips[index];
            audioData.audioSource.Play();
        }
    }
}