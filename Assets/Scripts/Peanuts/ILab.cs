public interface ILab
{
    string LabId { get; }
    void BeginLab();
    void SaveLive();
    void SaveAndClose();
}