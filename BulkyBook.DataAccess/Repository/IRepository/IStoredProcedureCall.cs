using Dapper;
using System;
using System.Collections.Generic;

namespace BulkyBook.DataAccess.Repository.IRepository;

public interface IStoredProcedureCall : IDisposable
{
    T Single<T>(string procedureName, DynamicParameters parameters = null);

    void Execute(string procedureName, DynamicParameters parameters = null);

    T OneRecord<T>(string procedureName, DynamicParameters parameters = null);

    IEnumerable<T> List<T>(string procedureName, DynamicParameters parameters = null);

    (IEnumerable<T1>, IEnumerable<T2>) List<T1, T2>(string procedureName, DynamicParameters parameters = null);
}