using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using PosApp.Domain.Entities;
using PosApp.Domain.Repositories;

namespace PosApp.Console.Infrastructure
{
    public class SqliteTransactionRepository : ITransactionRepository
    {
        private readonly string _connectionString;

        public SqliteTransactionRepository(string dbPath)
        {
            _connectionString = "Data Source=" + dbPath;
            InitSchema();
        }

        private void InitSchema()
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
#if CLIENT_A
                const string clientCol = ",Notes TEXT NOT NULL DEFAULT ''";
#elif CLIENT_B
                const string clientCol = ",DebitCard TEXT NOT NULL DEFAULT ''";
#else
                const string clientCol = "";
#endif
                var sql =
                    "CREATE TABLE IF NOT EXISTS Transactions (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "ItemName TEXT NOT NULL," +
                    "Quantity INTEGER NOT NULL," +
                    "UnitPrice REAL NOT NULL," +
                    "TotalAmount REAL NOT NULL," +
                    "CreatedAt TEXT NOT NULL," +
                    "Status TEXT NOT NULL" +
                    clientCol + ")";
                new SqliteCommand(sql, conn).ExecuteNonQuery();
            }
        }

        public int Add(Transaction t)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
#if CLIENT_A
                const string sql =
                    "INSERT INTO Transactions (ItemName,Quantity,UnitPrice,TotalAmount,CreatedAt,Status,Notes) " +
                    "VALUES (@item,@qty,@price,@total,@created,@status,@notes); " +
                    "SELECT last_insert_rowid();";
#elif CLIENT_B
                const string sql =
                    "INSERT INTO Transactions (ItemName,Quantity,UnitPrice,TotalAmount,CreatedAt,Status,DebitCard) " +
                    "VALUES (@item,@qty,@price,@total,@created,@status,@debit); " +
                    "SELECT last_insert_rowid();";
#else
                const string sql =
                    "INSERT INTO Transactions (ItemName,Quantity,UnitPrice,TotalAmount,CreatedAt,Status) " +
                    "VALUES (@item,@qty,@price,@total,@created,@status); " +
                    "SELECT last_insert_rowid();";
#endif
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@item", t.ItemName);
                    cmd.Parameters.AddWithValue("@qty", t.Quantity);
                    cmd.Parameters.AddWithValue("@price", t.UnitPrice);
                    cmd.Parameters.AddWithValue("@total", t.TotalAmount);
                    cmd.Parameters.AddWithValue("@created", t.CreatedAt.ToString("o"));
                    cmd.Parameters.AddWithValue("@status", t.Status.ToString());
#if CLIENT_A
                    cmd.Parameters.AddWithValue("@notes", t.Notes);
#elif CLIENT_B
                    cmd.Parameters.AddWithValue("@debit", t.DebitCard);
#endif
                    t.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    return t.Id;
                }
            }
        }

        public IEnumerable<Transaction> GetPending()
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                const string sql = "SELECT * FROM Transactions WHERE Status='Pending'";
                using (var cmd = new SqliteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var list = new List<Transaction>();
                    while (reader.Read())
                    {
                        var t = new Transaction
                        {
                            Id          = reader.GetInt32(reader.GetOrdinal("Id")),
                            ItemName    = reader.GetString(reader.GetOrdinal("ItemName")),
                            Quantity    = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            UnitPrice   = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                            TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                            CreatedAt   = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedAt"))),
                            Status      = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), reader.GetString(reader.GetOrdinal("Status")))
                        };
#if CLIENT_B
                        t.DebitCard = reader.GetString(reader.GetOrdinal("DebitCard"));
#endif
                        list.Add(t);
                    }
                    return list;
                }
            }
        }

        public void UpdateStatus(IEnumerable<int> ids, TransactionStatus status)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                var idList = string.Join(",", ids);
                var sql = "UPDATE Transactions SET Status=@status WHERE Id IN (" + idList + ")";
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status.ToString());
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
