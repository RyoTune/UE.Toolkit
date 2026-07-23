using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class UInt16Param(Ptr<ushort> rvalue, IUnrealMemoryInternal? memory = null) 
    : FunctionParamCopyable<ushort>(rvalue, "UInt16Property", memory);