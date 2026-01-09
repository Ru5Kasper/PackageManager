namespace PackageManager.Core.Contracts
{
    public interface IDownloader
    {
        string Location { get; set; }

        void Download(string url);

        void Remove(string name);
    }
}
