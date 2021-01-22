using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SimchaFund.Data
{
    public class SimchaFundDb
    {
        private string _connectionString;

        public SimchaFundDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Simcha> GetAllSimchas()
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Simchas";
                conn.Open();
                var simchas = new List<Simcha>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = (int)reader["Id"];

                    simchas.Add(new Simcha
                    {
                        Id = id,
                        Name = (string)reader["Name"],
                        Date = (DateTime)reader["Date"],
                        Contributions = GetContributionCountForSimcha(id),
                        Total = GetTotalAmountForSimcha(id)
                    });
                }
                return simchas;
            }
        }

        public int GetContributionCountForSimcha(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT COUNT(*) FROM Contributions
                                    WHERE SimchaId = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                int total = (int)cmd.ExecuteScalar();
                return total;
            }
        }

        public decimal GetTotalAmountForSimcha(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT ISNULL(SUM(Amount), 0) FROM Contributions
                                    WHERE SimchaId = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                decimal amt = (decimal)cmd.ExecuteScalar();
                return amt;
            }
        }

        public int GetContributorCount()
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count;
            }
        }

        public void AddSimcha(Simcha simcha)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Simchas (Name, Date)
                                    VALUES (@name, @date)";
                cmd.Parameters.AddWithValue("@name", simcha.Name);
                cmd.Parameters.AddWithValue("@date", simcha.Date);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public Simcha GetSimchaById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Simchas
                                    WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                Simcha simcha = new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    Contributions = GetContributionCountForSimcha(id),
                    Total = GetTotalAmountForSimcha(id)
                };
                return simcha;
            }
        }

        public List<Contributor> GetContributors()
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Contributors";
                conn.Open();
                var contributors = new List<Contributor>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    contributors.Add(new Contributor
                    {
                        Id = (int)reader["Id"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        AlwaysInclude = (bool)reader["AlwaysInclude"],
                        Balance = GetBalance((int)reader["Id"]),
                        CellNumber = (Int64)reader["CellNumber"],
                        DateCreated = (DateTime)reader["DateCreated"]
                    });
                }
                return contributors.OrderBy(c => c.LastName).ToList();
            }
        }

        public void SetContributionAmountForContributorForSimcha(int simchaId, Contributor contributor)
        {
            var contributions = GetContributionsForSimcha(simchaId);
            var contribution = contributions.FirstOrDefault(c => c.ContributorId == contributor.Id);
            if (contribution != null)
            {
                contributor.Amount = contribution.Amount;
            }
        }

        public List<Contribution> GetContributionsForSimcha(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Contributions WHERE SimchaId = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                var contributions = new List<Contribution>();
                while (reader.Read())
                {
                    contributions.Add(new Contribution
                    {
                        ContributorId = (int)reader["contributorId"],
                        Amount = (decimal)reader["amount"]
                    });
                }
                return contributions;
            }
        }

        public decimal GetBalance(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT SUM(Amount) FROM Deposits
                                    WHERE ContributorId = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                if (cmd.ExecuteScalar() is DBNull)
                {
                    return 0;
                }
                decimal deposits = (decimal)cmd.ExecuteScalar();

                cmd.CommandText = @"SELECT SUM(Amount) From Contributions
                                    WHERE ContributorId = @contributorId";
                cmd.Parameters.AddWithValue("@contributorId", id);
                if (cmd.ExecuteScalar() is DBNull)
                {
                    return deposits;
                }
                decimal contributions = (decimal)cmd.ExecuteScalar();
                return deposits - contributions;
            }
        }

        public void UpdateContributions(List<ContributionInclusion> contributors, int simchaId)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Contributions WHERE SimchaId = @simchaId";
                    cmd.Parameters.AddWithValue("@simchaId", simchaId);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"INSERT INTO Contributions (SimchaId, ContributorId, Amount)
                                        VALUES (@simchaId, @contributorId, @amount)";

                    foreach(ContributionInclusion c in contributors.Where(c => c.Include))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@amount", c.Amount);
                        cmd.Parameters.AddWithValue("@simchaId", simchaId);
                        cmd.Parameters.AddWithValue("@contributorId", c.ContributorId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        #region functions...
        public void AddDeposit(Deposit deposit)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Deposits(ContributorId, Amount, Date)
                                    VALUES(@id, @amount, @date)";
                cmd.Parameters.AddWithValue("@id", deposit.ContributorId);
                cmd.Parameters.AddWithValue("@amount", deposit.Amount);
                cmd.Parameters.AddWithValue("@date", deposit.Date);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public int AddContributor(Contributor contributor)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Contributors(FirstName, LastName, CellNumber, DateCreated, AlwaysInclude)
                                    VALUES(@firstName, @lastName, @cellNumber, @dateCreated, @alwaysInclude)
                                    SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
                cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
                cmd.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
                cmd.Parameters.AddWithValue("@dateCreated", contributor.DateCreated);
                cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
                conn.Open();
                int id = (int)(decimal)cmd.ExecuteScalar();
                return id;
            }
        }

        public List<Action> GetActions(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                var actions = new List<Action>();
                cmd.CommandText = @"SELECT c.Amount, s.Date, s.Name FROM Contributions c
                                    JOIN Simchas s
                                    ON c.SimchaId = s.Id
                                    WHERE c.ContributorId = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    actions.Add(new Action
                    {
                        Title = $"Contribution for {(string)reader["Name"]}",
                        Amount = (decimal)reader["Amount"],
                        Date = (DateTime)reader["Date"]
                    });
                }
                reader.Close();

                cmd.CommandText = @"SELECT d.Amount, d.Date FROM Deposits d
                                    WHERE ContributorId = @id";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    actions.Add(new Action
                    {
                        Title = "Deposit",
                        Amount = (decimal)reader["Amount"],
                        Date = (DateTime)reader["Date"]
                    });
                }

                return actions;
            }
        }

        public Contributor GetContributorById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Contributors
                                    WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                var contributor = new Contributor
                {
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Balance = GetBalance(id)
                };

                return contributor;
            }
        }

        public void EditContributor(Contributor c)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Contributors
                                    SET FirstName = @firstName, LastName = @lastName,
                                    CellNumber = @cellNumber, DateCreated = @dateCreated, AlwaysInclude = @alwaysInclude
                                    WHERE Id = @id";
                cmd.Parameters.AddWithValue("@firstName", c.FirstName);
                cmd.Parameters.AddWithValue("@lastName", c.LastName);
                cmd.Parameters.AddWithValue("@cellNumber", c.CellNumber);
                cmd.Parameters.AddWithValue("@dateCreated", c.DateCreated);
                cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
                cmd.Parameters.AddWithValue("@id", c.Id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        #endregion
        
    }
}
