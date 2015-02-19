using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BamPi.EntityFramework
{
    public class BamPiEfDataConext : IBamPiDataContext
    {
        public BamPiEfDataConext(Func<DbContext> geDbContext)
        {
            DbContext = geDbContext;
        }

        public Func<DbContext> DbContext { get; set; }

        public async Task<IEnumerable<T>> Query<T>(Func<IQueryable<T>, IQueryable<T>> query) where T : class
        {
            using (var dbContext = DbContext())
            {
                return await query(dbContext.Set<T>()).ToListAsync();
            }
        }

        public async Task<bool> Delete<T>(string id) where T : class
        {
            using (var dbContext = DbContext())
            {
                dbContext.Set<T>().Remove(await dbContext.Set<T>().FindAsync(GetId(id)));
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<T> Add<T>(T entity) where T : class
        {
            using (var dbContext = DbContext())
            {
                dbContext.Set<T>().Add(entity);
                await dbContext.SaveChangesAsync();
                return entity;
            }
        }

        public async Task<T> Update<T>(string id, T entity) where T : class
        {
            using (var dbContext = DbContext())
            {
                var keyNames = EntityKeyHelper.Instance.GetKeyNames<T>(dbContext).ToList();
                if (keyNames.Count > 1)
                {
                    throw new ArgumentException("BamPiEfDataContext can only update entities with one key.");
                }
                var keyName = keyNames[0];
                entity.GetType().GetProperty(keyName).SetValue(entity, GetId(id));
                dbContext.Set<T>().Attach(entity);
                dbContext.Entry(entity).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
                return entity;
            }
        }

        public async Task<T> Get<T>(string id) where T : class
        {
            using (var dbContext = DbContext())
            {
                return await dbContext.Set<T>().FindAsync(GetId(id));
            }
        }

        public async Task<TChild> AddChild<TParent, TChild>(string parentId,
            Expression<Func<TParent, ICollection<TChild>>> property, TChild child)
            where TParent : class
            where TChild : class
        {
            using (var dbContext = DbContext())
            {
                var parent = (await dbContext.Set<TParent>().FindAsync(GetId(parentId)));
                dbContext.Set<TChild>().Attach(child);
                property.Compile()(parent).Add(child);
                await dbContext.SaveChangesAsync();
                return child;
            }
        }

        public async Task<bool> RemoveChild<TParent, TChild>(string parentId,
            Expression<Func<TParent, ICollection<TChild>>> property, TChild child) where TParent : class
            where TChild : class
        {
            using (var dbContext = DbContext())
            {
                var parent = (await dbContext.Set<TParent>().FindAsync(GetId(parentId)));
                dbContext.Set<TChild>().Attach(child);
                await dbContext.Entry(parent).Collection(property).LoadAsync();
                property.Compile()(parent).Remove(child);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<IEnumerable<TChild>> QueryChildren<TParent, TChild>(string parentId,
            Expression<Func<TParent, ICollection<TChild>>> property, Func<IQueryable<TChild>, IQueryable<TChild>> query)
            where TParent : class where TChild : class
        {
            using (var dbContext = DbContext())
            {
                var parent = (await dbContext.Set<TParent>().FindAsync(GetId(parentId)));
                return await query(dbContext.Entry(parent).Collection(property).Query()).ToListAsync();
            }
        }

        private object GetId(string id)
        {
            int intId;
            if (int.TryParse(id, out intId))
            {
                return intId;
            }
            Guid guidId;
            if (Guid.TryParse(id, out guidId))
            {
                return guidId;
            }

            return id;
        }
    }
}