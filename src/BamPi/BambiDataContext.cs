using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BamPi
{
    public interface IBamPiDataContext
    {
        Task<IEnumerable<T>> Query<T>(Func<IQueryable<T>, IQueryable<T>> query) where T : class;
        Task<bool> Delete<T>(string id) where T : class;
        Task<T> Add<T>(T entity) where T : class;
        Task<T> Update<T>(string id, T entity) where T : class;
        Task<T> Get<T>(string id) where T : class;

        Task<TChild> AddChild<TParent, TChild>(string parentId,
            Expression<Func<TParent, ICollection<TChild>>> property, TChild child)
            where TParent : class
            where TChild : class;

        Task<bool> RemoveChild<TParent, TChild>(string parentId,
            Expression<Func<TParent, ICollection<TChild>>> property, TChild child)
            where TParent : class
            where TChild : class;

        Task<IEnumerable<TChild>> QueryChildren<TParent, TChild>(string parentId,
            Expression<Func<TParent, ICollection<TChild>>> property, Func<IQueryable<TChild>, IQueryable<TChild>> query)
            where TParent : class
            where TChild : class;
    }
}