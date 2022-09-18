using BulkyBook.DataAccess.Repository.IRepository;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BulkyBook.DataAccess.Repository;

public class StoredProcedureCall : IStoredProcedureCall
{
    private readonly DbContext _db;
    private readonly string _connectionString = null;

    public StoredProcedureCall(DbContext db)
    {
        _db = db;
        _connectionString = _db.Database.GetConnectionString();
    }

    public void Dispose()
    {
        _db?.Dispose();
    }

    public void Execute(string procedureName, DynamicParameters parameters = null)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        connection.Execute(procedureName, parameters, commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<T> List<T>(string procedureName, DynamicParameters parameters = null)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection.Query<T>(procedureName, parameters, commandType: CommandType.StoredProcedure).ToList();
    }

    public (IEnumerable<T1>, IEnumerable<T2>) List<T1, T2>(string procedureName, DynamicParameters parameters = null)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        var result = SqlMapper.QueryMultiple(connection, procedureName, parameters, commandType: CommandType.StoredProcedure);
        var t1 = result.Read<T1>().ToList();
        var t2 = result.Read<T2>().ToList();

        return (t1 ?? new List<T1>(), t2 ?? new List<T2>());
    }

    public T OneRecord<T>(string procedureName, DynamicParameters parameters = null)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        var value = connection.Query<T>(procedureName, parameters, commandType: CommandType.StoredProcedure).ToList();
        return (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T));
    }

    public T Single<T>(string procedureName, DynamicParameters parameters = null)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        var value = connection.ExecuteScalar<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        return (T)Convert.ChangeType(value, typeof(T));
    }
}