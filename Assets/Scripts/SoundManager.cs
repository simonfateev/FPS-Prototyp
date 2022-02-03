using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager { 

    public enum Sound
    {
        footstep1,
        footstep2,
        footstep3,
        footstep4,
        footstep5,
        footstep6,
        ammopickup,
        enemyhit,
        noammo,
        bulletimpact,
        playerfall,
        playerhit,
        playerlongfall,
        weaponphysics
    }

    private static Dictionary<Sound, float> soundTimerDictionary;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.footstep1] = 0f;
        soundTimerDictionary[Sound.noammo] = 0f;
    }

    public static void PlaySound(Sound sound, Vector3 position)
    {
        if (CanPlaySound(sound))
        {
            AudioSource.PlayClipAtPoint(GetAudioClip(sound), position);
        }
    }

    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                return true;
            case Sound.footstep1:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 0.5f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    } 
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            case Sound.noammo:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float noAmmoSoundMax = 0.15f;
                    if (lastTimePlayed + noAmmoSoundMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
        }

    }
    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.i.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + " not found");
        return null;

    }
}

