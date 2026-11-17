using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.DAL
{
    public class UserDAL
    {
        public UserDTO GetByUsername(string username)
        {
            string query = "SELECT UserId, Username, FullName, PasswordHash, Role, IsActive, CreatedAt FROM dbo.Users WHERE Username = @Username";
            SqlParameter[] parameters = {
                new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = username }
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return MapRowToUser(row);
            }
            return null;
        }

        public UserDTO GetById(int userId)
        {
            string query = "SELECT UserId, Username, FullName, PasswordHash, Role, IsActive, CreatedAt FROM dbo.Users WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return MapRowToUser(dt.Rows[0]);
            }
            return null;
        }

        public List<UserDTO> GetAll()
        {
            List<UserDTO> list = new List<UserDTO>();
            string query = "SELECT UserId, Username, FullName, PasswordHash, Role, IsActive, CreatedAt FROM dbo.Users ORDER BY UserId ASC";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToUser(row));
            }
            return list;
        }

        public int Insert(UserDTO user)
        {
            string query = @"INSERT INTO dbo.Users (Username, FullName, PasswordHash, Role, IsActive, CreatedAt)
                             VALUES (@Username, @FullName, @PasswordHash, @Role, @IsActive, SYSDATETIME());
                             SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
                new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = user.Username },
                new SqlParameter("@FullName", SqlDbType.NVarChar, 120) { Value = user.FullName },
                new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 500) { Value = user.PasswordHash },
                new SqlParameter("@Role", SqlDbType.NVarChar, 20) { Value = user.Role },
                new SqlParameter("@IsActive", SqlDbType.Bit) { Value = user.IsActive }
            };

            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        public bool Update(UserDTO user)
        {
            string query = @"UPDATE dbo.Users 
                             SET FullName = @FullName, Role = @Role, IsActive = @IsActive
                             WHERE UserId = @UserId";

            SqlParameter[] parameters = {
                new SqlParameter("@FullName", SqlDbType.NVarChar, 120) { Value = user.FullName },
                new SqlParameter("@Role", SqlDbType.NVarChar, 20) { Value = user.Role },
                new SqlParameter("@IsActive", SqlDbType.Bit) { Value = user.IsActive },
                new SqlParameter("@UserId", SqlDbType.Int) { Value = user.UserId }
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdatePassword(int userId, string passwordHash)
        {
            string query = "UPDATE dbo.Users SET PasswordHash = @PasswordHash WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 500) { Value = passwordHash },
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool SetStatus(int userId, bool isActive)
        {
            string query = "UPDATE dbo.Users SET IsActive = @IsActive WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@IsActive", SqlDbType.Bit) { Value = isActive },
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool Delete(int userId)
        {
            string query = "DELETE FROM dbo.Users WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public int CountAdmins()
        {
            string query = "SELECT COUNT(*) FROM dbo.Users WHERE Role = 'Admin' AND IsActive = 1";
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
        }

        public int CountAll()
        {
            string query = "SELECT COUNT(*) FROM dbo.Users";
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
        }

        private UserDTO MapRowToUser(DataRow row)
        {
            return new UserDTO
            {
                UserId = Convert.ToInt32(row["UserId"]),
                Username = row["Username"].ToString(),
                FullName = row["FullName"].ToString(),
                PasswordHash = row["PasswordHash"].ToString(),
                Role = row["Role"].ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"])
            };
        }
    }
}
