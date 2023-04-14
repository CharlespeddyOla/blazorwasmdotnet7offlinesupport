namespace WebAppAcademics.Shared.Models.LoginModels
{
    public class CBTSession
    {
        public int STDID { get; set; }
        public int ClassListID { get; set; }
        public string StudentPin { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string Password { get; set; }
        public bool CBTLock { get; set; }
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime ExpiryTimeStamp { get; set; }
        public string Response { get; set; }
    }
}
