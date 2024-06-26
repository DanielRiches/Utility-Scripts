using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Jobs;

public class Utils
{
    #if ENABLE_LEGACY_INPUT_MANAGER
    static bool legacyInputSystem = true;
    #else
    static bool legacyInputSystem = false;
    #endif

    #if ENABLE_INPUT_SYSTEM
    static bool newInputSystem = true;
    #else
    static bool newInputSystem = false;
    #endif
    
    public static Vector3 GetMouseWorldPosition3D()
    {
        Ray ray = MouseCursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorWorldPosition, Mathf.Infinity))
        {
            return cursorWorldPosition.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public static Vector3 RayCastIn3DLayerSpecific(LayerMask desiredLayer)
    {
        Ray ray = MouseCursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorRayCastHit, Mathf.Infinity, desiredLayer))
        {
            return cursorRayCastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public static Vector3 RayCastIn3DTagSpecific(string tag)
    {
        Ray ray = MouseCursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorRayCastHit, Mathf.Infinity) && cursorRayCastHit.transform.CompareTag(tag))
        {
            return cursorRayCastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public static Vector3 RayCastIn3DLayerAndTagSpecific(LayerMask desiredLayer, string tag)
    {
        Ray ray = MouseCursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorRayCastHit, Mathf.Infinity, desiredLayer) && cursorRayCastHit.transform.CompareTag(tag))
        {
            return cursorRayCastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public static Transform GetMouseCursorObjectTransform()
    {
        Ray ray = MouseCursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorObjectTransform, Mathf.Infinity))
        {
            return cursorObjectTransform.transform;
        }
        else
        {
            return null;
        }
    }

    public static GameObject GetMouseCursorGameObject()
    {
        Ray ray = MouseCursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorObject, Mathf.Infinity))
        {
            return cursorObject.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    public const string defaultLayer = "Default";
    public static GameObject RayCastFirstObjectHitNotDefaultLayer()
    {
        var layerMask = Physics.DefaultRaycastLayers & ~LayerMask.GetMask(defaultLayer);

        Ray ray = MouseCursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorObject, Mathf.Infinity, layerMask))
        {
            return cursorObject.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    public static GameObject RayCastFirstObjectHit()
    {        
        var layerMask = Physics.DefaultRaycastLayers;

        Ray ray = MouseCursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorObject, Mathf.Infinity, layerMask))
        {
            return cursorObject.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    public static void DontDestroyObjectOnLoad(GameObject gameObject)
    {
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
    }

    public static void ClearMemory()
    {
        var memoryCleanupJob = new MemoryCleanupJob();
        JobHandle memoryCleanupHandle = memoryCleanupJob.Schedule();
    }

    public struct MemoryCleanupJob : IJob
    {
        public void Execute()
        {
            System.GC.Collect();
        }
    }

    // -----------------------------------------------------
    public static Ray MouseCursorRay3D(Camera camera = null)
    {
        if (!camera)
        {
            camera = Camera.main;
        }

        if (newInputSystem && legacyInputSystem || newInputSystem && !legacyInputSystem)
        {
            return camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        }
        else
        {
            return camera.ScreenPointToRay(Input.mousePosition);
        }        
    }
}
