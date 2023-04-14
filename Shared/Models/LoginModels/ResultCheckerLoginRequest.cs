using FluentValidation;

namespace WebAppAcademics.Shared.Models.LoginModels
{
    public class ResultCheckerLoginRequest
    {
        public string ParentPin { get; set; }
    }

    public class ResultCheckerLoginRequestValidator : AbstractValidator<ResultCheckerLoginRequest>
    {
        public ResultCheckerLoginRequestValidator()
        {
            RuleFor(u => u.ParentPin).NotEmpty().WithMessage("PIN is required.");
        }
    }
}
