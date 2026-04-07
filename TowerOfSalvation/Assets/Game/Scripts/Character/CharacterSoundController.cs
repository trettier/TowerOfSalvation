using UnityEngine;
using System.Collections;

public class CharacterSoundController : MonoBehaviour
{
    public AudioSource step;
    public AudioSource hit;
    public AudioSource kick;
    public AudioSource dead;

    [SerializeField] private float stepPitchMin = 1.23f;
    [SerializeField] private float stepPitchMax = 1.4f;
    [SerializeField] private float stepInterval = 0.1f;

    private Coroutine stepCoroutine;

    public void StepStart()
    {
        if (step.isPlaying)
            return;

        step.loop = true;
        step.Play();
        stepCoroutine = StartCoroutine(ChangeStepPitch());
    }

    public void StepStop()
    {
        if (stepCoroutine != null)
            StopCoroutine(stepCoroutine);

        step.Stop();
    }

    private IEnumerator ChangeStepPitch()
    {
        while (true)
        {
            step.pitch = Random.Range(stepPitchMin, stepPitchMax);
            yield return new WaitForSeconds(stepInterval);
        }
    }

    public void Kick() => PlayOneShot(kick);
    public void Hit() => PlayOneShot(hit);
    public void Dead() => PlayOneShot(dead);

    private void PlayOneShot(AudioSource sound)
    {
        sound.pitch = Random.Range(0.9f, 1.1f);
        sound.PlayOneShot(sound.clip);
    }
}
