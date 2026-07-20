using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class IntParam(Ptr<int> rvalue, IUnrealMemoryInternal? memory = null) 
    : FunctionParamCopyable<int>(rvalue, "IntProperty", memory);