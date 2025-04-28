using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using System.Collections;// Works on Steamdeck

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public AudioMixer mainMixer;

    [System.Serializable]
    public class Music
    {
        public AudioSource musicAudioSource;
        public AudioSource musicLayerSource;
        [Space(5)]
        [Header("---- Splash ----")]
        public int chosenSplashMusicIndex;
        public AudioClip[] splashMusicArray;
        [Header("---- Menu ----")]
        public int chosenMenuMusicIndex;
        public AudioClip[] menuMusicArray;
    }
    public Music music;    

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

    void Start()
    {
        if (music.menuMusicArray != null && music.menuMusicArray.Length > 0)
        {
            //music.menuMusicArray = music.menuMusicArray.Where(c => c != null).ToArray();// clear array of null entries
            //music.splashMusicArray = music.splashMusicArray.Where(c => c != null).ToArray();// clear array of null entries
            int randomIndex = UnityEngine.Random.Range(0, music.menuMusicArray.Length);
            AudioClip randomClip = music.menuMusicArray[randomIndex];
            if (randomClip != null)
            {
                music.musicAudioSource.resource = randomClip;   
                music.chosenMenuMusicIndex = randomIndex;
                if (music.splashMusicArray[randomIndex] != null)
                {
                    music.musicLayerSource.resource = music.splashMusicArray[randomIndex];
                    music.chosenSplashMusicIndex = randomIndex;
                }
                    return;
            }
        }
    }

    private void Update()
    {
        if (!gameManager.inSplashScreen && music.musicAudioSource.mute)
        {
            music.musicLayerSource.mute = true;
            music.musicAudioSource.mute = false;
        }
    }

    public void OnClickUI()
    {
        if (!gameManager.inSplashScreen)
        {
            uiPitchAudioSource.resource = uiClickSound;
            gameManager.scripts.audioManager.mainMixer.SetFloat(Strings.audioUIPitchShift, Random.Range(0.95f, 1.05f));
            uiPitchAudioSource.Play();
        }
    }
    public void OnClickButton()
    {
        if (!gameManager.inSplashScreen)
        {
            uiAudioSource.resource = uiButtonSound;
            uiAudioSource.Play();
        }
    }
}
