using System.IO;
using System.Linq.Expressions;
using LiteDB;

namespace Gimmi.App;

public static class DbRepository
{
    private static object lock_instance = new object();
    private static object lock_stack = new object();
    private static List<DbStackDefinition> stackAction = new List<DbStackDefinition>();

    public static string FileName { get; } = Path.Combine(Assistant.ApplicationPath, "data.db");

    public static void GetCollection<T>(Action<ILiteCollection<T>> action, LiteDatabase db = null)
    {
        InTransaction((_) =>
        {
            var collection = _.GetCollection<T>();
            action(collection);
        }, db);
    }

    public static T[] FindAll<T>(LiteDatabase db = null)
    {
        T[] array = Array.Empty<T>();
        GetCollection<T>((collection) =>
        {
            array = collection.FindAll().ToArray();
        }, db);
        return array;
    }
    
    public static T[] Find<T>(Expression<Func<T, bool>> predicate, LiteDatabase db = null)
    {
        T[] array = Array.Empty<T>();
        GetCollection<T>((collection) =>
        {
            array = collection.Find(predicate).ToArray();
        }, db);
        return array;
    }
    
    public static T FindOne<T>(Expression<Func<T, bool>> predicate, LiteDatabase db = null)
    {
        T instance = default(T);
        GetCollection<T>((collection) =>
        {
            instance = collection.FindOne(predicate);
        }, db);
        return instance;
    }
    
    public static T FindById<T>(BsonValue id, LiteDatabase db = null)
    {
        T instance = default(T);
        GetCollection<T>((collection) =>
        {
            instance = collection.FindById(id);
        }, db);
        return instance;
    }

    public static void InTransaction(Action<LiteDatabase> action, LiteDatabase db = null)
    {
        if (db == null)
        {
            lock (lock_instance)
            {
                using (var _ = new LiteDatabase(FileName))
                {
                    action(_);
                }
            }
        }
        else
        {
            action(db);
        }
    }

    public static void AddToStack(object data, Action<LiteDatabase, object> action)
    {
        lock (lock_stack)
        {
            stackAction.Add(new DbStackDefinition()
            {
                data = data,
                action = action
            });
        }
    }
    
    public static void Flush()
    {
        lock (lock_stack)
        {
            if (stackAction.Count < 1)
                return;

            InTransaction((db) =>
            {
                foreach (var element in stackAction)
                {
                    element?.action?.Invoke(db, element.data);
                }
            });
            
            stackAction.Clear();
        }
    }
}

internal class DbStackDefinition
{
    public object data;
    public Action<LiteDatabase, object> action;
}