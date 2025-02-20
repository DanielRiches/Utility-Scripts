using UnityEngine;
using UnityEngine.Audio;
using System.Collections;// Works on Steamdeck

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public AudioMixer mainMixer;
    [Header("---- Music ----------------------------------------------------")]
    public AudioSource musicAudioSource;
    [Header("---- Ambient ----------------------------------------------------")]
    public AudioSource ambientAudioSource;
    [Header("---- UI ----------------------------------------------------")]
    public AudioSource uiAudioSource;
    public AudioSource uiPitchAudioSource;
    public AudioClip uiClickSound;
    public AudioClip uiButtonSound;
    [Header("---- Event ----------------------------------------------------")]
    public AudioSource eventAudioSource;
    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.scripts.audioManager = this;
    }

    public void OnClickUI()
    {
        uiPitchAudioSource.resource = uiClickSound;
        gameManager.scripts.audioManager.mainMixer.SetFloat(Strings.audioUIPitchShift, Random.Range(0.95f, 1.05f));
        uiPitchAudioSource.Play();
    }
    public void OnClickButton()
    {
        uiAudioSource.resource = uiButtonSound;
        uiAudioSource.Play();
    }
}
