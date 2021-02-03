using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource[] audioSources;
    public AudioSource musicSource;


    public AudioClip menuMusic, gameMusic, laser, meteorExplosion, shipExplosion, damage, engine, laserHit, select;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateVolume(float newVolume)
    {
        musicSource.volume = newVolume;
        foreach(AudioSource audio in audioSources)
        {
            audio.volume = newVolume;
        }
    }

    public void Mute(bool mute)
    {
        musicSource.mute = mute;
        foreach (AudioSource audio in audioSources)
        {
            audio.mute = mute;
        }
    }

    public void PlayMusic(AudioID audioID)
    {
        AudioClip clip = GetClipFromID(audioID);

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySelect()
    {
        PlaySound(AudioID.SELECT);
    }

    public void PlaySound(AudioID audioID)
    {
        AudioClip clip = GetClipFromID(audioID);

        foreach(AudioSource audio in audioSources)
        {
            if (!audio.isPlaying || audio.clip == clip)
            {
                audio.clip = clip;
                audio.Play();
                return;
            }
        }
    }

    public AudioClip GetClipFromID(AudioID audioID)
    {
        switch (audioID)
        {
            case AudioID.MENU_MUSIC:
                return menuMusic;
            case AudioID.GAME_MUSIC:
                return gameMusic;
            case AudioID.METEOR_EXPLOSION:
                return meteorExplosion;
            case AudioID.SHIP_EXPLOSION:
                return shipExplosion;
            case AudioID.DAMAGE:
                return damage;
            case AudioID.ENGINE:
                return engine;
            case AudioID.LASER:
                return laser;
            case AudioID.LASER_HIT:
                return laserHit;
            case AudioID.SELECT:
                return select;


            default:
                return menuMusic;
        }
    }
}

public enum AudioID
{
    MENU_MUSIC,
    GAME_MUSIC,
    
    METEOR_EXPLOSION,
    SHIP_EXPLOSION,
    DAMAGE,
    ENGINE,
    LASER,
    LASER_HIT,
    MISSILE,
    MISSILE_EXPLOSION,
    SHIELD_ON,
    SHIELD_OFF,

    SELECT,


}
