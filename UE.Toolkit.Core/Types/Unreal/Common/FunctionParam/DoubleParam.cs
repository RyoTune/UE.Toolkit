using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class DoubleParam(Ptr<double> rvalue, IUnrealMemoryInternal? memory = null) 
    : FunctionParamCopyable<double>(rvalue, "DoubleProperty", memory);