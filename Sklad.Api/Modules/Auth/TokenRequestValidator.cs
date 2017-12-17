using FluentValidation;

namespace Sklad.Api.Modules.Auth
{
    public class TokenRequestValidator : AbstractValidator<TokenRequest>
    {
        public TokenRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.GrantType)
                .Must(x => x == "password")
                .WithMessage("invalid grant type");
        }
    }
}
