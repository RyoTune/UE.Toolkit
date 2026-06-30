using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Interfaces;

namespace UE.Toolkit.Reloaded.Reflection.UE5_6_1;

public class TypeFactory(IUnrealFactory factory, IUnrealMemory memory, 
    IUnrealClasses classes, IPropertyFlagsBuilder flags) 
    : BaseTypeFactory(factory, memory, classes, flags)
{
    public override bool CreateI8Param(string Name, int Offset, out IFGenericPropertyParams? Out)
    {
        Out = null;
        return false;
    }
    
    public override bool CreateI16Param(string Name, int Offset, out IFGenericPropertyParams? Out)
    {
        Out = null;
        return false;
    }
    
    public override bool CreateI32Param(string Name, int Offset, out IFGenericPropertyParams? Out)
    {
        Out = null;
        return false;
    }
    
    public override bool CreateI64Param(string Name, int Offset, out IFGenericPropertyParams? Out)
    {
        Out = null;
        return false;
    }
    
    public override bool CreateU8Param(string Name, int Offset, out IFGenericPropertyParams? Out)
    {
        Out = null;
        return false;
    }
    
    public override bool CreateU16Param(string Name, int Offset, out IFGenericPropertyParams? Out)
    {
        Out = null;
        return false;
    }
    
    public override bool CreateU32Param(string Name, int Offset, out IFGenericPropertyParams? Out)
    {
        Out = null;
        return false;
    }
    
    public override bool CreateU64Param(string Name, int Offset, out IFGenericPropertyParams? Out)
    {
        Out = null;
        return false;
    }
    
    public override bool CreateF32Param(string Name, int Offset, out IFGenericPropertyParams? Out)
    {
        Out = null;
        return false;
    }
    
    public override bool CreateF64Param(string Name, int Offset, out IFGenericPropertyParams? Out)
    {
        Out = null;
        return false;
    }

    internal override unsafe bool CreateStructParam(string Name, int Size,
        List<IFPropertyParams> Fields, out IFStructParams? Out)
    {
        Out = null;
        return false;
    }
}