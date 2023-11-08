using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.Tools.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DATABASE.Helpers
{
    public abstract class DBHelperAbstract : IDisposable
    {

        protected readonly string connection_str;
        protected DbContextHelper dbhelper;
        private bool disposedValue;

        protected AGVSDbContext dbContext => dbhelper._context;
        public DBHelperAbstract()
        {
            connection_str = AGVSConfigulator.SysConfigs.DBConnection;
            dbhelper = new DbContextHelper(connection_str);
        }

        public async Task<int> SaveChanges()
        {
            return await dbhelper._context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                }
                dbhelper.Dispose();
                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~DBHelperAbstract()
        // {
        //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
