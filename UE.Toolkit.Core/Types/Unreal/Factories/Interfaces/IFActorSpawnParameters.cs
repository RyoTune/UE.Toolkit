using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

public interface IFActorSpawnParameters : IPtr
{
    void SetParams(EObjectFlags Flags);
    void SetParams(FName Name, EObjectFlags Flags);
}