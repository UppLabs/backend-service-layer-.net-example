using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Evalx.Core.DataAccess;
using Evalx.Core.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.SqlServer.Server;

namespace Evalx.Identity.DomainLogic
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, IEntity<int>
    {
        protected readonly IUnitOfWork UnitOfWork;

        private readonly IBaseService<TEntity, int> _baseService;

        public BaseService(IUnitOfWork unitOfWork, IBaseService<TEntity, int> baseService)
        {
            UnitOfWork = unitOfWork;
            _baseService = baseService;
        }

        public BaseService(IUnitOfWork unitOfWork)
            : this(unitOfWork, new BaseService<TEntity, int>(unitOfWork))
        {
        }


        public virtual TEntity Get(int id)
        {
            return UnitOfWork.Get<TEntity>().FirstOrDefault(e => e.Id == id);
        }

        public virtual TEntity Create(TEntity newItem, bool shouldBeCommited = true)
        {
            return _baseService.Create(newItem, shouldBeCommited);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return _baseService.GetAll();
        }

        public virtual void Remove(TEntity removeItem, bool shouldBeCommited = true)
        {
            _baseService.Remove(removeItem, shouldBeCommited);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> removeItems, bool shouldBeCommited = true)
        {
            _baseService.RemoveRange(removeItems, shouldBeCommited);
        }

        public virtual void RemoveRange(IEnumerable<int> removeItemIds, bool shouldBeCommited = true)
        {
            var items = GetAll().Where(i => removeItemIds.Contains(i.Id)).ToList();
            RemoveRange(items, shouldBeCommited);
        }

        public virtual void Remove(int id, bool shouldBecomminted = true)
        {
            var item = Get(id);
            Remove(item, shouldBecomminted);
        }

        public virtual void Commit(bool saveChanges = true)
        {
            _baseService.Commit(saveChanges);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _baseService.BeginTransaction();
        }
    }

    public class BaseService<TEntity, TPrimaryId> : IBaseService<TEntity, TPrimaryId>
        where TEntity : class, IEntity<TPrimaryId>
    {
        protected readonly IUnitOfWork UnitOfWork;


        public BaseService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public void Commit(bool saveChanges = true)
        {
            if (saveChanges)
                UnitOfWork.Commit();
        }

        public virtual TEntity Get(TPrimaryId id)
        {
            throw new NotImplementedException();
        }

        public virtual TEntity Create(TEntity newItem, bool shouldBeCommited = true)
        {
            UnitOfWork.Add(newItem);
            Commit(shouldBeCommited);
            return newItem;
        }

        public virtual void Remove(TEntity removeItem, bool shouldBeCommited = true)
        {
            if (removeItem != null)
            {
                UnitOfWork.Remove(removeItem);
                Commit(shouldBeCommited);
            }
        }

        public void RemoveRange(IEnumerable<TEntity> removeItems, bool shouldBeCommited = true)
        {
            UnitOfWork.RemoveRange(removeItems);
            Commit(shouldBeCommited);
        }

        public void RemoveRange(IEnumerable<TPrimaryId> removeItemIds, bool shouldBeCommited = true)
        {
            var items = GetAll().Where(i => removeItemIds.Contains(i.Id)).ToList();
            RemoveRange(items);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return GetList<TEntity>();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return UnitOfWork.GetContext().Database.BeginTransaction();
        }

        protected virtual IQueryable<T> GetList<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var list = GetList<T>();
            if (predicate != null)
            {
                return list.Where(predicate);
            }
            return list;
        }

        protected virtual IQueryable<T> GetList<T, TKey>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, TKey>> orderBy) where T : class
        {
            return GetList(predicate).OrderBy(orderBy);
        }

        protected virtual IQueryable<T> GetList<T, TKey>(Expression<Func<T, TKey>> orderBy) where T : class
        {
            return GetList<T>().OrderBy(orderBy);
        }

        protected virtual IQueryable<T> GetList<T>() where T : class
        {
            return UnitOfWork.Get<T>();
        }

    }
}