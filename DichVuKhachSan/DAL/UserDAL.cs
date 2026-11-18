using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.DAL
{
    public class UserDAL
    {
        public UserDTO GetByUsername(string username)
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
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
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Users.FirstOrDefault(x => x.UserId == userId);
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
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Users.OrderBy(x => x.UserId).ToList();
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
            if (DatabaseHelper.IsOfflineMode) { user.UserId = OfflineStore.NextId(OfflineStore.Users, x => x.UserId); user.CreatedAt = DateTime.Now; OfflineStore.Users.Add(user); return user.UserId; }
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
            if (DatabaseHelper.IsOfflineMode) { var old = OfflineStore.Users.FirstOrDefault(x => x.UserId == user.UserId); if (old == null) return false; old.FullName = user.FullName; old.Role = user.Role; old.IsActive = user.IsActive; return true; }
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
            if (DatabaseHelper.IsOfflineMode) { var user = OfflineStore.Users.FirstOrDefault(x => x.UserId == userId); if (user == null) return false; user.PasswordHash = passwordHash; return true; }
            string query = "UPDATE dbo.Users SET PasswordHash = @PasswordHash WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 500) { Value = passwordHash },
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool SetStatus(int userId, bool isActive)
        {
            if (DatabaseHelper.IsOfflineMode) { var user = OfflineStore.Users.FirstOrDefault(x => x.UserId == userId); if (user == null) return false; user.IsActive = isActive; return true; }
            string query = "UPDATE dbo.Users SET IsActive = @IsActive WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@IsActive", SqlDbType.Bit) { Value = isActive },
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool Delete(int userId)
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Users.RemoveAll(x => x.UserId == userId) > 0;
            string query = "DELETE FROM dbo.Users WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public int CountAdmins()
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Users.Count(x => x.Role == "Admin" && x.IsActive);
            string query = "SELECT COUNT(*) FROM dbo.Users WHERE Role = 'Admin' AND IsActive = 1";
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
        }

        public int CountAll()
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Users.Count;
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
