using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Administration.Staff
{
    public class ADMEmployee
    {
        public int StaffID { get; set; }
        public int TermID { get; set; }
        public int SchInfoID { get; set; }
        public int PrefixID { get; set; }
        public int EmployeeID { get; set; }
        public string StaffNo { get; set; }
        public int TitleID { get; set; }
        public string Title { get; set; }
        public int EmployeeGroupID { get; set; }
        public string EmployeeGroup { get; set; }
        public bool MarkAsTeacher { get; set; }
        public bool MarkAsPrincipal { get; set; }
        public int GenderID { get; set; }
        public string Gender { get; set; }
        public string StaffGender { get; set; }
        public int MStatusID { get; set; }
        public string MStatus { get; set; }
        public int StatusTypeID { get; set; }
        public string StatusType { get; set; }
        public int JobTypeID { get; set; }
        public string JobType { get; set; }
        public int StudentCount { get; set; }
        public int CountryID { get; set; }
        public string Country { get; set; }
        public int StateID { get; set; }
        public string State { get; set; }
        public int LGAID { get; set; }
        public string LGA { get; set; }
        public string NationalID { get; set; }
        public string PensionID { get; set; }
        public string HealthInsureNo { get; set; }
        public int ACL { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string StaffName { get; set; }
        public string StaffNameWithNo { get; set; }
        public string StaffInitialsWithNo { get; set; }        
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string Qualification { get; set; }
        public string EmployeeAddr { get; set; }
        public string PhoneNos { get; set; }
        public string PhoneNosAlternate { get; set; }
        public string NextOfKin { get; set; }
        public string NextOfKinPhone { get; set; }
        public string NextOfKinAddress { get; set; }
        public string NextOfKinRelationship { get; set; }
        public int BankID { get; set; }
        public string BankAcctName { get; set; }
        public string AcctName { get; set; }
        public string AcctNumber { get; set; }
        public string Memo { get; set; }
        public int photoStatus { get; set; }
        public byte[] employeePicture { get; set; }
        public string ImageUrl { get; set; }
        public int ReligionID { get; set; }
        public string Religion { get; set; }
        public int LocID { get; set; }
        public string Location { get; set; }
        public byte[] signPicture { get; set; }
        public bool DeleteName { get; set; }
        public int ParentID { get; set; }
        public string ParentName { get; set; }
        public int RoleID { get; set; }
        public string RoleDesc { get; set; }
        public bool ResetPassword { get; set; }
        public string Password { get; set; }
        public int SN { get; set; }
        public string AcademicSession { get; set; }
        public string StaffPIN { get; set; }
        public DateTime AddOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool SignInStatus { get; set; }
        public int Id { get; set; }
    }

    public class EmployeValidator : AbstractValidator<ADMEmployee>
    {
        public EmployeValidator()
        {
            RuleFor(s => s.EmployeeID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Staff No. must be greater than 0")
                .GreaterThan(-1).WithMessage("Staff No. must be greater than 0");
            RuleFor(s => s.EmployeeGroup).NotEmpty().WithMessage("Staff Department is required");
            //RuleFor(s => s.Location).NotEmpty().WithMessage("Staff Location is required");
            RuleFor(s => s.JobType).NotEmpty().WithMessage("Staff Job Type is required");
            RuleFor(s => s.Gender).NotEmpty().WithMessage("Staff Gender is required");
            RuleFor(s => s.Title).NotEmpty().WithMessage("Staff Title is required");
            RuleFor(s => s.MStatus).NotEmpty().WithMessage("Staff Staff Marital Status is required");
            RuleFor(s => s.Religion).NotEmpty().WithMessage("Staff Religion is required");
            RuleFor(s => s.Surname).NotEmpty().WithMessage("Surname is required");
            RuleFor(s => s.FirstName).NotEmpty().WithMessage("First Name is required");
            RuleFor(s => s.JobTitle).NotEmpty().WithMessage("Job Title is required");
            RuleFor(s => s.PhoneNos).NotEmpty().WithMessage("Please Phone Number Is Required");
            RuleFor(s => s.PhoneNos).MinimumLength(11).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(s => s.PhoneNos).MaximumLength(11).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(s => s.Email).NotEmpty().EmailAddress().WithMessage("Please specify a valid email");
            RuleFor(s => s.BirthDate).NotEmpty().WithMessage("Please Select Staff Date of Birth");
            RuleFor(s => s.HireDate).NotEmpty().WithMessage("Please Select Staff Date of Employment");
            RuleFor(s => s.EmployeeAddr).NotEmpty().WithMessage("Staff Address Is Required");
            RuleFor(s => s.Qualification).NotEmpty().WithMessage("Staff Qualification Is Required");
            RuleFor(s => s.Country).NotEmpty().WithMessage("Please select Country");
            RuleFor(s => s.State).NotEmpty().WithMessage("Please select State");
            RuleFor(s => s.LGA).NotEmpty().WithMessage("Please select Local Government");
            RuleFor(s => s.NextOfKin).NotEmpty().WithMessage("Staff Next of Kin Is Required");
            RuleFor(s => s.NextOfKinPhone).NotEmpty().WithMessage("Next of Kin Phone Number Is Required");
            RuleFor(s => s.NextOfKinRelationship).NotEmpty().WithMessage("Next of Kin Relationship Is Required");
            RuleFor(s => s.NextOfKinAddress).NotEmpty().WithMessage("Next of Kin Address Is Required");
            RuleFor(s => s.BankAcctName).NotEmpty().WithMessage("Staff Bank Name Is Required");
            RuleFor(s => s.AcctName).NotEmpty().WithMessage("Staff Bank Account Name Is Required");
            RuleFor(s => s.AcctNumber).NotEmpty().WithMessage("Staff Bank Account Number Is Required");
        }
    }

}
