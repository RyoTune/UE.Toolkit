using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class StringParam(Ptr<FString> rvalue, IUnrealMemoryInternal? memory = null)
    : FunctionParamCopyable<FString>(rvalue, "StrProperty", memory);