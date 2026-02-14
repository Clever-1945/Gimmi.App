using LiteDB;

namespace Gimmi.App.SettingsApp;

public class Entity<T>: IEntity where T : IEntity
{
    public Guid Id { get; set; }

    public void Update(LiteDatabase db = null)
    {
        DbRepository.GetCollection<T>((collection) =>
        {
            collection.Update(this.Id, (T)(object)this);
        }, db);
    }

    public void Insert(LiteDatabase db = null)
    {
        DbRepository.GetCollection<T>((collection) =>
        {
            collection.Insert((T)(object)this);
        }, db);
    }
}