using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Administration.Staff
{
    public class ADMEmployeeJobType
    {
        public int JobTypeID { get; set; }
        public string JobType { get; set; }
        public bool DeleteName { get; set; }
        public int StaffCount { get; set; }
        public int Id { get; set; }
    }

    public class JobTypeValidator : AbstractValidator<ADMEmployeeJobType>
    {
        public JobTypeValidator()
        {
            RuleFor(job => job.JobType).NotEmpty().WithMessage("Job Type is required");
        }
    }

}
