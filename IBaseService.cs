using System.Collections.Generic;
using System.Linq;
using Evalx.Core.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Evalx.Identity.DomainLogic
{
    public interface IBaseService<T> where T: class
    {
        T Get(int id);
        T Create(T newItem, bool shouldBeCommited = true);
        IQueryable<T> GetAll();
        void Remove(T removeItem, bool shouldBeCommited = true);
        void RemoveRange(IEnumerable<T> removeItems, bool shouldBeCommited = true);
        void RemoveRange(IEnumerable<int> removeItemIds, bool shouldBeCommited = true);
        void Remove(int id, bool shouldBecomminted = true);
        void Commit(bool saveChanges = true);
        IDbContextTransaction BeginTransaction();
    }

    public interface IBaseService<TEntity,  TPrimaryId> where TEntity: class , IEntity<TPrimaryId>
    {
        TEntity Get(TPrimaryId id);
        TEntity Create(TEntity newItem, bool shouldBeCommited = true);
        IQueryable<TEntity> GetAll();
        void Remove(TEntity removeItem, bool shouldBeCommited = true);
        void RemoveRange(IEnumerable<TEntity> removeItems, bool shouldBeCommited = true);
        void RemoveRange(IEnumerable<TPrimaryId> removeItemIds, bool shouldBeCommited = true);
        void Commit(bool saveChanges = true);
        IDbContextTransaction BeginTransaction();
    }
}