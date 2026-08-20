public interface ISaveable
{
    void CaptureState(SaveGameData data);
    void RestoreState(SaveGameData data);
}