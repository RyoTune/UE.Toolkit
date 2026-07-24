using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class MulticastInlineDelegateParam(Ptr<FMulticastScriptDelegate> rvalue, IUnrealMemoryInternal? memory = null)
    : FunctionParamCopyable<FMulticastScriptDelegate>(rvalue, "MulticastInlineDelegateProperty", memory);
    
public class MulticastSparseDelegateParam(Ptr<FMulticastSparseDelegateProperty> rvalue, IUnrealMemoryInternal? memory = null)
    : FunctionParamCopyable<FMulticastSparseDelegateProperty>(rvalue, "MulticastSparseDelegateProperty", memory);