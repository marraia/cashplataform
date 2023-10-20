namespace CashPlataform.Domain.Common.TransactionExecution
{
    public class CommandTransaction
    {
        public CommandTransaction()
        {
            Sucess = false;
        }

        public bool Sucess { get; private set; }
        public string Message { get; private set; }

        public void ErrorMessage(string message)
        {
            Message = message;
        }

        public void ExecutionSucess()
        {
            Sucess = true;
        }
    }
}
