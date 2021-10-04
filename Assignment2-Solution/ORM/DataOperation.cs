using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ORM
{
    public class DataOperation<T> where T:Entity
    {
        private readonly SqlConnection _connection;
        private readonly string _connectionString;
        private readonly CommandGenerator _commandGenerator;

        public DataOperation(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(connectionString);
            _commandGenerator = new CommandGenerator(_connection);
        }

        public void Insert(T item)
        {
            var commands = new List<SqlCommand>();
            
            _commandGenerator.GenerateInsertCommand(item, null, commands);

            if (_connection.State == System.Data.ConnectionState.Closed)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();
            try
            {
                foreach (var command in commands)
                {
                    command.Transaction = transaction;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
            }
        }

        public IList<T> GetAll()
        {
            return (IList<T>)LoadData(typeof(T));
        }

        private object LoadData(Type type, Guid? parentId = null)
        {
            var command = _commandGenerator.GenerateGetCommand(type, parentId);
            var connection = new SqlConnection(_connectionString);
            command.Connection = connection;

            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();

            var reader = command.ExecuteReader();
            var listType = typeof(List<>);
            var genericType = listType.MakeGenericType(type);
            var data = genericType.GetConstructor(new Type[] { }).Invoke(new object[]{ });

            while (reader.Read())
            {
                var constructor = type.GetConstructor(new Type[] { });
                var instance = constructor.Invoke(new Type[] { });

                Guid? id = null;
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var columnValue = reader.GetValue(i);

                    if (columnName == "Id")
                        id = (Guid)columnValue;

                    var property = type.GetProperty(columnName);
                    property?.SetValue(instance, columnValue);
                }

                var columns = type.GetProperties(System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly);

                var complexTypes = columns.Where(x => !x.PropertyType.IsSimpleType()).ToList();

                foreach (var complexType in complexTypes)
                {
                    object childData = null;
                    
                    if (complexType.PropertyType.IsGenericType)
                        childData = LoadData(complexType.PropertyType.GetGenericArguments().First(), id);
                    else
                        childData = LoadData(complexType.PropertyType, id);

                    if (complexType.PropertyType.GetInterfaces().Any(x => x.Name == "IList"))
                    {
                        complexType.SetValue(instance, childData);
                    }
                    else
                    {
                        var collection = ((IList)childData);
                        complexType.SetValue(instance, collection != null
                            && collection.Count >= 0 ? collection[0] : null);
                    }
                }

                var addMethod = genericType.GetMethod("Add");
                addMethod.Invoke(data, new object[] { instance });
            }

            return data;
        }
    }
}
