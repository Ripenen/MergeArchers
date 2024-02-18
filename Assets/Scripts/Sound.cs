using UnityEngine;

public class Sound
{
    public static Sounds Sounds;

    private static GameObject _sounds;

    public static AudioSource Play(AudioClip clip, bool loop = false)
    {
        if (!_sounds)
            _sounds = new GameObject("Sounds");

        var source = _sounds.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = 1;
        source.loop = loop;
        source.playOnAwake = false;
        
        source.Play();
        
        if(!loop)
            Object.Destroy(source, clip.length);

        return source;
    }
}