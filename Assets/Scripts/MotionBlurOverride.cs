using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MotionBlurOverride : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [Range(0f, 5f)]
    [SerializeField] private float intensity = 2.0f;

    private void Start()
    {
        if (volume.profile.TryGet<MotionBlur>(out var blur))
        {
            blur.intensity.Override(intensity);
        }
    }
}
