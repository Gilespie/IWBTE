[System.Serializable]
public class SettingsData
{
    public int ResolutionIndex = 5;
    public int QualityIndex = 1;
    public bool Fullscreen = true;
    public int VSync = 1;
    public int MasterVolume = 80;

    public string InputBindingsOverridesJson = "";
}