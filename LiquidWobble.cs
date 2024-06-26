using UnityEngine;
using UnityEngine.VFX;

public class LiquidWobble : MonoBehaviour
{
    VisualEffect visualEffect;
    Transform thisTransform;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;

    // Use this for initialization
    void Start()
    {
        thisTransform = this.transform;
        visualEffect = thisTransform.GetComponent<VisualEffect>();
    }
    private void Update()
    {
        if (visualEffect.GetBool(Strings.marineGasMaskBackpack))
        {
            time += Time.deltaTime;
            // decrease wobble over time
            wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
            wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

            // make a sine wave of the decreasing wobble
            pulse = 2 * Mathf.PI * WobbleSpeed;
            wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
            wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

            // send it to the shader
            visualEffect.SetFloat(Strings.liquidWobbleX, wobbleAmountX);
            visualEffect.SetFloat(Strings.liquidWobbleZ, wobbleAmountZ);

            // velocity
            velocity = (lastPos - transform.position) / Time.deltaTime;
            angularVelocity = thisTransform.rotation.eulerAngles - lastRot;


            // add clamped velocity to wobble
            wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
            wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

            // keep last position
            lastPos = thisTransform.position;
            lastRot = thisTransform.rotation.eulerAngles;
        }
    }
}
