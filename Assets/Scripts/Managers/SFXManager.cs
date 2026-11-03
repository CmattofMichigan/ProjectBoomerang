using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
    }

    
    public void PlaySFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //Spawn in object
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length of clip
        float clipLength = audioSource.clip.length;

        //destroy object after clip length
        Destroy(audioSource.gameObject, clipLength);
    }

}
