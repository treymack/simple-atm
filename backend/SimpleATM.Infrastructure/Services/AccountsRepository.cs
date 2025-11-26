using Dapper;
using MySqlConnector;
using SimpleATM.Domain.Entities;
using SimpleATM.Domain.Services;

namespace SimpleATM.Infrastructure.Services;

public class AccountsRepository(MySqlConnection db) : IAccountsRepository
{
    public async Task WipeAndSeedDatabaseAsync()
    {
        var sql = @"
        DROP TABLE IF EXISTS Transactions;
        DROP TABLE IF EXISTS Accounts;

        CREATE TABLE IF NOT EXISTS Accounts (
            Id INT PRIMARY KEY AUTO_INCREMENT,
            Type VARCHAR(50) NOT NULL,
            Balance DECIMAL(18,2) NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Transactions (
            Id INT PRIMARY KEY AUTO_INCREMENT,
            FromAccountId INT,
            ToAccountId INT,
            Type VARCHAR(50) NOT NULL,
            Amount DECIMAL(18,2) NOT NULL,
            Timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
            Description VARCHAR(255),
            FOREIGN KEY (FromAccountId) REFERENCES Accounts(Id),
            FOREIGN KEY (ToAccountId) REFERENCES Accounts(Id)
        );

        INSERT INTO Accounts (Type, Balance) VALUES
            ('checking', 0.00),
            ('savings', 0.00),
            ('checking', 0.00),
            ('savings', 0.00);
        ";

        await db.ExecuteAsync(sql);

        await DepositAsync(1, 1500m, "Seeding DB");
        await DepositAsync(2, 3000m, "Seeding DB");
        await DepositAsync(3, 500m, "Seeding DB");
        await DepositAsync(4, 7500m, "Seeding DB");
    }

    public Task<IEnumerable<Account>> GetAccounts()
    {
        return db.QueryAsync<Account>("SELECT Id, Type, Balance FROM Accounts");
    }

    public async Task<Account> DepositAsync(int accountId, decimal amount, string description)
    {
        using var trans = db.BeginTransaction();

        var sql = @"
        INSERT INTO Transactions (ToAccountId, Type, Amount, Description)
        VALUES (@AccountId, 'deposit', @Amount, @Description);

        UPDATE Accounts
        SET Balance = Balance + @Amount
        WHERE Id = @AccountId;

        SELECT Id, Type, Balance FROM Accounts WHERE Id = @AccountId";

        var res = await db.QuerySingleAsync<Account>(
            sql: sql,
            param: new { Amount = amount, AccountId = accountId, Description = description },
            transaction: trans);

        await trans.CommitAsync();

        return res;
    }

    public async Task<IEnumerable<Transaction>> GetAccountTransactions(int accountId)
    {
        // Would page results in a real app
        return await db.QueryAsync<Transaction>(
            sql: @"
            SELECT 
                Id,
                FromAccountId,
                ToAccountId,
                Type,
                Amount,
                Timestamp,
                Description
            FROM Transactions
            WHERE FromAccountId = @AccountId OR ToAccountId = @AccountId
            ORDER BY Timestamp DESC",
            param: new { AccountId = accountId });
    }

    public async Task<Account> WithdrawAsync(int accountId, decimal amount, string description)
    {
        using var trans = db.BeginTransaction();

        var sql = @"
            INSERT INTO Transactions (FromAccountId, Type, Amount, Description)
            VALUES (@AccountId, 'withdrawal', @Amount, @Description);

            UPDATE Accounts
              SET Balance = Balance - @Amount
              WHERE Id = @AccountId;

            SELECT Id, Type, Balance FROM Accounts WHERE Id = @AccountId";
        var res = await db.QuerySingleAsync<Account>(
            sql: sql,
            param: new { Amount = amount, AccountId = accountId, Description = description },
            transaction: trans);

        await trans.CommitAsync();

        return res;
    }
}