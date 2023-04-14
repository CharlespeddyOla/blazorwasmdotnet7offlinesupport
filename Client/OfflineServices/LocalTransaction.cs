namespace WebAppAcademics.Client.OfflineServices
{
    public class LocalTransaction<T>
    {
        public T Entity { get; set; }
        public LocalTransactionTypes Action { get; set; }
        public string ActionName { get; set; }
        public object Id { get; set; }
    }
}
