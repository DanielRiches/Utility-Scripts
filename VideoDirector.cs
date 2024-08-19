using System.Linq;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(AudioSource))]
public class VideoDirector : MonoBehaviour
{
    // 1. Create an empty container and drag this script onto it, this script will add the required components and set appropriate properties.
    // 2. Create as many 3D objects as you like and attach a material to each object. 
    // 3. Set each 3D object with a tag or Layer. The Tag name can be changed below. You can then choose what layer / tag is searched for.
    // 3. Right click your project window and create a new Render Texture, assign it to the appropriate field in this script.
    //     -- Dont forget to specify your desired size in the Inspector Settings for the RenderTexture (EG: 1920x1080).
    // 4. Add your videos to the videos array.
    // 5. Add your audio to the audio array.
    //     -- Make sure your names for associated audio are IDENTICAL. There is no need to exactly match the order in the arrays.
    //     -- If your intended video already have audio built in this script will automatically use that.
    // 6. Enter Play Mode and select the play bool on this script.

    private VideoPlayer videoPlayer;
    private Renderer objectenderer;
    private VideoClip previousVideo;
    private AudioSource audioSource;
    private float syncCheck;
    private float previousAudioTime = 0f;
    private long previousFrame = -1;
    private bool started;
    private bool ready;
    private bool paused;
    private int previousVideoNumber;
    private bool matchFound = false;
    private bool hasNonNullElements = false;
    private bool audioHasNonNullElements = false;

    public bool detectEventObjects;
    [SerializeField] private GameObject[] eventObjects;
    [Space(10)]
    [Tooltip("Set to Nothing if using Tags.")][SerializeField] private LayerMask desiredLayer;
    public string desiredTag;
    [Header("--------------------------------------------------------------------------------------")]
    public RenderTexture renderTexture;
    [Header("--------------------------------------------------------------------------------------")]
    [Tooltip("Video to play from the array.")] public int desiredVideoNumber;
    [Tooltip("Leave blank if you wish to use numbers.")] public string desiredVideoName;
    [Space(20)]
    public bool play;
    public bool pause;
    public bool stop;
    public bool skip;
    public bool back;
    public bool restart;
    public bool loop;
    [Space(10)]
    [Tooltip("will play videos in order.")] public bool continuousPlay;
    [Tooltip("will play a random video.")] public bool randomize;
    [Tooltip("Synchronize Video and Audio.")] public bool synchronizeVideo = true;
    [Tooltip("Lower frequency will increase stutter.")][Range(0, 10)] public float synchronizationFrequency = 3f;
    [Space(20)]
    [Tooltip("Optional.")][SerializeField] private Texture2D standByTexture;
    [Tooltip("Optional.")][SerializeField] private VideoClip standByVideo;
    [SerializeField] private VideoClip[] video;
    [Header("--------------------------------------------------------------------------------------")]
    public bool muteAudio;
    [Range(0, 1)] public float audioVolume = 1f;
    [Tooltip("Note that a sound with a larger priority will more likely be stolen by sounds with smaller priorities.")][Range(0, 256)] public int audioPriority = 128;
    [Tooltip("Default = 0 , Left = -1 , Right = 1 (Only valid for Stereo and Mono audio).")][Range(-1, 1)] public float stereoPan = 0f;
    [Tooltip("2D = 0 , 3D = 1 (Only works with seperate audio).")][Range(0, 1)] public float audio3DSpacialBlend;
    [Range(0, 1.1f)] public float reverbZoneMix = 1f;
    [Tooltip("Tempo.")][Range(0, 3)] public float audioPitch = 1f;
    [Space(20)]
    [SerializeField] private AudioClip[] videoAudio;

    private void Awake()
    {
        if (TryGetComponent<AudioSource>(out audioSource))
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = audio3DSpacialBlend;
        }
        if (TryGetComponent<VideoPlayer>(out videoPlayer))
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.skipOnDrop = true;
            videoPlayer.timeUpdateMode = VideoTimeUpdateMode.GameTime;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }

        if (renderTexture)
        {
            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.black); // Ensure it's cleared to black
            RenderTexture.active = null;
            videoPlayer.targetTexture = renderTexture;
        }

        FindEvents();
    }

    void Update()
    {
        if (detectEventObjects)
        {
            detectEventObjects = false;
            FindEvents();
        }

        if (!ready)
        {
            return;
        }

        Synchronize();
        Options();
        PauseVideo();

        if (play)
        {
            if (play)
            {
                started = true;
            }
            if (pause)
            {
                pause = false;
                paused = false;
            }
            PlayVideo();
        }
        if (videoPlayer.isPlaying)
        {
            if (restart || skip || stop || back)
            {
                PlayVideo();
            }
        }

    }

    private void PleaseStandBy()
    {
        if (standByVideo)
        {
            videoPlayer.clip = standByVideo;
            videoPlayer.frame = 0;
            videoPlayer.time = 0f;

            videoPlayer.Play();
        }
        else
        {
            RenderTexture.active = renderTexture;
            if (standByTexture != null)
            {
                Graphics.Blit(standByTexture, renderTexture);
            }
            else
            {
                GL.Clear(true, true, Color.black); // Ensure it's cleared to black
            }
            RenderTexture.active = null; // Reset the active render target
        }
    }

    private void PlayVideo()
    {
        audioHasNonNullElements = false;
        hasNonNullElements = false;

        if (video != null)
        {
            if (stop)
            {
                videoPlayer.Stop();

                if (audioSource.clip)
                {
                    audioSource.Stop();
                }
                skip = false;
                restart = false;
                pause = false;
                back = false;
                stop = false;
                play = false;
                PleaseStandBy();
                return;
            }
            else if (play)
            {
                if (video[desiredVideoNumber] != null)
                {                    
                    Setclip();
                }
                else
                {
                    Randomize();
                    Debug.Log("No video found in array number, attempting random selection.");
                }
                play = false;
            }
            else if (skip || back)
            {
                if (randomize)
                {
                    Randomize();
                }
                else
                {
                    if (skip)
                    {
                        if (desiredVideoNumber == video.Length - 1)
                        {
                            desiredVideoNumber = 0;
                        }
                        else
                        {
                            desiredVideoNumber++;
                        }

                        if (video[desiredVideoNumber] != null)
                        {
                            Setclip();
                            skip = false;
                        }
                        else
                        {
                            skip = false;
                            Randomize();
                            Debug.Log("No video found in array number, attempting random selection.");
                        }
                        back = false;
                    }
                    else
                    {
                        if (desiredVideoNumber == 0)
                        {
                            desiredVideoNumber = video.Length - 1;
                            Setclip();
                        }
                        else
                        {
                            desiredVideoNumber--;
                            Setclip();
                        }
                        back = false;
                    }
                }
                restart = false;
            }
            else if (loop && !skip || restart)
            {
                Setclip();
                restart = false;
            }
            else if (randomize)
            {
                Randomize();
            }
            else if (!string.IsNullOrEmpty(desiredVideoName))
            {
                for (int i = 0; i < video.Length; i++) // Check if the array contains any non-null elements
                {
                    VideoClip videoClip = video[i];
                    if (videoClip)
                    {
                        if (videoClip.name == desiredVideoName)
                        {
                            desiredVideoNumber = i;
                            previousVideoNumber = i;
                            matchFound = true;
                            break;  // Exit the loop early since we've found a match
                        }
                    }
                }

                if (!matchFound)
                {
                    Randomize();
                    Debug.Log("No clip found matching name, attempting random selection.");
                }
                else
                {
                    Setclip();
                }
                desiredVideoName = null;
            }
            else
            {
                Setclip();
            }            
        }

        DetectAudio();

        if (videoPlayer.clip && audioSource.clip)
        {
            videoPlayer.Play();
            audioSource.Play();
        }
        else if (videoPlayer.clip)
        {
            videoPlayer.Play();
        }

        eventObjects = null;
    }

    private void Setclip()
    {
        videoPlayer.clip = video[desiredVideoNumber];
        previousVideoNumber = desiredVideoNumber;
        videoPlayer.frame = 0;
        videoPlayer.time = 0f;
    }

    private void DetectAudio()
    {
        audioSource.clip = null;
        for (int i = 0; i < videoAudio.Length; i++) // Check if the array contains any non-null elements
        {
            if (videoAudio[i] != null)
            {
                audioHasNonNullElements = true;
                break;
            }
        }
        if (audioHasNonNullElements)
        {      
            if (videoPlayer.audioTrackCount == 0)
            {
                videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;                
                foreach (AudioClip audioClip in videoAudio)
                {
                    if (audioClip)
                    {
                        if (audioClip.name == videoPlayer.clip.name)
                        {
                            audioSource.clip = audioClip;
                            break;  // Exit the loop early since we've found a match
                        }
                    }
                }
            }
            else
            {
                videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;                
            }
            audioSource.time = 0f;
        }
    }

    private void PauseVideo()
    {
        if (videoPlayer.clip)
        {
            if (!pause && paused)
            {
                paused = false;
                videoPlayer.Play();
                if (audioSource.clip)
                {
                    audioSource.Play();
                }
            }
            else if (pause && !paused)
            {
                paused = true;
                videoPlayer.Pause();
                if (audioSource.clip)
                {
                    audioSource.Pause();
                }
            }
        }
    }

    private void Options()
    {
        if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
        {
            videoPlayer.SetDirectAudioVolume(0, audioVolume);
        }        
        audioSource.volume = audioVolume;

        audioSource.spatialBlend = audio3DSpacialBlend;
        audioSource.priority = audioPriority;
        audioSource.panStereo = stereoPan;
        audioSource.reverbZoneMix = reverbZoneMix;

        if (audioPitch > 0f)
        {
            audioSource.pitch = audioPitch;
        }
        else if (audioPitch <= 0f)
        {
            audioSource.pitch = 0f;
        }

        if (muteAudio)
        {
            audioSource.mute = true;
        }
        else
        {
            audioSource.mute = false;
        }
    }

    private void FindEvents()
    {
        if (desiredLayer == (LayerMask)0)
        {
            Debug.Log("No Layer selected, attempting to use Tag");
            eventObjects = GameObject.FindGameObjectsWithTag(desiredTag);
        }
        else
        {
            Debug.Log("Layer specified, using LayerMask");
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            eventObjects = allObjects.Where(go => (desiredLayer.value & (1 << go.layer)) != 0).ToArray(); // Filter to include only the ones that match the LayerMask

            allObjects = null;
        }

        if (eventObjects.Length == 0)
        {
            Debug.Log("No objects found.");
            System.GC.Collect();
            return;
        }

        foreach (GameObject eventItem in eventObjects)
        {
            if (eventItem)
            {
                if (eventItem.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    renderer.material.mainTexture = renderTexture;
                }
            }
        }

        System.GC.Collect();

        if (renderTexture)
        {
            PleaseStandBy();
        }

        if (!videoPlayer || !audioSource || !renderTexture || eventObjects == null)
        {
            ready = false;
            Debug.Log("Audio Director Error, please make sure you have the following: RenderTexture assigned, AudioSource / VideoPlayer components, Objects with Event tag, video and audio names must match");
        }
        else
        {
            ready = true;
        }
    }

    private void Randomize()
    {
        desiredVideoNumber = Random.Range(0, video.Length);

        for (int i = 0; i < video.Length; i++) // Check if the array contains any non-null elements
        {
            if (video[i] != null)
            {
                hasNonNullElements = true;
                break;
            }
        }
        if (hasNonNullElements)
        {
            while (desiredVideoNumber == previousVideoNumber || video[desiredVideoNumber] == null)
            {
                desiredVideoNumber = Random.Range(0, video.Length);
            }

            videoPlayer.clip = video[desiredVideoNumber];
            previousVideoNumber = desiredVideoNumber;
            videoPlayer.frame = 0;
            videoPlayer.time = 0f;
        }
        else
        {
            Debug.Log("All elements in the video array are null. Cannot select a valid video clip.");
        }
    }

    private void Synchronize()
    {
        if (!videoPlayer.GetTargetAudioSource(0))
        {
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }

        if (videoPlayer.clip && videoPlayer.frame >= (long)videoPlayer.frameCount - 1f)
        {
            if (videoPlayer.clip == standByVideo)
            {
                videoPlayer.Stop();
                videoPlayer.frame = 0;
                videoPlayer.time = 0f;
                videoPlayer.Play();
            }
            else
            {
                if (!continuousPlay || !started)
                {
                    PleaseStandBy();
                }
                else
                {
                    videoPlayer.Stop();
                    videoPlayer.frame = 0;
                    videoPlayer.time = 0f;
                    videoPlayer.clip = null;

                    if (continuousPlay && randomize)
                    {
                        Randomize();
                    }
                    else if (loop)
                    {
                        PlayVideo();
                    }
                    else if (continuousPlay)
                    {                        
                        if (desiredVideoNumber == video.Length - 1)
                        {
                            desiredVideoNumber = 0;
                        }
                        else
                        {
                            desiredVideoNumber++;
                        }
                        Setclip();
                        PlayVideo();
                    }
                }
            }
        }
        if (audioSource.clip && audioSource.time >= audioSource.clip.length)
        {
            audioSource.Stop();
            audioSource.time = 0f;
            audioSource.clip = null;
        }

        if (videoPlayer.isPlaying && audioSource.clip)
        {
            syncCheck += Time.deltaTime;

            if (synchronizeVideo && syncCheck > synchronizationFrequency)
            {
                float audioTime = audioSource.time;
                long targetFrame = (long)((audioTime / audioSource.clip.length) * videoPlayer.frameCount);

                if (Mathf.Abs(audioTime - previousAudioTime) > 0.1f || targetFrame != previousFrame)
                {
                    videoPlayer.Pause();

                    RenderTexture.active = renderTexture;
                    GL.Clear(true, true, Color.black); // Ensure it's cleared to black
                    RenderTexture.active = null;

                    videoPlayer.frame = targetFrame;
                    videoPlayer.Play();

                    previousAudioTime = audioTime;
                    previousFrame = targetFrame;
                }
                syncCheck = 0f;
            }

            videoPlayer.playbackSpeed = Mathf.Abs(audioSource.pitch);
        }
        else
        {
            syncCheck = 0f;
        }
    }
}
