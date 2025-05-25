using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipsSO", menuName = "Scriptable Objects/AudioClipsSO")]
public class AudioClipsSO : ScriptableObject {
    [Header("Player")]
    public AudioClip playerRollSound;
    [Range(0f, 1f)] public float playerRollVolume = 1f;
    [Space]
    public AudioClip playerDeathSound;
    [Range(0f, 1f)] public float playerDeathVolume = 1f;

    [Header("Grid")]
    public AudioClip gridPreDestroySound;
    [Range(0f, 1f)] public float gridPreDestroyVolume = 1f;
    [Space]
    public AudioClip gridDestroySound;
    [Range(0f, 1f)] public float gridDestroyVolume = 1f;

    [Header("Collectables")]
    public AudioClip[] collectSounds;
    [Range(0f, 1f)] public float collectVolume = 1f;

    [Header("Game State")]
    public AudioClip gameOverSound;
    [Range(0f, 1f)] public float gameOverVolume = 1f;
    [Space]
    public AudioClip gameStartSound;
    [Range(0f, 1f)] public float gameStartVolume = 1f;
    [Space]
    public AudioClip victorySound;
    [Range(0f, 1f)] public float victoryVolume = 1f;



}
