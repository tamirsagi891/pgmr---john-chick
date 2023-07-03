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
    
    
    internal static string FootStepsVolume = "StepsVolume";
    internal static string FootStepsSurfaceParam = "FootStepsSurface";

    public enum SurfaceSound
    {
        Grass,
        WoodPlatform,
        Cave,
        FallingPlatform
    }
}
