using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;


namespace WebAppAcademics.Shared.Models.Administration.Students
{
    public class ADMStudents
    {
        public int STDID { get; set; }
        public int SchInfoID { get; set; }       
        public int TermID { get; set; }        
        public int SchID { get; set; }        
        public string School { get; set; }
        public int PrefixID { get; set; }       
        public int StudentID { get; set; }
        public string AdmissionNo { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string StudentName { get; set; }
        public string StudentInitials { get; set; }
        public string StudentNameWithNo { get; set; }
        public int StatusTypeID { get; set; }
        public string StatusType { get; set; }
        public int StudentTypeID { get; set; }       
        public string StudentType { get; set; }  //Day Or Boarding
        public int ClassListID { get; set; }        
        public int ClassID { get; set; }
        public string ClassCode { get; set; }
        public string CATName { get; set; }
        public string ClassName { get; set; }        
        public int GenderID { get; set; }        
        public string Gender { get; set; }
        public string GenderAbrv { get; set; }
        public int ReligionID { get; set; }
        public string Religion { get; set; }
        public int DisciplineID { get; set; }
        public string Discipline { get; set; }
        public int ClubID { get; set; }
        public string ClubName { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int ClassTeacherID { get; set; }
        public int PayeeTypeID { get; set; }  
        public string PayeeType { get; set; }
        public int PayeeID { get; set; }        
        public int StaffID { get; set; }           
        public string ParentName { get; set; }
        public int CountryID { get; set; }
        public string Country { get; set; }
        public int StateID { get; set; }
        public string State { get; set; }
        public int LGAID { get; set; }
        public string LGA { get; set; }
        public DateTime? DOB { get; set; } //= DateTime.Parse("1900-01-01 00:00:00");
        public string Email { get; set; }
        public string Address { get; set; }
        public bool Enrollment { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string PhoneNumber { get; set; }
        public string AltPhoneNumber { get; set; }
        public string ExamNo { get; set; }
        public string ExamNoNECO { get; set; }
        public string Memo { get; set; }
        public int photoStatus { get; set; }
        public byte[] studentPhoto { get; set; }
        public string ImageUrl { get; set; }
        public int EDUID { get; set; }
        public string EDUInstitute { get; set; }  //PreviousSchoolName
        public string ClassPrevious { get; set; }
        public int ExitID { get; set; }
        public string ExitType { get; set; }
        public DateTime? ExitDate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool Promoted { get; set; }
        public int PayStatus { get; set; }
        public bool DeleteName { get; set; }       
        public string ParentPin { get; set; }
        public string StudentPin { get; set; }
        public string Password { get; set; }
        public int SN { get; set; }
        public string AcademicSession { get; set; }
        public string AccessToken { get; set; }       
        public bool ShowDetails { get; set; }
        public bool CBTLock { get; set; }
        public int PrefixDigits { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public int ParentPinCount { get; set; }
        public bool ParentPinLock { get; set; }
        public int ResultTypeID { get; set; }
        public int ResultTermID { get; set; }
        public int Id { get; set; }
    }

    public class StudentDetailsValidator : AbstractValidator<ADMStudents>
    {        
        public StudentDetailsValidator() 
        {
            RuleFor(s => s.StudentID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Adminsion No. must be greater than 0")
                .GreaterThan(-1).WithMessage("Adminsion No. must be greater than 0");

            RuleFor(s => s.Surname).NotEmpty().WithMessage("Surname is required");
            RuleFor(s => s.Surname).MaximumLength(50).WithMessage("Surname cannot be longer than 50 characters");
            RuleFor(s => s.FirstName).NotEmpty().WithMessage("First Name is required");
            RuleFor(s => s.FirstName).MaximumLength(50).WithMessage("First Name cannot be longer than 50 characters");
            RuleFor(s => s.School).NotEmpty().WithMessage("Please Select A School");
            RuleFor(s => s.ClassName).NotEmpty().WithMessage("Please Select Student Class");
            RuleFor(s => s.DOB).NotEmpty().WithMessage("Please Select Student Date of Birth");
            RuleFor(s => s.Gender).NotEmpty().WithMessage("Please Select Student Gender");
            RuleFor(s => s.StudentType).NotEmpty().WithMessage("Please Select Student Type");
            RuleFor(s => s.PhoneNumber).NotEmpty().WithMessage("Please Phone Number Is Required");
            RuleFor(s => s.PhoneNumber).MinimumLength(11).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(s => s.PhoneNumber).MaximumLength(11).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(s => s.Email).NotEmpty().EmailAddress().WithMessage("Please specify a valid email");
            RuleFor(s => s.Address).NotEmpty().WithMessage("Please enter student address");
            RuleFor(s => s.Discipline).NotEmpty().WithMessage("Please select student descipline");
            RuleFor(s => s.Country).NotEmpty().WithMessage("Please select Country");
            RuleFor(s => s.State).NotEmpty().WithMessage("Please select State");
            RuleFor(s => s.LGA).NotEmpty().WithMessage("Please select Local Government");
            RuleFor(s => s.Religion).NotEmpty().WithMessage("Please select Religion");
            RuleFor(s => s.PayeeType).NotEmpty().WithMessage("Please select Parent Type");
            RuleFor(s => s.ParentName).NotEmpty().WithMessage("Please select Parent");
            RuleFor(s => s.ClubName).NotEmpty().WithMessage("Please select Club");
            RuleFor(s => s.RoleName).NotEmpty().WithMessage("Please select Role");            
        }
    }

}
