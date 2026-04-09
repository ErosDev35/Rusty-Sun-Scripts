using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OptionData
{
    public float masterVolume = 0.5f;
    public float sfxVolume = 0.5f;
    public float musicVolume = 0.5f;
    public float volumeMute = 0;
    public float screenResolution = 0;
    public float screenMode = 1;
    public float pixelisation = 1;
    public float dithering = 1;
    public float chromaticAberration = 1;
    public float language = 1;
    public List<string> keybindings = new List<string>();
    public Dictionary<int, Vector2Int> screenResolutionDict = new Dictionary<int, Vector2Int>()
    { { 0, new Vector2Int(1920, 1080) }, { 1, new Vector2Int(1366, 768) }, { 2, new Vector2Int(1280, 720) } };
    public Dictionary<int, FullScreenMode> screenModeDict = new Dictionary<int, FullScreenMode>()
    { { 0, FullScreenMode.FullScreenWindow }, { 1, FullScreenMode.Windowed }};
}
