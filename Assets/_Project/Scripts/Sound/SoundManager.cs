using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundSO soundSO;
    [Range(.1f, .2f)]
    [SerializeField] private float clampPitch = .15f;

    private AudioSource audioSource;
    private float minPitch;
    private float maxPitch;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        minPitch = 1 - clampPitch;
        maxPitch = 1 + clampPitch;
    }

    private void OnEnable()
    {
        UIEvents.OnButtonClicked += HandleButtonClicked;
        GameEvents.OnKnoxsConnected += HandleKnoxsConnected;
        GameEvents.OnLevelCompleted += HandleLevelCompleted;
    }

    private void OnDisable()
    {
        UIEvents.OnButtonClicked -= HandleButtonClicked;
        GameEvents.OnKnoxsConnected -= HandleKnoxsConnected;
        GameEvents.OnLevelCompleted -= HandleLevelCompleted;
    }

    private void HandleButtonClicked() => PlaySound(SoundType.ButtonClicked);
    private void HandleKnoxsConnected() => PlaySound(SoundType.KnoxsConnected);
    private void HandleLevelCompleted(bool isPerfect, int moves) => PlaySound(SoundType.LevelCompleted);

    private void PlaySound(SoundType type)
    {
        AudioClip randomClip = soundSO.GetRandomClip(type);
        if (randomClip == null)
            return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(randomClip);
    }
}
