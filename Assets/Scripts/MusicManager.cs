using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource audioSource;

    private void Start()
    {
        //get the PlayerPrefs float 'musicVolume' with a default value of 1.0f if it doesn't exist
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1.0f);
        if(!PlayerPrefs.HasKey("musicVolume")){
            Debug.Log("No key for musicVolume.");
        }
        //get the AudioSource component attached to this object
        audioSource = GetComponent<AudioSource>();

        //set the volume of the audio source to the 'musicVolume' value
        audioSource.volume = musicVolume;
    }
}