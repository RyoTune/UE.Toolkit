namespace UE.Toolkit.DumperMod.Definitions;

public interface IObjectFactory
{
    void Register();
}

public interface ISerializable
{
    string Serialize(Context context);
}

public abstract class BaseObjectFactory(Context context, ObjectType objectType) : IObjectFactory
{
    protected Context Context = context;
    protected ObjectType ObjectType = objectType;

    public abstract void Register();
}