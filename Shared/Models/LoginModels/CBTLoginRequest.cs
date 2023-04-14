using FluentValidation;

namespace WebAppAcademics.Shared.Models.LoginModels
{
    public class CBTLoginRequest
    {
        public string StudentPin { get; set; }
        public string Password { get; set; }
    }

    public class CBTLoginRequestValidator : AbstractValidator<CBTLoginRequest>
    {
        public CBTLoginRequestValidator()
        {
            RuleFor(u => u.StudentPin).NotEmpty().WithMessage("PIN is required.");
            RuleFor(u => u.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}
