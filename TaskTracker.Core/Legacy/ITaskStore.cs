namespace TaskTracker.Core.Legacy;

public interface ITaskStore
{
    void Add(LegacyTask task);
    LegacyTask GetById(int id);
    void SaveToFile(LegacyTask task, string path);
    void SendEmail(string to, string body);
    string ExportCsv();
    void Backup(string folder);
    void PrintInvoice(LegacyTask task);
}
