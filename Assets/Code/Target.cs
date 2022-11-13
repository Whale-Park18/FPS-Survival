using System.Collections;
using UnityEngine;

public class Target : InteractionObject
{
    [SerializeField]
    private AudioClip   clipTargetUp;
    [SerializeField] 
    private AudioClip   clipTargetDown;
    [SerializeField]
    private float       targetUpDelayTime = 3f;

    private AudioSource audioSource;
    private bool        isPossibleHit = true;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void TakeDamage(int damage)
    {
        print("Hit Target");
        currentHP -= damage;

        if(currentHP <= 0 && isPossibleHit)
        {
            isPossibleHit = false;
            StartCoroutine("OnTargetDown");
        }
    }

    private IEnumerator OnTargetDown()
    {
        audioSource.clip = clipTargetDown;
        audioSource.Play();

        yield return StartCoroutine(OnAnimation(0, 90));

        StartCoroutine("OnTargetUp");
    }

    private IEnumerator OnTargetUp()
    {
        yield return new WaitForSeconds(targetUpDelayTime);

        audioSource.clip = clipTargetUp;
        audioSource.Play();

        yield return StartCoroutine(OnAnimation(90, 0));

        isPossibleHit = true;
    }

    private IEnumerator OnAnimation(float start, float end)
    {
        float percent = 0;
        float current = 0;
        float time = 1f;

        /// Tip.
        ///  과녁이 아래 축을 기준으로 회전하도록 빈 오브젝트를 부모로 설정
        while(percent < 1)
        {
            current += Time.deltaTime;
            percent= current / time;

            transform.rotation = Quaternion.Slerp(Quaternion.Euler(start, 0, 0), Quaternion.Euler(end, 0, 0), percent);

            yield return null;
        }
    }
}
