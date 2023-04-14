namespace WebAppAcademics.Client.Services
{
    public interface IAPIServices<T>
    {
        Task<int> CountAsync(string requestUri, int Id);
        Task<bool> DeleteAsync(string requestUri, int Id);
        Task ExportResultsAsync(string requestUri, T obj);
        Task<List<T>> GetAllAsync(string requestUri);
        Task<T> GetByIdAsync(string requestUri, int Id);
        Task<byte[]> GetResults(string requestUri);
        Task<string> GetStringAsync(string requestUri);
        Task<int> LicenseCountAsync(string requestUri, int Id);
        Task<T> SaveAsync(string requestUri, T obj);
        Task<T> UpdateAsync(string requestUri, int Id, T obj);
    }
}