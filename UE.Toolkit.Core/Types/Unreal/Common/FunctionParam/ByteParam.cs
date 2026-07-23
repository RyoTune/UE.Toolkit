using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class ByteParam(Ptr<byte> rvalue, IUnrealMemoryInternal? memory = null)
    : FunctionParamCopyable<byte>(rvalue, "ByteProperty", memory);