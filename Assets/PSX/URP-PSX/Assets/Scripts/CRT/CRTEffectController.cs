using System;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CRTEffectController : MonoBehaviour
{
    [SerializeField] protected VolumeProfile volumeProfile;
    [SerializeField] protected bool isEnabled = true;

    protected Crt crt;

    [SerializeField] protected float screenBendX = 1000.0f;
    [SerializeField] protected float screenBendY = 1000.0f;
    [SerializeField] protected float vignetteAmount = 0.0f;
    [SerializeField] protected float vignetteSize = 0.0f;
    [SerializeField] protected float vignetteRounding = 0.0f;
    [SerializeField] protected float vignetteSmoothing = 0.0f;
    
    [SerializeField] protected float scanLinesDensity = 200.0f;
    [SerializeField] protected float scanLinesSpeed = -10.0f;
    [SerializeField] protected float noiseAmount = 250.0f;
    
    [SerializeField] protected Vector2 chromaticRed = new Vector2();
    [SerializeField] protected Vector2 chromaticGreen = new Vector2();
    [SerializeField] protected Vector2 chromaticBlue = new Vector2();
    
    protected void Update()
    {
        this.SetParams();
    }

    protected void SetParams()
    {
        if (!this.isEnabled) return; 
        if (this.volumeProfile == null) return;
        if (this.crt == null) volumeProfile.TryGet<Crt>(out this.crt);
        if (this.crt == null) return;
        
        this.crt.screenBendX.value = this.screenBendX;
        this.crt.screenBendY.value = this.screenBendY;
        this.crt.vignetteAmount.value = this.vignetteAmount;
        this.crt.vignetteSize.value = this.vignetteSize;
        this.crt.vignetteRounding.value = this.vignetteRounding;
        this.crt.vignetteSmoothing.value = this.vignetteSmoothing;

        this.crt.scanlinesDensity.value = this.scanLinesDensity;
        this.crt.scanlinesSpeed.value = this.scanLinesDensity;
        this.crt.noiseAmount.value = this.noiseAmount;

        this.crt.chromaticRed.value = this.chromaticRed;
        this.crt.chromaticGreen.value = this.chromaticGreen;
        this.crt.chromaticBlue.value = this.chromaticBlue;

    }
}