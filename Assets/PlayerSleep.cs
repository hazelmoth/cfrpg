using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSleep : MonoBehaviour
{
    private const float WakeTime = 0.25f;
    private const float ScreenFadeDuration = 0.25f;
    private const float BlackScreenDuration = 0.5f;

    public static void SleepToMorning ()
    {
        GlobalCoroutineObject.Instance.StartCoroutine(SleepCoroutine(WakeTime));
    }
    private static IEnumerator SleepCoroutine (float wakeTime)
    {
        float start = Time.time;

        ScreenFadeAnimator.FadeOut(ScreenFadeDuration);
        while (Time.time - start < ScreenFadeDuration)
        {
            yield return null;
        }

        TimeKeeper.SetTime(WakeTime);
        TimeKeeper.IncrementDay();

        while (Time.time - start < ScreenFadeDuration + BlackScreenDuration)
        {
            yield return null;
        }


        ScreenFadeAnimator.FadeIn(ScreenFadeDuration);
    }
}
