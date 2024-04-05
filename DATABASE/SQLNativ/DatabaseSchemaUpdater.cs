using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DATABASE.SQLNativ
{
    public class DatabaseSchemaUpdater
    {
        private readonly string _connectionString;

        public DatabaseSchemaUpdater(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task EnsureFieldExists(string tableName, string fieldName, string fieldType)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var columnExists = await CheckIfColumnExists(connection, tableName, fieldName);

                if (!columnExists)
                {
                    var commandText = $"ALTER TABLE {tableName} ADD {fieldName} {fieldType};";
                    using (var command = new SqlCommand(commandText, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        internal async Task EnsureFieldExists<T>(string tableName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // Check if the table exists
                if (!await CheckIfTableExists(connection, tableName))
                {
                    // Create the table if it doesn't exist
                    await CreateTable<T>(connection, tableName);
                }
                foreach (PropertyInfo prop in typeof(T).GetProperties())
                {
                    // Skip properties that are not both readable and writable
                    if (prop.GetGetMethod() == null || prop.GetSetMethod() == null ||
                        !prop.GetGetMethod().IsPublic || !prop.GetSetMethod().IsPublic)
                    {
                        continue;
                    }


                    if (Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
                    {
                        continue;
                    }
                    var columnName = prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;
                    var columnType = GetSqlDataType(prop.PropertyType.Name, prop.PropertyType.BaseType.Name);

                    var columnExists = await CheckIfColumnExists(connection, tableName, columnName);
                    if (!columnExists)
                    {
                        var commandText = $"ALTER TABLE {tableName} ADD {columnName} {columnType};";
                        using (var command = new SqlCommand(commandText, connection))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                        Console.WriteLine($"column name:{columnName}, type:{columnType} now is added!");

                    }
                }
            }
        }


        private string GetSqlDataType(string csharpTypeName, string baseTypeName)
        {
            var typeMappings = new Dictionary<string, string>
        {
            {"Boolean", "bit"},
            {"Byte", "tinyint"},
            {"Int16", "smallint"},
            {"Int32", "int"},
            {"Int64", "bigint"},
            {"Single", "real"},
            {"Double", "float"},
            {"Decimal", "decimal"},
            {"String", "nvarchar(max)"},
            {"Char", "nchar(1)"},
            {"DateTime", "datetime2"},
            {"DateTimeOffset", "datetimeoffset"},
            {"Byte[]", "varbinary(max)"},
            {"Guid", "uniqueidentifier"},
            {"Object", "sql_variant"},
            {"TimeSpan", "time"}
        };
            // Enum types should be mapped to int (or another appropriate integer type)
            if (baseTypeName == "Enum")
            {
                return "int";
            }
            if (typeMappings.TryGetValue(csharpTypeName, out string sqlType))
            {
                return sqlType;
            }

            throw new NotSupportedException($"The C# type '{csharpTypeName}' is not supported.");
        }
        private async Task<bool> CheckIfColumnExists(SqlConnection connection, string tableName, string columnName)
        {
            var commandText = $"SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{columnName}'";

            using (var command = new SqlCommand(commandText, connection))
            {
                var result = await command.ExecuteScalarAsync();
                return result != null;
            }
        }


        private async Task<bool> CheckIfTableExists(SqlConnection connection, string tableName)
        {
            var commandText = $"SELECT CASE WHEN OBJECT_ID(N'{tableName}', N'U') IS NOT NULL THEN 1 ELSE 0 END";
            using (var command = new SqlCommand(commandText, connection))
            {
                return (int)await command.ExecuteScalarAsync() == 1;
            }
        }

        private async Task CreateTable<T>(SqlConnection connection, string tableName)
        {
            var commandText = $"CREATE TABLE {tableName} (";
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetGetMethod().IsPublic && p.GetSetMethod().IsPublic &&
                            !Attribute.IsDefined(p, typeof(NotMappedAttribute)) );

            foreach (var prop in properties)
            {
                var columnName = prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;
                var columnType = GetSqlDataType(prop.PropertyType.Name, prop.PropertyType.BaseType?.Name);
                commandText += $"[{columnName}] {columnType},";
            }

            commandText = commandText.TrimEnd(',') + ");";

            using (var command = new SqlCommand(commandText, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
