using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControllerScript : MonoBehaviour
{
    // Editor Fields
    public float Zoom          = 1;
    public float ZoomMax       = 5;
    public float HeightMin     = 5;
    public float HeightMax     = 25;
    public float ScalingFactor = 2;

    private Camera Camera;

    // the smoothing function between the height boundaries supplied to it
    private float SmoothingCurve(float z)
    {
#if UNITY_EDITOR
        if (ZoomMax < 0)
            Debug.LogError($"Parameter ZoomMax must be a positive decimal - got {ZoomMax}");
        if (HeightMax < 0)
            Debug.LogError($"Parameter HeightMax must be a positive decimal - got {HeightMax}");
        if (HeightMin < 0)
            Debug.LogError($"Parameter HeightMin must be a positive decimal - got {HeightMin}");
        if (HeightMin > HeightMax)
            Debug.LogError($"Parameter HeightMin is greater than HeightMax - {HeightMin} > {HeightMax}");
#endif

        float D = (HeightMax - HeightMin) / math.pow(ZoomMax, math.E);
        float C = math.pow(z, math.E) * D + HeightMin;
        return math.clamp(C, HeightMin, HeightMax);
    }

    void Start()
    {
        Camera = GetComponent<Camera>();
        if (Camera == null)
            Debug.LogError($"{GetType()} must be attached to a Camera");
    }

    void Update()
    {
        if (Mouse.current.rightButton.IsPressed())
        {
            var delta = -Mouse.current.delta.ReadValue() * (Time.deltaTime * Zoom / 2);
            transform.position += new Vector3(delta.x, 0, delta.y);
        }

        var movementVector = Vector2.zero;
        if (Keyboard.current.wKey.IsPressed())
            movementVector += new Vector2(0, 5);
        if (Keyboard.current.sKey.IsPressed())
            movementVector += new Vector2(0, -5);
        if (Keyboard.current.aKey.IsPressed())
            movementVector += new Vector2(-5, 0);
        if (Keyboard.current.dKey.IsPressed())
            movementVector += new Vector2(5, 0);
        movementVector = movementVector.normalized * (Time.deltaTime * Zoom * ScalingFactor);
        transform.position += new Vector3(movementVector.x, 0, movementVector.y);

        // Zooming
        var scroll = -Mouse.current.scroll.y.ReadValue();
        Zoom += scroll * Time.deltaTime * ScalingFactor;
        Zoom = math.clamp(Zoom, 0, ZoomMax);
        var newHeight = SmoothingCurve(Zoom);
        transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
    }
}
