using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.SqlClient;
using System.Collections;

namespace ORM
{
    public class CommandGenerator
    {
        private readonly SqlConnection _connection;

        public CommandGenerator(SqlConnection connection)
        {
            _connection = connection;
        }

        public void GenerateInsertCommand(object item, Guid? parentPrimaryKey, IList<SqlCommand> commands)
        {
            var type = item.GetType();
            var tableName = type.Name;
            var columns = type.GetProperties(System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly);

            var simpleTypes = columns.Where(x => x.PropertyType.IsSimpleType()).ToList();
            var complexTypes = columns.Where(x => !x.PropertyType.IsSimpleType()).ToList();

            var columnNames = new StringBuilder("Id");
            var values = new StringBuilder("@Id");

            if (simpleTypes.Count > 0)
            {
                columnNames.Append(", ");
                values.Append(", ");
            }

            for (int i = 0; i< simpleTypes.Count; i++)
            {
                columnNames.Append(simpleTypes[i].Name);
                values.Append("@").Append(simpleTypes[i].Name);

                if (i < simpleTypes.Count - 1)
                {
                    columnNames.Append(", ");
                    values.Append(", ");
                }
            }

            if (parentPrimaryKey.HasValue)
            {
                columnNames.Append(", ParentId");
                values.Append(", @ParentId");
            }

            var sql = $"Insert into {tableName} ({columnNames}) Values ({values});";

            var id = Guid.NewGuid();
            
            var command = new SqlCommand(sql, _connection);
            command.Parameters.AddWithValue("@Id", id);

            foreach(var column in simpleTypes)
            {
                command.Parameters.AddWithValue("@" + column.Name, column.GetValue(item));
            }

            if (parentPrimaryKey.HasValue)
                command.Parameters.AddWithValue("@ParentId", parentPrimaryKey.Value);

            commands.Add(command);

            foreach (var complexType in complexTypes)
            {
                var childItem = complexType.GetValue(item);

                if (childItem.GetType().GetInterfaces().Any(x => x.Name == "IEnumerable"))
                {
                    var childCollection = (IEnumerable)childItem;
                    foreach(var childItemFromCollection in childCollection)
                    {
                        GenerateInsertCommand(childItemFromCollection, id, commands);
                    }
                }
                else
                    GenerateInsertCommand(childItem, id, commands);
            }
        }

        public SqlCommand GenerateGetCommand(Type type, Guid? parentPrimaryKey = null)
        {
            var tableName = type.Name;
            var columns = type.GetProperties(System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly);

            var simpleTypes = columns.Where(x => x.PropertyType.IsSimpleType()).ToList();
            var complexTypes = columns.Where(x => !x.PropertyType.IsSimpleType()).ToList();

            var sql = $"Select * from {tableName}";

            if (parentPrimaryKey.HasValue)
                sql += " Where ParentId = @ParentId";

            var command = new SqlCommand(sql);

            if (parentPrimaryKey.HasValue)
                command.Parameters.AddWithValue("@ParentId", parentPrimaryKey.Value);

            return command;
        }
    }
}
