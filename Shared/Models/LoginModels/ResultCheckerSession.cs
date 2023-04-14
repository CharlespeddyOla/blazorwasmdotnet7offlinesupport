namespace WebAppAcademics.Shared.Models.LoginModels
{
    public class ResultCheckerSession
    {
        public int STDID { get; set; }
        public string ParentPin { get; set; }
        public string AdmissionNo { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string StudentName { get; set; }
        public int ParentPinCount { get; set; }
        public bool ParentPinLock { get; set; }
        public int ResultTypeID { get; set; }
        public int ResultTermID { get; set; }
        public string Response { get; set; }
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime ExpiryTimeStamp { get; set; }
    }
}
