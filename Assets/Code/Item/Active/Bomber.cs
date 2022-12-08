using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Apple;

[RequireComponent(typeof(AudioSource))]
public class Bomber : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;           // ÀÌµ¿ ¼Óµµ;

    [SerializeField]
    private AudioClip[] engineClips;    // Á¦Æ® ¿£Áø Å¬¸³

    [SerializeField]
    private GameObject bomb;            // ÆøÅº
    private Transform target;           // Å¸°Ù

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator OnBomb()
    {
        audioSource.clip = engineClips[Random.Range(0, engineClips.Length)];
        audioSource.loop = true;
        audioSource.Play();

        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y;
        yield return StartCoroutine("Move", targetPosition);
        DropBomb();

        yield return StartCoroutine("Move", transform.forward * 300f);
        Destroy(gameObject);
    }

    private IEnumerator Move(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float destinationDistance = Vector3.Distance(startPosition, targetPosition);
        float currentMoveDistance = 0f;
        float percent = 0f;

        while(true)
        {
            currentMoveDistance += speed * Time.deltaTime;
            percent = currentMoveDistance / destinationDistance;
            transform.position = Vector3.Lerp(startPosition, targetPosition, percent);

            if(percent >= 1f)
            {
                yield break;
            }

            yield return null;
        }
    }

    private void DropBomb()
    {
        bomb.transform.parent = null;
        bomb.GetComponent<Bomb>().Use();
    }

    private void LookAt(Vector3 target)
    {
        Vector3 relative = target - transform.position;
        float radian = Mathf.Atan2(relative.x, relative.z);
        float degree = radian * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, degree, transform.eulerAngles.z);
    }

    public void Fly(Transform target)
    {
        this.target = target;
        LookAt(target.position);
        StartCoroutine("OnBomb");
    }
}
