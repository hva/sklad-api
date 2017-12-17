using FluentValidation;

namespace Sklad.Api.Modules.Auth
{
    public class TokenRequestValidator : AbstractValidator<TokenRequest>
    {
        public TokenRequestValidator()
        {
            RuleFor(x => x.GrantType).NotEmpty();
        }
    }
}
