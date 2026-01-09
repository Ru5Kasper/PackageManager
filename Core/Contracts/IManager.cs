namespace PackageManager.Core.Contracts
{
    public interface IManager
    {
        bool Create(string path);

        bool Delete(string path);
    }
}
