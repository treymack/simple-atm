using MySqlConnector;

namespace SimpleATM.Infrastructure.Services
{
    public class TransactionToken(
        MySqlTransaction transaction, Action disposeAction) : IDisposable
    {
        public MySqlTransaction Transaction => transaction;

        public void Dispose()
        {
            transaction.Dispose();
            disposeAction();
        }
    }
}