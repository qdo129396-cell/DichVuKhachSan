using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.DAL
{
    public class ServiceDAL
    {
        public List<ServiceDTO> GetAll()
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Services.OrderByDescending(x => x.ServiceId).ToList();
            List<ServiceDTO> list = new List<ServiceDTO>();
            string query = "SELECT ServiceId, ServiceCode, ServiceName, Category, Unit, UnitPrice, Description, IsActive, CreatedAt FROM dbo.Services ORDER BY ServiceId DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToService(row));
            }
            return list;
        }

        public List<ServiceDTO> GetActiveServices()
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Services.Where(x => x.IsActive).OrderBy(x => x.ServiceName).ToList();
            List<ServiceDTO> list = new List<ServiceDTO>();
            string query = "SELECT ServiceId, ServiceCode, ServiceName, Category, Unit, UnitPrice, Description, IsActive, CreatedAt FROM dbo.Services WHERE IsActive = 1 ORDER BY ServiceName ASC";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToService(row));
            }
            return list;
        }

        public ServiceDTO GetById(int serviceId)
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Services.FirstOrDefault(x => x.ServiceId == serviceId);
            string query = "SELECT ServiceId, ServiceCode, ServiceName, Category, Unit, UnitPrice, Description, IsActive, CreatedAt FROM dbo.Services WHERE ServiceId = @ServiceId";
            SqlParameter[] parameters = {
                new SqlParameter("@ServiceId", SqlDbType.Int) { Value = serviceId }
            };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return MapRowToService(dt.Rows[0]);
            }
            return null;
        }

        public int Insert(ServiceDTO s)
        {
            if (DatabaseHelper.IsOfflineMode) { s.ServiceId = OfflineStore.NextId(OfflineStore.Services, x => x.ServiceId); s.CreatedAt = DateTime.Now; OfflineStore.Services.Add(s); return s.ServiceId; }
            string query = @"INSERT INTO dbo.Services (ServiceCode, ServiceName, Category, Unit, UnitPrice, Description, IsActive, CreatedAt)
                             VALUES (@ServiceCode, @ServiceName, @Category, @Unit, @UnitPrice, @Description, @IsActive, SYSDATETIME());
                             SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
                new SqlParameter("@ServiceCode", SqlDbType.NVarChar, 20) { Value = s.ServiceCode },
                new SqlParameter("@ServiceName", SqlDbType.NVarChar, 120) { Value = s.ServiceName },
                new SqlParameter("@Category", SqlDbType.NVarChar, 50) { Value = s.Category },
                new SqlParameter("@Unit", SqlDbType.NVarChar, 30) { Value = s.Unit ?? "Lần" },
                new SqlParameter("@UnitPrice", SqlDbType.Decimal) { Value = s.UnitPrice },
                new SqlParameter("@Description", SqlDbType.NVarChar, 500) { Value = (object)s.Description ?? DBNull.Value },
                new SqlParameter("@IsActive", SqlDbType.Bit) { Value = s.IsActive }
            };

            object result = DatabaseHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        public bool Update(ServiceDTO s)
        {
            if (DatabaseHelper.IsOfflineMode) { var old = OfflineStore.Services.FirstOrDefault(x => x.ServiceId == s.ServiceId); if (old == null) return false; old.ServiceName = s.ServiceName; old.Category = s.Category; old.Unit = s.Unit; old.UnitPrice = s.UnitPrice; old.Description = s.Description; old.IsActive = s.IsActive; return true; }
            string query = @"UPDATE dbo.Services
                             SET ServiceName = @ServiceName, Category = @Category, Unit = @Unit, UnitPrice = @UnitPrice, Description = @Description, IsActive = @IsActive
                             WHERE ServiceId = @ServiceId";

            SqlParameter[] parameters = {
                new SqlParameter("@ServiceName", SqlDbType.NVarChar, 120) { Value = s.ServiceName },
                new SqlParameter("@Category", SqlDbType.NVarChar, 50) { Value = s.Category },
                new SqlParameter("@Unit", SqlDbType.NVarChar, 30) { Value = s.Unit ?? "Lần" },
                new SqlParameter("@UnitPrice", SqlDbType.Decimal) { Value = s.UnitPrice },
                new SqlParameter("@Description", SqlDbType.NVarChar, 500) { Value = (object)s.Description ?? DBNull.Value },
                new SqlParameter("@IsActive", SqlDbType.Bit) { Value = s.IsActive },
                new SqlParameter("@ServiceId", SqlDbType.Int) { Value = s.ServiceId }
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool SetStatus(int serviceId, bool isActive)
        {
            if (DatabaseHelper.IsOfflineMode) { var item = OfflineStore.Services.FirstOrDefault(x => x.ServiceId == serviceId); if (item == null) return false; item.IsActive = isActive; return true; }
            string query = "UPDATE dbo.Services SET IsActive = @IsActive WHERE ServiceId = @ServiceId";
            SqlParameter[] parameters = {
                new SqlParameter("@IsActive", SqlDbType.Bit) { Value = isActive },
                new SqlParameter("@ServiceId", SqlDbType.Int) { Value = serviceId }
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool Delete(int serviceId)
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Services.RemoveAll(x => x.ServiceId == serviceId) > 0;
            string query = "DELETE FROM dbo.Services WHERE ServiceId = @ServiceId";
            SqlParameter[] parameters = {
                new SqlParameter("@ServiceId", SqlDbType.Int) { Value = serviceId }
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool HasOrders(int serviceId)
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Orders.Any(x => x.Details.Any(d => d.ServiceId == serviceId));
            string query = "SELECT COUNT(*) FROM dbo.ServiceOrderDetails WHERE ServiceId = @ServiceId";
            SqlParameter[] parameters = {
                new SqlParameter("@ServiceId", SqlDbType.Int) { Value = serviceId }
            };
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters)) > 0;
        }

        public List<ServiceDTO> Search(string keyword, string category = "")
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Services.Where(x => ($"{x.ServiceCode} {x.ServiceName}").IndexOf(keyword ?? "", StringComparison.OrdinalIgnoreCase) >= 0 && (string.IsNullOrEmpty(category) || category == "Tất cả" || x.Category == category)).OrderByDescending(x => x.ServiceId).ToList();
            List<ServiceDTO> list = new List<ServiceDTO>();
            string query = @"SELECT ServiceId, ServiceCode, ServiceName, Category, Unit, UnitPrice, Description, IsActive, CreatedAt
                             FROM dbo.Services
                             WHERE (ServiceCode LIKE @Kw OR ServiceName LIKE @Kw)";

            List<SqlParameter> parameters = new List<SqlParameter> {
                new SqlParameter("@Kw", SqlDbType.NVarChar, 120) { Value = "%" + keyword + "%" }
            };

            if (!string.IsNullOrEmpty(category) && category != "Tất cả")
            {
                query += " AND Category = @Category";
                parameters.Add(new SqlParameter("@Category", SqlDbType.NVarChar, 50) { Value = category });
            }

            query += " ORDER BY ServiceId DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToService(row));
            }
            return list;
        }

        public List<string> GetCategories()
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Services.Select(x => x.Category).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).ToList();
            List<string> list = new List<string>();
            string query = "SELECT DISTINCT Category FROM dbo.Services WHERE Category IS NOT NULL AND Category <> '' ORDER BY Category ASC";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(row["Category"].ToString());
            }
            return list;
        }

        public bool IsCodeExists(string serviceCode, int excludeId = 0)
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Services.Any(x => x.ServiceCode == serviceCode && x.ServiceId != excludeId);
            string query = "SELECT COUNT(*) FROM dbo.Services WHERE ServiceCode = @Code AND ServiceId <> @ExcludeId";
            SqlParameter[] parameters = {
                new SqlParameter("@Code", SqlDbType.NVarChar, 20) { Value = serviceCode },
                new SqlParameter("@ExcludeId", SqlDbType.Int) { Value = excludeId }
            };
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters)) > 0;
        }

        public string GenerateNextServiceCode()
        {
            if (DatabaseHelper.IsOfflineMode) return $"DV{OfflineStore.NextId(OfflineStore.Services, x => x.ServiceId):D3}";
            string query = "SELECT ISNULL(MAX(ServiceId), 0) + 1 FROM dbo.Services";
            int nextId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
            return $"DV{nextId:D3}";
        }

        private ServiceDTO MapRowToService(DataRow row)
        {
            return new ServiceDTO
            {
                ServiceId = Convert.ToInt32(row["ServiceId"]),
                ServiceCode = row["ServiceCode"].ToString(),
                ServiceName = row["ServiceName"].ToString(),
                Category = row["Category"].ToString(),
                Unit = row["Unit"].ToString(),
                UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                Description = row["Description"] == DBNull.Value ? "" : row["Description"].ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"])
            };
        }
    }
}
