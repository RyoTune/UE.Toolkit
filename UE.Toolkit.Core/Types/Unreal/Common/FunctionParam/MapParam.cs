using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class MapParam(Ptr<TMap<int, int>> rvalue, IUnrealMemoryInternal? memory = null)
    : FunctionParamCopyable<TMap<int, int>>(rvalue, "MapProperty", memory);