using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Reflection;
using Rocket.API.Providers;
using Rocket.API.Providers.Database;

namespace Rocket.Core.Providers.Database
{
    public class AdoNetEntityDatabaseProvider : ProviderBase, IDatabaseProvider
    {
        public List<DatabaseContext> DatabaseContexts { get; } = new List<DatabaseContext>();
        protected override void OnUnload()
        {
            if (Connection == null || Connection.State == ConnectionState.Closed)
                return;

            Connection.Close();
        }

        protected override void OnLoad(ProviderManager providerManager)
        {
            Connection.Open();
        }

        public void Save()
        {
            foreach (var ctx in DatabaseContexts)
                ctx.SubmitChanges();
        }

        public void Setup(string connectionString)
        {
            Connection = new EntityConnection(connectionString);
        }

        public ContextInitializationResult InitializeContext(DatabaseContext context)
        {

            if ((context.Connection.State & ConnectionState.Open) != 0)
            {
                return new ContextInitializationResult(ContextInitializationState.NOT_CONNECTED);
            }

            try
            {
                if (!context.DatabaseExists())
                    context.CreateDatabase();
            }
            catch (Exception e)
            {
                return new ContextInitializationResult(ContextInitializationState.EXCEPTION) { Exception = e };
            }

            foreach (var property in context.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var propType = property.PropertyType;
                if (!typeof(Table<>).IsAssignableFrom(propType))
                    continue;

                var serializerClass = propType.GetGenericArguments()[0];
                if(!typeof(DatabaseTable).IsAssignableFrom(serializerClass))
                    throw new Exception(serializerClass.FullName + " does not extend " + typeof(DatabaseTable).FullName + "!");

                if (property.GetValue(context, null) == null)
                    property.SetValue(context, context.GetTable(serializerClass), null);

                //SetupProperties(serializerClass);
            }

            DatabaseContexts.Add(context);
            context.OnDatabaseCreated();

            return new ContextInitializationResult(ContextInitializationState.CONNECTED);
        }

        private void SetupProperties(Type serializerClass)
        {
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
            }
        }

        private static void Prefix(INotifyPropertyChanging __instance)
        {
            
        }

        private static void Postfix(INotifyPropertyChanged __instance)
        {
            
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
                return new ContextInitializationResult(ContextInitializationState.EXCEPTION) { Exception = e};
            }
        }
        */

        public bool TableExists(string name)
        {
            DataTable dTable = ((SqlConnection)Connection).GetSchema("TABLES", new[] { null, null, name });

            return dTable.Rows.Count > 0;
        }
        
        public IDbConnection Connection { get; private set; }
    }
}
