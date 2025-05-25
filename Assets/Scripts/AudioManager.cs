using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField] AudioClipsSO audioClips;
    void Start()
    {
        Player.Instance.OnPlayerStartRoll += Player_OnPlayerStartRoll;
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
        LevelGrid.Instance.OnGridDestroy += LevelGrid_OnGridDestroy;
        LevelGrid.Instance.OnGridPreDestroy += LevelGrid_OnGridPreDestroy;
        CollectableManager.Instance.OnAllCollected += CollectableManager_OnAllCollected;
        CollectableManager.Instance.OnCollected += CollectableManager_OnCollected;

    }

    private void CollectableManager_OnCollected(object sender, EventArgs e)
    {
        PlaySound(audioClips.collectSounds, Camera.main.transform.position, audioClips.collectVolume);
    }

    private void CollectableManager_OnAllCollected(object sender, EventArgs e)
    {
        PlaySound(audioClips.victorySound, Camera.main.transform.position, audioClips.victoryVolume);
    }

    private void LevelGrid_OnGridPreDestroy(object sender, GridPosition e)
    {
        PlaySound(audioClips.gridPreDestroySound, Camera.main.transform.position, audioClips.gridPreDestroyVolume);

    }

    private void LevelGrid_OnGridDestroy(object sender, GridPosition e)
    {
        PlaySound(audioClips.gridDestroySound, Camera.main.transform.position, audioClips.gridDestroyVolume);

    }

    private void Player_OnPlayerDeath(object sender, EventArgs e)
    {
        PlaySound(audioClips.playerDeathSound, Camera.main.transform.position, audioClips.playerDeathVolume);
    }

    void Player_OnPlayerStartRoll(object sender, EventArgs e)
    {
        PlaySound(audioClips.playerRollSound, Camera.main.transform.position, audioClips.playerRollVolume);
    }

    void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], position, volume);
    }

    void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
}
