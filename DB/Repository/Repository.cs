using Contracts;
using DB.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DB.Repository
{
    public class RepositoryCommon<T> : IRepositoryCommon<T> where T : Entry
    {
        protected readonly MainDbContext dbContext;
        protected readonly ILogger<RepositoryCommon<T>> logger;
        public RepositoryCommon(MainDbContext dbContext, ILogger<RepositoryCommon<T>> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<FilteredList<T>> GetByFilter(Filter<T> filter)
        {
            try
            {
                var all = dbContext.Set<T>()
                    .Where(s => s.BeginDate >= filter.BeginDate && s.BeginDate <= filter.EndDate);

                if (filter.AddFilter != null) all = all.Where(filter.AddFilter);

                if (filter.Page.HasValue) all = all.Skip(filter.Page.Value);
                if (filter.Size.HasValue) all = all.Take(filter.Size.Value);

                if (!string.IsNullOrEmpty(filter.SortField))
                {
                    string sort = filter.SortField;
                    if (filter.DescSort == true)
                    {
                        sort += ", DESC";
                    }
                    all = all.OrderBy(filter.SortField);
                }
                else
                {
                    all = all.OrderBy(s => s.BeginDate);
                }

                return new FilteredList<T>()
                {
                    AllCount = await all.CountAsync(),
                    Entries = await all.ToListAsync()
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in method GetByFilter of Repository<{typeof(T).Name}> : {ex.Message} StackTrace: {ex.StackTrace}");
                throw new RepositoryException($"Error in method GetByFilter of Repository<{typeof(T).Name}> : {ex.Message}");
            }
        }
    }


    public class Repository<T> : RepositoryCommon<T>, IRepository<T> where T : Entry
    {
        public Repository(MainDbContext dbContext, ILogger<Repository<T>> logger) : base(dbContext, logger)
        {

        }

        public async Task<T> Get(Guid id)
        {
            try
            {
                return await dbContext.Set<T>().Where(s => s.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in method Get of Repository<{typeof(T).Name}> : {ex.Message} StackTrace: {ex.StackTrace}");
                throw new RepositoryException($"Error in method GetByFilter of Repository<{typeof(T).Name}> : {ex.Message}");
            }
        }

        public async Task<T> Add(T entity, bool withSaving = true)
        {
            try
            {
                entity.Id = Guid.NewGuid();
                entity.VersionDate = DateTimeOffset.Now;
                var result = dbContext.Set<T>().Add(entity).Entity;
                if (withSaving)
                {
                    await dbContext.SaveChangesAsync(default);
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in method Add of Repository<{typeof(T).Name}> : {ex.Message} StackTrace: {ex.StackTrace}");
                throw new RepositoryException($"Error in method GetByFilter of Repository<{typeof(T).Name}> : {ex.Message}");
            }
        }

        public async Task<T> Update(T entity, bool withSaving = true)
        {
            try
            {
                entity.VersionDate = DateTimeOffset.Now;
                var trackedEntity = dbContext.Attach(entity);
                trackedEntity.CurrentValues.SetValues(entity);
                trackedEntity.State = EntityState.Modified;
                trackedEntity.Property(s => s.EntryType).IsModified = false;
                //var result = dbContext.Set<T>().Update(entity);
                if (withSaving)
                {
                    await dbContext.SaveChangesAsync(default);
                }
                return trackedEntity.Entity;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in method Update of Repository<{typeof(T).Name}> : {ex.Message} StackTrace: {ex.StackTrace}");
                throw new RepositoryException($"Error in method GetByFilter of Repository<{typeof(T).Name}> : {ex.Message}");
            }
        }

        public async Task<T> Delete(Guid id, bool withSaving = true)
        {
            try
            {
                var entity = await dbContext.Set<T>().FirstOrDefaultAsync(s => s.Id == id);
                entity.VersionDate = DateTimeOffset.Now;
                entity.IsDeleted = true;
                var result = dbContext.Set<T>().Update(entity).Entity;
                if (withSaving)
                {
                    await dbContext.SaveChangesAsync(default);
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in method Delete of Repository<{typeof(T).Name}> : {ex.Message} StackTrace: {ex.StackTrace}");
                throw new RepositoryException($"Error in method GetByFilter of Repository<{typeof(T).Name}> : {ex.Message}");
            }
        }

    }

    public class RepositoryAll : RepositoryCommon<Entry>, IRepositoryAll
    {
        public RepositoryAll(MainDbContext dbContext, ILogger<RepositoryAll> logger) : base(dbContext, logger)
        {

        }
    }

    public class FilteredList<T> : IFilteredList<T> where T : Entry
    {
        public IEnumerable<T> Entries { get; set; }
        public int AllCount { get; set; }
        public int BeginNumber { get; set; }
        public int EndNumber { get; set; }
    }

    public class Filter<T>
    {
        public Expression<Func<T, bool>> AddFilter { get; set; }
        public int? Size { get; set; }
        public int? Page { get; set; }
        public string SortField { get; set; }
        public bool? DescSort { get; set; }
        public DateTimeOffset BeginDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }

    public class RepositoryException : Exception
    {
        public RepositoryException(string message) : base(message)
        {
        }
    }
}
