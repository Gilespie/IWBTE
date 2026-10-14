[System.Serializable]
public class SettingsData
{
    public int ResolutionIndex = 5;
    public int QualityIndex = 1;
    public bool Fullscreen = true;
    public int VSync = 1;
    public float MasterVolume = 0.75f;
    public float MusicVolume = 0.5f;
    public float VoiceVolume = 0.5f;
    public float SFXVolume = 0.5f;
    public float UIVolume = 0.2f;

    public string InputBindingsOverridesJson = "";
}