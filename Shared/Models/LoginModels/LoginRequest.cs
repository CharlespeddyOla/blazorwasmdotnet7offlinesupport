using FluentValidation;
using WebAppAcademics.Shared.Models.Administration.Staff;

namespace WebAppAcademics.Shared.Models.LoginModels
{
    public  class LoginRequest
    {        
        public string Email { get; set; }
        public string Password { get; set; }

        public List<ADMEmployee> staffList { get; set; }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(u => u.Email).NotEmpty().EmailAddress().WithMessage("Please specify a valid email.");
            RuleFor(u => u.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}
