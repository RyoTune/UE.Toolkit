using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_6_1;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class AnsiStringParam(Ptr<FAnsiString> rvalue, IUnrealMemoryInternal? memory = null)
    : FunctionParamCopyable<FAnsiString>(rvalue, "AnsiStrProperty", memory);