using UnityEngine;

public enum SoundType
{
    ButtonClicked, KnoxsConnected, LevelCompleted
}

[System.Serializable]
public struct SoundData
{
    public AudioClip[] clips;
    public SoundType type;
}

[CreateAssetMenu(fileName = "Sound", menuName = "Scriptable Objects/SoundSO")]
public class SoundSO : ScriptableObject
{
    [SerializeField] private SoundData[] sounds;

    public AudioClip GetRandomClip(SoundType type)
    {
        foreach (var sound in sounds)
        {
            if (sound.clips.Length > 0 && sound.type == type)
            {
                int randomClipsIdx = Random.Range(0, sound.clips.Length);
                return sound.clips[randomClipsIdx];
            }
        }

        return null;
    }
}
