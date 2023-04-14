using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Settings
{
    public class SETSchCalendar
    {
        public int CalendarID { get; set; }
        public string SchTerm { get; set; }
        public int StartMonthID { get; set; }
        public string StartMonth { get; set; }
        public int EndMonthID { get; set; }
        public string EndMonth { get; set; }
        public int Id { get; set; }
    }

    public class SETSchCalendarValidator : AbstractValidator<SETSchCalendar>
    {
        public SETSchCalendarValidator()
        {
            RuleFor(c => c.SchTerm).NotEmpty().WithMessage("Term Cannot Be Empty");
            RuleFor(c => c.StartMonth).NotEmpty().WithMessage("School Calendar Start Month Cannot Be Empty");
            RuleFor(c => c.EndMonth).NotEmpty().WithMessage("School Calendar Month End Cannot Be Empty");

            RuleFor(calendar => calendar.StartMonth)
                .NotEqual(calendar => calendar.EndMonth)
                .When(calendar => !String.IsNullOrWhiteSpace(calendar.StartMonth));

            RuleFor(calendar => calendar.EndMonth)
                .NotEqual(calendar => calendar.StartMonth)
                .When(calendar => !String.IsNullOrWhiteSpace(calendar.EndMonth));           
        }
    }
}
