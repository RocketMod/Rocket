** OUTDATED **
This page has not been updated for RocketMod 5. It will be updated once RocketMod 5 supports Databases.

Rocket has an ORM API (based on Linq to Sql) so plugins do not have to deal with database connections.
The API is abstract and all you need to care about is creating some classes. 
Connections, credentials etc are all managed by Rocket.

# Creating Contexts
First create a class which extens `DatabaseContext`.
After that, add your tables as public properties like shown below (see below on how to create tables).

```cs
public MyDatabaseContext : DatabaseContext
{
    public MyDatabaseContext(IRocketPlugin plugin, IDatabaseProvider provider) : base(plugin, provider)
    {
    }
}
```

You can initialize a DatabaseContext like this:
```cs
IDatabaseProvider provider = R.Providers.GetProvider<IDatabaseProvider>();
MyDatabaseContext context = new MyDatabaseContext(plugin, provider);
var result = provider.InitializeContext(context);
if(result.State == ContextInitializationState.CONNECTED)
{
    //connected & created tables successfully
}
```

# Tables
After you have got a database context, you can create a table. Just add a `public Table<MyTable> MyTable => GetTable<MyTable>();` to your context class. A context can have multiple tables.

```cs
public MyDatabaseContext : DatabaseContext
{
    public MyDatabaseContext(IRocketPlugin plugin, IDatabaseProvider provider) : base(plugin, provider)
    {
    }

    public Table<MyTable> MyTable => GetTable<MyTable>();
    public Table<PlayerScore> PlayerScores => GetTable<PlayerScore>();
}
```
Create a class which extends database and has properties setup like this:
```cs
[Table]
public class PlayerScore: DatabaseTable
{
     [Column]
     public virtual string PlayerName
     {
        set { Set(value, nameof(PlayerName)); }
        get { return Get<string>(nameof(PlayerName)); }
     }

     [Column]
     public virtual int Score
     {
        set { Set(value, nameof(Score)); }
        get { return Get<int>(nameof(Score)); }
     }  
}
```

# Queries
To do queries, you can use LINQ.
```
MyDatabaseContext context...;
int score = context.PlayerScores.First(c => c.PlayerName == "Rocket").Select(c => c.Score);
```