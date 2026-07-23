using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class SoftObjectParam(Ptr<TSoftObjectPtr<int>> rvalue, IUnrealMemoryInternal? memory = null)
    : FunctionParamCopyable<TSoftObjectPtr<int>>(rvalue, "SoftObjectProperty", memory);