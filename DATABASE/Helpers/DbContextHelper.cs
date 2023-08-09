using Microsoft.EntityFrameworkCore;
using AGVSystemCommonNet6.TASK;

namespace AGVSystemCommonNet6.DATABASE.Helpers
{
    public class DbContextHelper : IDisposable
    {
        private readonly string _connectionString;
        internal AGVSDbContext _context;
        private bool disposedValue;

        public DbContextHelper(string connectionString)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(connectionString.Split('=')[1]));
            _connectionString = connectionString;
            CreateDbContext();
        }

        private void CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AGVSDbContext>();
            optionsBuilder.UseSqlite(_connectionString);
            _context = new AGVSDbContext(optionsBuilder.Options);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                _context.Dispose();
                disposedValue = true;
            }
        }

        // ~DbContextHelper()
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
