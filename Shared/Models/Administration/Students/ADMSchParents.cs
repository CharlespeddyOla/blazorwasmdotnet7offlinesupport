using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Administration.Students
{
    public class ADMSchParents 
    {
        public int PayeeID { get; set; }
        public int ParentID { get; set; }
        public int SchInfoID { get; set; }
        public int StatusTypeID { get; set; }
        public int StudentCount { get; set; }
        public string ParentTitle { get; set; }
        public string ParentSurname { get; set; }
        public string ParentName { get; set; }
        public string FatherName { get; set; }
        public string FatherPhones { get; set; }
        public string FatherPhonesAlternate { get; set; }
        public string FatherOccupation { get; set; }
        public string FatherAddrHome { get; set; }
        public string FatherAddrWork { get; set; }
        public string FatherEmail { get; set; }
        public int photoStatusFather { get; set; }
        public byte[] fatherPhoto { get; set; }
        public string ImageUrl { get; set; }
        public string MotherName { get; set; }
        public string MotherPhones { get; set; }
        public string MotherPhonesAlternate { get; set; }
        public string MotherOccupation { get; set; }
        public string MotherAddrHome { get; set; }
        public string MotherAddrWork { get; set; }
        public string MotherEmail { get; set; }
        public int photoStatusMother { get; set; }
        public byte[] motherPhoto { get; set; }
        public string ImageUrlM { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string Memo { get; set; }
        public bool DeletePayee { get; set; }
        public int PrefixID { get; set; }
        public string ParentNo { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int Id { get; set; }
    }

    public class ParentDetailsValidator : AbstractValidator<ADMSchParents>
    {
        public ParentDetailsValidator()
        {
            RuleFor(p => p.ParentSurname).NotEmpty().WithMessage("Parent Surname is required");
            RuleFor(p => p.FatherName).NotEmpty().WithMessage("Father's Name is required");
            RuleFor(p => p.FatherPhones).NotEmpty().WithMessage("Father's Phone No. is required");
            RuleFor(p => p.FatherPhones).MinimumLength(11).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(p => p.FatherPhones).MaximumLength(11).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(p => p.FatherPhonesAlternate).MinimumLength(11).When(p => p.FatherPhonesAlternate != string.Empty).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(p => p.FatherPhonesAlternate).MaximumLength(11).When(p => p.FatherPhonesAlternate != string.Empty).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(p => p.FatherEmail).NotEmpty().EmailAddress().WithMessage("Please specify a valid email");
            RuleFor(p => p.FatherAddrHome).NotEmpty().WithMessage("Father's Home Address is required");

            RuleFor(p => p.MotherName).NotEmpty().WithMessage("Father's Name is required");
            RuleFor(p => p.MotherPhones).NotEmpty().WithMessage("Father's Phone No. is required");
            RuleFor(p => p.MotherPhones).MinimumLength(11).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(p => p.MotherPhones).MaximumLength(11).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(p => p.MotherPhonesAlternate).MinimumLength(11).When(p => p.MotherPhonesAlternate != string.Empty).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(p => p.MotherPhonesAlternate).MaximumLength(11).When(p => p.MotherPhonesAlternate != string.Empty).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(p => p.MotherEmail).NotEmpty().EmailAddress().WithMessage("Please specify a valid email");
            RuleFor(p => p.MotherAddrHome).NotEmpty().WithMessage("Father's Home Address is required");
        }
    }
}
