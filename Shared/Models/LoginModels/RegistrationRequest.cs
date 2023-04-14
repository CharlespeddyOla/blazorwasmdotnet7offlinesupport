using FluentValidation;

namespace WebAppAcademics.Shared.Models.LoginModels
{
    public class RegistrationRequest
    {
        public string StaffPIN { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
    {
        public RegistrationRequestValidator()
        {
            RuleFor(u => u.StaffPIN).NotEmpty().WithMessage("Please enter your PIN.");
            RuleFor(u => u.Email).NotEmpty().EmailAddress().WithMessage("Please specify a valid email.");
            RuleFor(u => u.Password).NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                .MaximumLength(16).WithMessage("Your password length must not exceed 16.");
            RuleFor(u => u.ConfirmPassword)
               .Equal(u => u.Password)
               .WithMessage("Passwords do not match");
        }
    }

}
