using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DATABASE
{
    public class DualDbTransactionInterceptor : DbTransactionInterceptor
    {
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        AGVSDbContext _warRoomDbContext;
        Logger logger = LogManager.GetCurrentClassLogger();
        public DualDbTransactionInterceptor() : base()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AGVSDbContext>();
            optionsBuilder.UseSqlServer("Server=127.0.0.1;Database=3FSE;User Id=sa;Password=12345678;Encrypt=False;MultipleActiveResultSets=True;Connection Lifetime=1;Min Pool Size=5;Max Pool Size=50;");
            _warRoomDbContext = new AGVSDbContext(optionsBuilder.Options, true);
        }

        public override ValueTask<DbTransaction> TransactionStartedAsync(DbConnection connection, TransactionEndEventData eventData, DbTransaction result, CancellationToken cancellationToken = default)
        {
            return base.TransactionStartedAsync(connection, eventData, result, cancellationToken);
        }
        public override async Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphore.WaitAsync();
                var entries = eventData.Context.ChangeTracker.Entries()
                                                             .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);
                foreach (var entry in entries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        _warRoomDbContext.Add(entry.Entity);
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        _warRoomDbContext.Remove(entry.Entity);
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        _warRoomDbContext.Update(entry.Entity);
                    }
                    //_warRoomDbContext.Entry(entry.Entity).State = entry.State;
                }
                await _warRoomDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "TransactionCommittedAsync");
            }
            finally
            {
                _semaphore.Release();
            }
            //return base.TransactionCommittedAsync(transaction, eventData, cancellationToken);
        }
    }
}
