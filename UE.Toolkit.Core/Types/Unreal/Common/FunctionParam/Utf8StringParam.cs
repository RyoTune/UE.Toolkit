using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_6_1;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class Utf8StringParam(Ptr<FUtf8String> rvalue, IUnrealMemoryInternal? memory = null)
    : FunctionParamCopyable<FUtf8String>(rvalue, "Utf8StrProperty", memory);