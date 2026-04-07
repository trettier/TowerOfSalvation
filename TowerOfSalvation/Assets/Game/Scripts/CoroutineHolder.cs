using System;
using System.Collections;
using UnityEngine;

public class CoroutineHolder : Singleton<CoroutineHolder>
{
    public IEnumerator AfterSeconds(Single seconds, Action action)
    {
        if (action == null)
            return null;

        var coroutine = AfterSecondsCoroutine(seconds, action);
        StartCoroutine(coroutine);
        return coroutine;
    }

    private static IEnumerator AfterSecondsCoroutine(Single seconds, Action action)
    {
        if (seconds > 0f)
        {
            yield return new WaitForSeconds(seconds);
        }
        action?.Invoke();
    }

    private static IEnumerator AfterRealSecondsCoroutine(Single seconds, Action action)
    {
        if (seconds > 0f)
        {
            yield return new WaitForSecondsRealtime(seconds);
        }
        action?.Invoke();
    }
    private static IEnumerator EndOfFrameCoroutine(Action action)
    {
        yield return new WaitForEndOfFrame();
        action?.Invoke();
    }

    private static IEnumerator ConditionalAwaitCoroutine(Func<Boolean> awaitCondition, Action action)
    {
        while (awaitCondition())
        {
            yield return null;
        }
        action?.Invoke();
    }
}