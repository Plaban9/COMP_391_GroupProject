namespace Effects.Audio
{
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class AudioFade : MonoBehaviour
    {
        private static bool isFadingIn;

        public static IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
        {
            //while (isFadingIn)
            //{
            //    yield return new WaitForSeconds(0.5f);
            //}

            fadeTime = fadeTime + 2f;

            double lengthOfSource = (double)audioSource.clip.samples / audioSource.clip.frequency;
            yield return new WaitForSecondsRealtime((float)(lengthOfSource - fadeTime));

            float startVolume = audioSource.volume;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = startVolume;
        }

        public static IEnumerator FadeIn(AudioSource audioSource, float fadeTime, float maxVolume)
        {
            isFadingIn = true;
            float startVolume = 0.1f;

            audioSource.volume = 0;
            audioSource.Play();

            while (audioSource.volume < maxVolume)
            {
                audioSource.volume += startVolume * Time.deltaTime / fadeTime;

                yield return null;
            }

            audioSource.volume = maxVolume;
            isFadingIn = false;
        }
    }
}
