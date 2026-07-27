namespace UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

public interface IFFieldVariant : IPtr
{
    IFField? Field { get; }
    IUObject? Object { get; }
    bool IsObject { get; }
}