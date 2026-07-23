using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

public interface IUObject : IPtr
{
    nint VTable { get; }
    
    EObjectFlags ObjectFlags { get; }
    
    int InternalIndex { get; }
    
    IUClass ClassPrivate { get; }
    
    FName NamePrivate { get; }
    
    IUObject? OuterPrivate { get; }

    bool IsChildOf(string type);

    bool IsA(string type) => IsChildOf(type);

    bool IsChildOf<T>() => IsChildOf(typeof(T).Name);

    bool IsA<T>() => IsChildOf<T>();

    IUObject GetOutermost();

    string GetNativeName();

    string GetPathName();

    // IUObject GetWorld();

    ProcessEventResult ProcessEvent(string Name, List<IFunctionParam> Params, out IFunctionParam? Return);

    IUnrealFactory GetFactory();
}

public abstract unsafe class BaseUObject<TUObjectBase>(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory) 
    : IUObject where TUObjectBase : unmanaged
{
    protected readonly TUObjectBase* _self = (TUObjectBase*)ptr;
    protected readonly IUnrealFactory _factory = factory;
    protected readonly IUnrealMemoryInternal _memory = memory;

    public IUnrealFactory GetFactory() => _factory;
    
    public nint Ptr => (nint)_self;
    
    public abstract nint VTable { get; }
    public abstract EObjectFlags ObjectFlags { get; }
    public abstract int InternalIndex { get; }
    public abstract IUClass ClassPrivate { get; }
    public abstract FName NamePrivate { get; }
    public abstract IUObject? OuterPrivate { get; }

    public abstract bool IsChildOf(string type);

    public abstract IUObject GetOutermost();

    public abstract string GetNativeName();

    public abstract string GetPathName();

    public ProcessEventResult ProcessEvent(string Name, List<IFunctionParam> Params, out IFunctionParam? Return)
    {
        Return = null;
        var Function = ClassPrivate.GetFunction(Name);
        if (Function == null)
        {
            return ProcessEventResult.CouldNotFindFunction;
        }
        var Alloc = Function.GetTotalParameterSize() switch
        {
            0 => nint.Zero, var SizeOf => _memory.Malloc(SizeOf)
        };
        foreach (var (Index, Field) in Function.ChildProperties.Select((x, i) => (i, x)))
        {
            var Property = _factory.CreateFProperty(Field.Ptr);
            if (Property.PropertyFlags.HasFlag(EPropertyFlags.CPF_ReturnParm))
            {
                NativeMemory.Clear((void*)(Alloc + Property.Offset_Internal), (nuint)Property.ElementSize);
                break;
            }
            var Parameter = Params[Index];
            if (Property.ClassPrivate.Name != Parameter.PropertyType)
            {
                return ProcessEventResult.ParameterTypeMismatch;
            }
            Parameter.Write(Alloc + Property.Offset_Internal);
        }
        _factory.ProcessEvent!(Ptr, Function, Alloc);
        foreach (var (Index, Field) in Function.ChildProperties.Select((x, i) => (i, x)))
        {
            var Property = _factory.CreateFProperty(Field.Ptr);
            if (Property.PropertyFlags.HasFlag(EPropertyFlags.CPF_ReturnParm))
            {
                Return = _factory.CreateReturnParam!(Property);
                Return.Read(Alloc + Property.Offset_Internal);
                break;
            }
            if (Property.PropertyFlags.HasFlag(EPropertyFlags.CPF_OutParm))
            {
                Params[Index].Read(Alloc + Property.Offset_Internal);
            }
        }
        if (Alloc != nint.Zero)
        {
            _memory.Free(Alloc);
        }
        return ProcessEventResult.Success;
    }
}