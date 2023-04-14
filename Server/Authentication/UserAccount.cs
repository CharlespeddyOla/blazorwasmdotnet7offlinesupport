namespace WebAppAcademics.Server.Authentication
{
    public class UserAccount
    {
        public int StaffID { get; set; }
        public string StaffPIN { get; set; }
        public string Email { get; set; }
        public string StaffNameWithNo { get; set; }
        public string Surname { get; set; }
        public int RoleID { get; set; }
        public string Role { get; set; }
        public string Response { get; set; }
    }

    public class ResultCheckerAccount
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
    }

    public class CBTAccount
    {
        public int STDID { get; set; }
        public int ClassListID { get; set; }
        public string StudentPin { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string Password { get; set; }
        public bool CBTLock { get; set; }
        public string Response { get; set; }
    }
}
