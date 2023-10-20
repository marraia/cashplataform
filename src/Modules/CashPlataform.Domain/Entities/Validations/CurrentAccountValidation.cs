using FluentValidation;

namespace CashPlataform.Domain.Entities.Validations
{
    public class CurrentAccountValidation : AbstractValidator<CurrentAccount>
    {
        public CurrentAccountValidation()
        {
            ValidateProperties();
        }

        private void ValidateProperties()
        {
            RuleFor(doc => doc.AccountName)
            .NotEmpty().WithMessage("O nome da conta é obrigatório");
        }
    }
}
