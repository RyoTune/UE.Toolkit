namespace UE.Toolkit.DumperMod.Definitions;

public interface IObjectFactory
{
    void Register();
}

public interface ISerializable
{
    string Serialize(Context context);
}

public abstract class BaseObjectFactory(Context context) : IObjectFactory
{
    protected Context Context = context;

    public abstract void Register();
}