using AGVSystemCommonNet6.AGVDispatch;
using AutoMapper;
using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DATABASE
{

    public class DBDataService
    {

        public class OperationResult
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
            public OperationResult(bool isSuccess, string message)
            {
                IsSuccess = isSuccess;
                Message = message;
            }
        }

        private SemaphoreSlim taskDtoLock = new SemaphoreSlim(1, 1);


        private Dictionary<string, SemaphoreSlim> tableLocksMap = new Dictionary<string, SemaphoreSlim>();

        IServiceScopeFactory _scopeFactory;
        public DBDataService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            tableLocksMap = InitTablesLocksMapper();
        }

        public async Task<OperationResult> AddEntityToTableAsync<T>(T entity) where T : class
        {
            using AGVSDbContext _dbContext = GetDBContext();
            string entityTypeName = typeof(T).Name;
            if (!tableLocksMap.TryGetValue(entityTypeName, out SemaphoreSlim _lock))
            {
                _lock = new SemaphoreSlim(1, 1);
                tableLocksMap.Add(entityTypeName, _lock);
            }
            try
            {
                await _lock.WaitAsync();
                var dbSet = _dbContext.Set<T>();
                if (CheckEntityExists(entity, _dbContext, out string key, out object keyVal, out object existEntity))
                    return new OperationResult(false, $"{entityTypeName} already exists same key({key})={keyVal.ToString()}");

                dbSet.Add(entity);
                await _dbContext.SaveChangesAsync();
                return new OperationResult(true, "Success");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message + "InnerException Message" + ex.InnerException?.Message);
            }
            finally
            {
                _lock.Release();
            }
        }
        public async Task<OperationResult> ModifyEntityInTableAsync<T>(T entity) where T : class
        {
            using AGVSDbContext _dbContext = GetDBContext();
            string entityTypeName = typeof(T).Name;
            if (!tableLocksMap.TryGetValue(entityTypeName, out SemaphoreSlim _lock))
            {
                _lock = new SemaphoreSlim(1, 1);
                tableLocksMap.Add(entityTypeName, _lock);
            }
            try
            {
                await _lock.WaitAsync();
                var dbSet = _dbContext.Set<T>();
                if (!CheckEntityExists(entity, _dbContext, out string key, out object keyVal, out object existEntity))
                {
                    _lock.Release();
                    return await AddEntityToTableAsync(entity);
                } // 檢查實體是否已經被追蹤

                var entry = _dbContext.Entry(existEntity);
                if (entry.State == EntityState.Detached)
                {
                    // 如果實體未被追蹤，將它附加並設置為 Modified
                    dbSet.Attach((T)existEntity);
                }
                // 更新數據
                _dbContext.Entry(existEntity).CurrentValues.SetValues(entity);
                await _dbContext.SaveChangesAsync();
                return new OperationResult(true, "Modification successful");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message + "InnerException Message" + ex.InnerException?.Message);
            }
            finally
            {
                if (_lock.CurrentCount == 0)
                    _lock.Release();
            }
        }
        public async Task<OperationResult> DeleteEntityFromTableAsync<T>(T entity) where T : class
        {
            using AGVSDbContext _dbContext = GetDBContext();
            string entityTypeName = typeof(T).Name;
            if (!tableLocksMap.TryGetValue(entityTypeName, out SemaphoreSlim _lock))
            {
                _lock = new SemaphoreSlim(1, 1);
                tableLocksMap.Add(entityTypeName, _lock);
            }
            try
            {
                await _lock.WaitAsync();
                var dbSet = _dbContext.Set<T>();
                if (!CheckEntityExists(entity, _dbContext, out string key, out object keyVal, out object existEntity))
                    return new OperationResult(false, $"Entity {entityTypeName} already does not exist");
                // 檢查實體是否已經被追蹤
                var entry = _dbContext.Entry(existEntity);
                if (entry.State == EntityState.Detached)
                {
                    // 如果未被追蹤，則附加該實體
                    dbSet.Attach((T)existEntity);
                }

                // 刪除實體
                dbSet.Remove((T)existEntity);
                await _dbContext.SaveChangesAsync();
                return new OperationResult(true, "Deletion successful");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message + "InnerException Message" + ex.InnerException?.Message);
            }
            finally
            {
                _lock.Release();
            }
        }

        private AGVSDbContext GetDBContext()
        {
            var _dbContext = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AGVSDbContext>();
            return _dbContext;
        }

        private Dictionary<string, SemaphoreSlim> InitTablesLocksMapper()
        { // 獲取 AGVSDbContext 的類型
            var dbContextType = typeof(AGVSDbContext);

            // 使用反射篩選出所有 DbSet 屬性
            var dbSetProperties = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.PropertyType.IsGenericType &&
                               prop.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToList();
            // 輸出 DbSet<T> 中泛型 T 的名稱
            Dictionary<string, SemaphoreSlim> tableLocksMap = new Dictionary<string, SemaphoreSlim>();
            foreach (var property in dbSetProperties)
            {
                var genericType = property.PropertyType.GetGenericArguments().FirstOrDefault();
                if (genericType != null)
                {
                    Console.WriteLine($"Property: {property.Name}, Generic Type: {genericType.Name}");
                    tableLocksMap.Add(genericType.Name, new SemaphoreSlim(1, 1));
                }
            }
            return tableLocksMap;
        }

        private bool CheckEntityExists<T>(T entity, AGVSDbContext _dbContext, out string key, out object keyVal, out object existEntity) where T : class
        {
            string entityTypeName = typeof(T).Name;
            key = "";
            existEntity = keyVal = null;

            if (entityTypeName == _dbContext.Tasks.GetType().GenericTypeArguments.FirstOrDefault().Name)
            {
                key = "TaskName";
                existEntity = _dbContext.Tasks.FirstOrDefault(_entity => _entity.TaskName == entity.GetType().GetProperty("TaskName").GetValue(entity).ToString());
                if (existEntity != null)
                    keyVal = existEntity.GetType().GetProperty("TaskName").GetValue(existEntity);
                return existEntity != null;
            }

            if (entityTypeName == _dbContext.DeepChargeRecords.GetType().GenericTypeArguments.FirstOrDefault().Name)
            {
                key = "OrderRecievedTime";
                existEntity = _dbContext.DeepChargeRecords.FirstOrDefault(_entity => _entity.OrderRecievedTime == (DateTime)entity.GetType().GetProperty("OrderRecievedTime").GetValue(entity));
                if (existEntity != null)
                    keyVal = existEntity.GetType().GetProperty(key).GetValue(existEntity);
                return existEntity != null;
            }
            return false;
        }
    }
}
