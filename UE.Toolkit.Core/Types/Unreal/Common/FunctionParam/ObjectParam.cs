using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class ObjectParam(Ptr<nint> rvalue, IUnrealMemoryInternal? memory = null) 
    : FunctionParamCopyable<nint>(rvalue, "ObjectProperty", memory);