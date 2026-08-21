namespace UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

public interface IICppStructOps : IPtr
{
    nint VTable { get; }
    int Size { get; }
    int Alignment { get; }

    nint Construct();
}