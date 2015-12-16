using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Custom/Color By Time")]
    public class ColorEffects : ImageEffectBase
    {
        public Texture textureRamp;
        public float rampOffset;
        public float amount;

        // Called by camera to apply image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            material.SetTexture("_RampTex", textureRamp);
            material.SetFloat("_RampOffset", rampOffset);
            material.SetFloat("_Amount", Mathf.Min(1,Mathf.Max(0,amount)));
            Graphics.Blit(source, destination, material);
        }
    }
}