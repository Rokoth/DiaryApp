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
    public class RepositoryCommon<T> : IRepositoryCommon<T> where T : Entity
    {
        protected readonly MainDbContext dbContext;
        protected readonly ILogger<RepositoryCommon<T>> logger;
        public RepositoryCommon(MainDbContext dbContext, ILogger<RepositoryCommon<T>> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<FilteredList<T>> GetByFilter(Filter<T> filter, string include = null)
        {
            try
            {
                IQueryable<T> all = null;
                if (!string.IsNullOrEmpty(include))
                {
                    all = dbContext.Set<T>().Include(include);
                }
                else
                {
                    all = dbContext.Set<T>();
                }



                if (filter.AddFilter != null) all = all.Where(filter.AddFilter);

                if (!string.IsNullOrEmpty(filter.SortField))
                {
                    string sort = filter.SortField;
                    if (filter.DescSort == true)
                    {
                        sort += " DESC";
                        all = all.OrderBy(sort);
                    }
                    all = all.OrderBy(sort);
                }

                if (filter.Page.HasValue && filter.Size.HasValue) all = all.Skip(filter.Page.Value * filter.Size.Value);
                if (filter.Size.HasValue) all = all.Take(filter.Size.Value);

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

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync(default);
        }
    }


    public class RepositoryEntry<T> : Repository<T> where T : Entry
    {
        public RepositoryEntry(MainDbContext dbContext, ILogger<Repository<T>> logger) : base(dbContext, logger)
        {

        }

        public override async Task<T> Update(T entity, bool withSaving = true)
        {
            try
            {
                entity.VersionDate = DateTimeOffset.Now;
                var trackedEntity = dbContext.Attach(entity);
                trackedEntity.CurrentValues.SetValues(entity);
                trackedEntity.State = EntityState.Modified;
                trackedEntity.Property(s => s.EntryType).IsModified = false;
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
    }

    public class Repository<T> : RepositoryCommon<T>, IRepository<T> where T : Entity
    {
        public Repository(MainDbContext dbContext, ILogger<Repository<T>> logger) : base(dbContext, logger)
        {

        }

        public async Task<T> Get(Guid id, string include = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(include))
                {
                    return await dbContext.Set<T>().Include(include).Where(s => s.Id == id).FirstOrDefaultAsync();
                }
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

        public virtual async Task<T> Update(T entity, bool withSaving = true)
        {
            try
            {
                entity.VersionDate = DateTimeOffset.Now;
                var trackedEntity = dbContext.Attach(entity);
                trackedEntity.CurrentValues.SetValues(entity);
                trackedEntity.State = EntityState.Modified;
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

    public class RepositoryAll : RepositoryCommon<Entry>, IRepositoryAll<Entry>
    {
        public RepositoryAll(MainDbContext dbContext, ILogger<RepositoryAll> logger) : base(dbContext, logger)
        {

        }
    }

    public class FilteredList<T> : IFilteredList<T> where T : Entity
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
    }

    public class RepositoryException : Exception
    {
        public RepositoryException(string message) : base(message)
        {
        }
    }
}
