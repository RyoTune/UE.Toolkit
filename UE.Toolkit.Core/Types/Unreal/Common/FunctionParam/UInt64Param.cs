using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class UInt64Param(Ptr<ulong> rvalue, IUnrealMemoryInternal? memory = null) 
    : FunctionParamCopyable<ulong>(rvalue, "UInt64Property", memory);