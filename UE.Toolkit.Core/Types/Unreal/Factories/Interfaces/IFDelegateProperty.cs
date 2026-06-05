namespace UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

public interface IFDelegateProperty : IFProperty
{
    IUFunction? Function { get; }
}