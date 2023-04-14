using FluentValidation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Settings
{
    public class SETSchSessions 
    {
        public int TermID { get; set; }
        public int SchSession { get; set; }
        public string AcademicYear { get; set; }
        public string AcademicSession { get; set; }
        public string SchTerm { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.Today;
        public DateTime? EndDate { get; set; } = DateTime.Today;
        public bool Status { get; set; }
        public int Attendance { get; set; }
        public int StartMonthID { get; set; }
        public string StartMonth { get; set; }
        public int EndMonthID { get; set; }
        public string EndMonth { get; set; }       
        public int CalendarID { get; set; }
        public int Id { get; set; }
    }

    public class SchSessionDetailsValidator : AbstractValidator<SETSchSessions>
    {      
        public SchSessionDetailsValidator()
        {           
            RuleFor(a => a.StartDate)
                .Cascade(CascadeMode.Stop).NotEmpty()
                .WithMessage("Please Select Start Date");
                
            RuleFor(a => a.EndDate)
                .Cascade(CascadeMode.Stop).NotEmpty()
                .WithMessage("Please Select End Date")
                .GreaterThan(a => a.StartDate.Value)
                .WithMessage("Academic Session End Date must be greater than Start Date");
            
            RuleFor(a => a.Attendance)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("Total Expected Attendance For The Term Must Be Greater Than 0")
               .GreaterThan(-1).WithMessage("Total Expected Attendance For The Term Must Be Greater Than 0");
        }       
    }
}
