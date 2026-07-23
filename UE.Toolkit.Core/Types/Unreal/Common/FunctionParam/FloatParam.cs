using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class FloatParam(Ptr<float> rvalue, IUnrealMemoryInternal? memory = null) 
    : FunctionParamCopyable<float>(rvalue, "FloatProperty", memory);