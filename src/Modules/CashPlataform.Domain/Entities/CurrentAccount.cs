using CashPlataform.Domain.Common.Enums;
using CashPlataform.Domain.Common.TransactionExecution;
using CashPlataform.Domain.Entities.Validations;

using Marraia.MongoDb.Core;
using Marraia.Notifications.Validations;

namespace CashPlataform.Domain.Entities
{
    public class CurrentAccount : Entity<Guid>
    {
        object lockTransact = new();
        public CurrentAccount() {}
        public CurrentAccount(string accountName)
        {
            Id = Guid.NewGuid();
            Balance = 0;
            AccountName = accountName;
        }
        public string AccountName { get; private set; }
        public decimal Balance { get; private set; }

        public CommandTransaction Transact(decimal value, OperationType operation)
        {
            var commandTransaction = new CommandTransaction();

            lock (lockTransact)
            {
                if (operation == OperationType.Credit)
                    Credit(commandTransaction, value);
                else
                    Debit(commandTransaction, value);
            }

            return commandTransaction;
        }

        private void Credit(CommandTransaction command, decimal value)
        {   
            Balance += value;

            command.ExecutionSucess();
        }

        private CommandTransaction Debit(CommandTransaction command, decimal value) 
        {
            if (Balance > 0 
                && Balance >= value) 
            {
                Balance -= value;

                command.ExecutionSucess();
            }
            else
            {
                command
                    .ErrorMessage($"Você não tem saldo suficiente para fazer esse débito de {value.ToString("c")}! Pois seu saldo é de {Balance.ToString("c")}");
            }

            return command;
        }

        public FieldValidation Validate()
        {
            var fieldValidation = new FieldValidation(true);

            var valid = new CurrentAccountValidation().Validate(this);

            if (valid.IsValid) return fieldValidation;

            fieldValidation.AssignValid(valid.IsValid);
            fieldValidation.AddValidation(valid);

            return fieldValidation;
        }
    }
}
