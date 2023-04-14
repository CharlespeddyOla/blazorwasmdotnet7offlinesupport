namespace WebAppAcademics.Shared.Models.LoginModels
{
    public class UserSession
    {
        public int StaffID { get; set; }
        public string StaffPIN { get; set; }
        public string Email { get; set; }
        public string StaffNameWithNo { get; set; }
        public string Surname { get; set; }
        public int RoleID { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime ExpiryTimeStamp { get; set; }
        public string Response { get; set; }
    }
}
