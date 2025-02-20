using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public GameObject mainCameraGameobject;// ACCESSED BY GAME MANAGER
    public Camera mainCamera;
    public HDAdditionalCameraData cameraData;
    public CinemachineCamera playerCinemachineCamera;
    
    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.scripts.cameraManager = this;
        mainCamera = Camera.main;
        cameraData = mainCamera.GetComponent<HDAdditionalCameraData>();
        mainCamera.allowMSAA = true;
        mainCamera.allowHDR = true;
        mainCamera.useOcclusionCulling = true;
    }

    
    void Update()
    {
        
    }
}
