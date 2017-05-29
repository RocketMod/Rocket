using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Harmony;
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
            HarmonyInstance instance = HarmonyInstance.Create(serializerClass.FullName);
            foreach (var property in serializerClass.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (serializerClass.GetInterface(nameof(INotifyPropertyChanged)) == null)
                {
                //    throw new Exception("Class " + serializerClass.FullName + " is missing INotifyPropertyChanged interface");
                }

                if (serializerClass.GetInterface(nameof(INotifyPropertyChanging)) == null)
                {
                //    throw new Exception("Class " + serializerClass.FullName + " is missing INotifyPropertyChanging interface");
                }

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

                PatchProperty(instance, property);
            }
        }

        private void PatchProperty(HarmonyInstance instance, PropertyInfo property)
        {
            var method = property.GetGetMethod();

            var prefix = typeof(AdoNetEntityDatabaseProvider).GetMethod("Prefix",
                BindingFlags.Static | BindingFlags.NonPublic);


            var postfix = typeof(AdoNetEntityDatabaseProvider).GetMethod("Postfix",
                BindingFlags.Static | BindingFlags.NonPublic);

            HarmonyMethod preM = new HarmonyMethod(prefix);
            HarmonyMethod postM = new HarmonyMethod(postfix);

            instance.Patch(method, preM, postM);
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
