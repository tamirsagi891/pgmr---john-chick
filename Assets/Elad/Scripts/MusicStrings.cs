using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class MusicStrings
{
    internal static string musicVol = "MusicVolume";
    
    internal static string areaParam = "area";

    public enum AreaSound
    {
        OpenField,
        Cave
    }
    
    [Header("FootSteps")]
    internal static string FootStepsVolume = "StepsVolume";
    internal static string FootStepsSurfaceParam = "FootStepsSurface";
    internal static string FootStepsPitch = "FootStepsPitch";
    
    public enum SurfaceSound
    {
        Grass,
        WoodPlatform,
        Cave,
        FallingPlatform
    }
    
    
    [Header("HeartBeat")]
    internal static string HeartBeatPitch = "heartBeatPitch";

    [Header("Wind")]
    internal static string shortWindVol = "ShortWindVol";
}
