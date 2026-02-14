using LiteDB;

namespace Gimmi.App.SettingsApp;

public interface IEntity
{
    Guid Id { set; get; }

    void Update(LiteDatabase db = null);

    void Insert(LiteDatabase db = null);
}