using BulkyBook.DataAccess.Repository.IRepository;
using System;

namespace BulkyBook.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        public ICategoryRepository Categories { get; }
        public ICoverTypeRepository CoverTypes { get; }
        public IStoredProcedureCall StoredProcedureCalls { get; }

        public void Save();
    }
}
