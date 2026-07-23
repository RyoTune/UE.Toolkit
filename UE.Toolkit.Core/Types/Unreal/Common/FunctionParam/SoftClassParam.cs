using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class SoftClassParam(Ptr<TSoftClassPtr<int>> rvalue, IUnrealMemoryInternal? memory = null)
    : FunctionParamCopyable<TSoftClassPtr<int>>(rvalue, "SoftClassProperty", memory);