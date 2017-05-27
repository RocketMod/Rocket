using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using Rocket.API.Providers.Database;

namespace Rocket.Core.Providers.Database
{
    public class AdoNetEntityDatabaseProvider : IDatabaseProvider
    {
        public List<DatabaseContext> DatabaseContexts { get; } = new List<DatabaseContext>();
        public void Unload(bool isReload = false)
        {
            if (Connection == null || Connection.State == ConnectionState.Closed)
                return;

            Connection.Close();
        }

        public void Load(bool isReload = false)
        {
            Connection.Open();
        }

        public void Save()
        {
            foreach(var ctx in DatabaseContexts)
                ctx.SubmitChanges();
        }

        public void Setup(string connectionString)
        {
            Connection = new EntityConnection(connectionString);
        }

        public IQueryResult InitializeContext(DatabaseContext context)
        {
            if(!context.DatabaseExists())
                context.CreateDatabase();
            
            /*
            foreach (var property in context.GetType().GetProperties(BindingFlags.Instance|BindingFlags.Public))
            {
                var propType = property.PropertyType;
                if(!typeof(Table<>).IsAssignableFrom(propType))
                    continue;

                var serializerClass = propType.GetGenericArguments()[0];

                string tableName = property.Name;
                var attribute = (TableAttribute) property.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
                if (attribute != null)
                {
                    tableName = attribute.Name;
                }

                if(TableExists(tableName))
                    continue;

                var result = CreateTable(tableName, serializerClass);
                if (result.State != QueryState.SUCCESS)
                {
                    return result;
                }

                property.SetValue(context, Activator.CreateInstance(propType, context), null);
            }
            */

            DatabaseContexts.Add(context);
            return new AdoNetQueryResult(QueryState.SUCCESS);
        }

        /*
        private IQueryResult CreateTable(string tableName, Type serializerClass)
        {
            try
            {
                var datatable = new DataTable(tableName);
                foreach (var property in serializerClass.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.GetGetMethod() == null || property.GetSetMethod() == null)
                    {
                        throw new Exception("Property: " + property.Name + " in class " + serializerClass.FullName +
                                            " is missing getter or setter!");
                    }
                    if (!property.GetGetMethod().IsVirtual || !property.GetSetMethod().IsVirtual)
                    {
                        throw new Exception("Property: " + property.Name + " in class " + serializerClass.FullName +
                                            " has non virtual getter or setter!");
                    }

                    string columnName = property.Name;
                    var attribute =
                        (ColumnAttribute) property.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();
                    if (attribute != null)
                    {
                        columnName = attribute.Name;
                    }

                    datatable.Columns.Add(columnName);
                }

                
            }
            catch (Exception e)
            {
                return new AdoNetQueryResult(QueryState.EXCEPTION) { Exception = e};
            }
        }
        */

        public bool TableExists(string name)
        {
            DataTable dTable = ((SqlConnection)Connection).GetSchema("TABLES", new[] { null, null, name});

            return dTable.Rows.Count > 0;
        }

        public IDbConnection Connection { get; private set; }
    }
}
