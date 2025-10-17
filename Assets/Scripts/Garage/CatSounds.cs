using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;

public class CatSounds : MonoBehaviour
{
    public string folderPath = "Audio/SFX/cat";
    public float minDelay = 5f;
    public float maxDelay = 10f;
    public bool playOnStart = true;
    public bool loop = true;

    void Awake()
    {
        if (playOnStart)
            StartCoroutine(PlayRandomSounds());
    }

    IEnumerator PlayRandomSounds()
    {
        while (loop)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            PlayRandomClip();
        }
    }

    public void PlayRandomClip()
    {
        // generate a random number from 1 to 7 inclusive 
        int randomNumber = Random.Range(1, 7);
        string soundName = "cat_" + randomNumber.ToString();
        AudioManager.Instance.PlaySFX(soundName);
        Debug.Log("MEOW" + soundName);
    }

}
