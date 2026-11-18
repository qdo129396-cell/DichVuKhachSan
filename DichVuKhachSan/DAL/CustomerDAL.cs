using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.DAL
{
    public class CustomerDAL
    {
        public List<CustomerDTO> GetAll()
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Customers.Where(x => x.IsActive).OrderByDescending(x => x.CustomerId).ToList();
            List<CustomerDTO> list = new List<CustomerDTO>();
            string query = "SELECT CustomerId, CustomerCode, FullName, Phone, Email, Address, Notes, IsActive, CreatedAt FROM dbo.Customers WHERE IsActive = 1 ORDER BY CustomerId DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToCustomer(row));
            }
            return list;
        }

        public CustomerDTO GetById(int customerId)
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Customers.FirstOrDefault(x => x.CustomerId == customerId);
            string query = "SELECT CustomerId, CustomerCode, FullName, Phone, Email, Address, Notes, IsActive, CreatedAt FROM dbo.Customers WHERE CustomerId = @CustomerId";
            SqlParameter[] parameters = {
                new SqlParameter("@CustomerId", SqlDbType.Int) { Value = customerId }
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return MapRowToCustomer(dt.Rows[0]);
            }
            return null;
        }

        public CustomerDTO GetByCode(string code)
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Customers.FirstOrDefault(x => x.CustomerCode == code);
            string query = "SELECT CustomerId, CustomerCode, FullName, Phone, Email, Address, Notes, IsActive, CreatedAt FROM dbo.Customers WHERE CustomerCode = @CustomerCode";
            SqlParameter[] parameters = {
                new SqlParameter("@CustomerCode", SqlDbType.NVarChar, 20) { Value = code }
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return MapRowToCustomer(dt.Rows[0]);
            }
            return null;
        }

        public int Insert(CustomerDTO customer)
        {
            if (DatabaseHelper.IsOfflineMode) { customer.CustomerId = OfflineStore.NextId(OfflineStore.Customers, x => x.CustomerId); customer.CreatedAt = DateTime.Now; OfflineStore.Customers.Add(customer); return customer.CustomerId; }
            string query = @"INSERT INTO dbo.Customers (CustomerCode, FullName, Phone, Email, Address, Notes, IsActive, CreatedAt)
                             VALUES (@CustomerCode, @FullName, @Phone, @Email, @Address, @Notes, @IsActive, SYSDATETIME());
                             SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
                new SqlParameter("@CustomerCode", SqlDbType.NVarChar, 20) { Value = customer.CustomerCode },
                new SqlParameter("@FullName", SqlDbType.NVarChar, 120) { Value = customer.FullName },
                new SqlParameter("@Phone", SqlDbType.NVarChar, 20) { Value = (object)customer.Phone ?? DBNull.Value },
                new SqlParameter("@Email", SqlDbType.NVarChar, 120) { Value = (object)customer.Email ?? DBNull.Value },
                new SqlParameter("@Address", SqlDbType.NVarChar, 250) { Value = (object)customer.Address ?? DBNull.Value },
                new SqlParameter("@Notes", SqlDbType.NVarChar, 500) { Value = (object)customer.Notes ?? DBNull.Value },
                new SqlParameter("@IsActive", SqlDbType.Bit) { Value = customer.IsActive }
            };

            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        public bool Update(CustomerDTO customer)
        {
            if (DatabaseHelper.IsOfflineMode) { var old = OfflineStore.Customers.FirstOrDefault(x => x.CustomerId == customer.CustomerId); if (old == null) return false; old.FullName = customer.FullName; old.Phone = customer.Phone; old.Email = customer.Email; old.Address = customer.Address; old.Notes = customer.Notes; old.IsActive = customer.IsActive; return true; }
            string query = @"UPDATE dbo.Customers
                             SET FullName = @FullName, Phone = @Phone, Email = @Email, Address = @Address, Notes = @Notes, IsActive = @IsActive
                             WHERE CustomerId = @CustomerId";

            SqlParameter[] parameters = {
                new SqlParameter("@FullName", SqlDbType.NVarChar, 120) { Value = customer.FullName },
                new SqlParameter("@Phone", SqlDbType.NVarChar, 20) { Value = (object)customer.Phone ?? DBNull.Value },
                new SqlParameter("@Email", SqlDbType.NVarChar, 120) { Value = (object)customer.Email ?? DBNull.Value },
                new SqlParameter("@Address", SqlDbType.NVarChar, 250) { Value = (object)customer.Address ?? DBNull.Value },
                new SqlParameter("@Notes", SqlDbType.NVarChar, 500) { Value = (object)customer.Notes ?? DBNull.Value },
                new SqlParameter("@IsActive", SqlDbType.Bit) { Value = customer.IsActive },
                new SqlParameter("@CustomerId", SqlDbType.Int) { Value = customer.CustomerId }
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool Delete(int customerId)
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Customers.RemoveAll(x => x.CustomerId == customerId) > 0;
            string query = "DELETE FROM dbo.Customers WHERE CustomerId = @CustomerId";
            SqlParameter[] parameters = {
                new SqlParameter("@CustomerId", SqlDbType.Int) { Value = customerId }
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool HasOrders(int customerId)
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Orders.Any(x => x.CustomerId == customerId);
            string query = "SELECT COUNT(*) FROM dbo.ServiceOrders WHERE CustomerId = @CustomerId";
            SqlParameter[] parameters = {
                new SqlParameter("@CustomerId", SqlDbType.Int) { Value = customerId }
            };
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters)) > 0;
        }

        public List<CustomerDTO> Search(string keyword)
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Customers.Where(x => x.IsActive && ($"{x.CustomerCode} {x.FullName} {x.Phone} {x.Email}").IndexOf(keyword ?? "", StringComparison.OrdinalIgnoreCase) >= 0).OrderByDescending(x => x.CustomerId).ToList();
            List<CustomerDTO> list = new List<CustomerDTO>();
            string query = @"SELECT CustomerId, CustomerCode, FullName, Phone, Email, Address, Notes, IsActive, CreatedAt 
                             FROM dbo.Customers 
                             WHERE IsActive = 1 AND (CustomerCode LIKE @Kw OR FullName LIKE @Kw OR Phone LIKE @Kw OR Email LIKE @Kw)
                             ORDER BY CustomerId DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@Kw", SqlDbType.NVarChar, 120) { Value = "%" + keyword + "%" }
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToCustomer(row));
            }
            return list;
        }

        public string GenerateNextCustomerCode()
        {
            if (DatabaseHelper.IsOfflineMode) return $"KH{OfflineStore.NextId(OfflineStore.Customers, x => x.CustomerId):D3}";
            string query = "SELECT ISNULL(MAX(CustomerId), 0) + 1 FROM dbo.Customers";
            int nextId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
            return $"KH{nextId:D3}";
        }

        private CustomerDTO MapRowToCustomer(DataRow row)
        {
            return new CustomerDTO
            {
                CustomerId = Convert.ToInt32(row["CustomerId"]),
                CustomerCode = row["CustomerCode"].ToString(),
                FullName = row["FullName"].ToString(),
                Phone = row["Phone"] == DBNull.Value ? "" : row["Phone"].ToString(),
                Email = row["Email"] == DBNull.Value ? "" : row["Email"].ToString(),
                Address = row["Address"] == DBNull.Value ? "" : row["Address"].ToString(),
                Notes = row["Notes"] == DBNull.Value ? "" : row["Notes"].ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"])
            };
        }
    }
}
