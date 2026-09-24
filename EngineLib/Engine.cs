namespace EngineLib;

public interface IEngine
{
    string Name { get; }

    void CreateServer();
    void ReplaceWorld();
    void UpdateWorldOptions();
    void ResetWorld();
    void AddPlayer();
    void OpPlayer();
    void DeopPlayer();
    void RemovePlayer();
    void StartServer();
    void StopServer();
    void DeleteServer();
}
