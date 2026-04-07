using UnityEngine;
using System.Collections;

public class ItemSoundController : MonoBehaviour
{
    public AudioSource hit;

    [SerializeField] private float pitchMin = 0.8f;
    [SerializeField] private float pitchMax = 1.2f;


    private Coroutine stepCoroutine;

    public void StepStop()
    {
        if (stepCoroutine != null)
            StopCoroutine(stepCoroutine);

    }

    public void Hit() => PlayOneShot(hit);

    private void PlayOneShot(AudioSource sound)
    {
        sound.pitch = Random.Range(pitchMin, pitchMax);
        sound.PlayOneShot(sound.clip);
    }
}
