using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using EPropertyFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EPropertyFlags;
using FName = UE.Toolkit.Core.Types.Unreal.UE5_4_4.FName;
using FString = UE.Toolkit.Core.Types.Unreal.UE5_4_4.FString;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Common;

namespace UE.Toolkit.Reloaded.Unreal;

public class I8InvocationParameter(sbyte value) : InvocationParameterCopyable<sbyte>(value)
{
    public override string ExpectedPropertyClass => "Int8Property";
}

public class I16InvocationParameter(short value) : InvocationParameterCopyable<short>(value)
{
    public override string ExpectedPropertyClass => "Int16Property";
}

public class I32InvocationParameter(int value) : InvocationParameterCopyable<int>(value)
{
    public override string ExpectedPropertyClass => "IntProperty";
}

public class I64InvocationParameter(long value) : InvocationParameterCopyable<long>(value)
{
    public override string ExpectedPropertyClass => "Int64Property";
}

public class U8InvocationParameter(byte value) : InvocationParameterCopyable<byte>(value)
{
    public override string ExpectedPropertyClass => "Uint8Property";
}

public class U16InvocationParameter(ushort value) : InvocationParameterCopyable<ushort>(value)
{
    public override string ExpectedPropertyClass => "Uint16Property";
}

public class U32InvocationParameter(uint value) : InvocationParameterCopyable<uint>(value)
{
    public override string ExpectedPropertyClass => "Uint32Property";
}

public class U64InvocationParameter(ulong value) : InvocationParameterCopyable<ulong>(value)
{
    public override string ExpectedPropertyClass => "Uint64Property";
}

public class F32InvocationParameter(float value) : InvocationParameterCopyable<float>(value)
{
    public override string ExpectedPropertyClass => "FloatProperty";
}

public class F64InvocationParameter(double value) : InvocationParameterCopyable<double>(value)
{
    public override string ExpectedPropertyClass => "DoubleProperty";
}

public class NameInvocationParameter(FName value) : InvocationParameterCopyable<FName>(value)
{
    public override string ExpectedPropertyClass => "NameProperty";
}

public class StringInvocationParameter(string value, IUnrealMemory memory, IUnrealObjects objects) 
    : IInvocationParameter, IDisposable
{
    private IUnrealMemory _memory = memory;
    private IUnrealObjects _objects = objects;
    
    private string InnerValue { get; set; } = value;
    public object? Value => InnerValue;

    // Assume that Empty FStrings are out values.
    private bool IsOutValue => InnerValue.Length == 0;
    private nint StringDataAlloc = nint.Zero;

    public unsafe void ToAlloc(nint pAlloc)
    {
        // Zero out this memory region so Unreal doesn't try to Free this
        if (IsOutValue)
        {
            NativeMemory.Clear((void*)pAlloc, (nuint)sizeof(FString));
        }
        else
        {
            var pFString = _objects.CreateFString(InnerValue);
            StringDataAlloc = (nint)pFString->Data.AllocatorInstance;
            NativeMemory.Copy(pFString, (void*)pAlloc, (nuint)sizeof(FString));
            _memory.Free((nint)pFString);
        }
    }

    public unsafe void FromAlloc(nint pAlloc)
    {
        var pFString = (FString*)pAlloc;
        // String was changed during the function invocation - FString was passed by a mutable reference
        // Unreal would have already deallocated the old StringDataAlloc
        if (!IsOutValue && (nint)pFString->Data.AllocatorInstance != StringDataAlloc)
            StringDataAlloc = (nint)pFString->Data.AllocatorInstance;
        InnerValue = ((FString*)pAlloc)->ToString();
    }
    
    public string ExpectedPropertyClass => "StrProperty";
    public int ExpectedSize => Marshal.SizeOf<FString>();

    public void Dispose()
    {
        Disposing();
        GC.SuppressFinalize(this);
    }

    ~StringInvocationParameter() => Disposing();

    protected virtual void Disposing()
    {
        if (StringDataAlloc != nint.Zero)
            _memory.Free(StringDataAlloc);
    }
}

public class BoolInvocationParameter(bool value) : IInvocationParameter
{
    private bool InnerValue { get; set; } = value;
    public object? Value => InnerValue;

    public unsafe void ToAlloc(nint pAlloc)
        => *(byte*)pAlloc = (byte)(InnerValue ? 1 : 0);

    public unsafe void FromAlloc(nint pAlloc)
        => InnerValue = *(byte*)pAlloc != 0;
    
    public string ExpectedPropertyClass => "BoolProperty";
    public int ExpectedSize => Marshal.SizeOf<byte>();
}

public class UnrealMethods : IUnrealMethods
{
    
    #region Function Invocation Parameters (OLD)

    public IInvocationParameter CreateI8Param(sbyte Value = 0) => new I8InvocationParameter(Value);
    public IInvocationParameter CreateI16Param(short Value = 0) => new I16InvocationParameter(Value);
    public IInvocationParameter CreateI32Param(int Value = 0) => new I32InvocationParameter(Value);
    public IInvocationParameter CreateI64Param(long Value = 0) => new I64InvocationParameter(Value);
    public IInvocationParameter CreateU8Param(byte Value = 0) => new U8InvocationParameter(Value);
    public IInvocationParameter CreateU16Param(ushort Value = 0) => new U16InvocationParameter(Value);
    public IInvocationParameter CreateU32Param(uint Value = 0) => new U32InvocationParameter(Value);
    public IInvocationParameter CreateU64Param(ulong Value = 0) => new U64InvocationParameter(Value);
    public IInvocationParameter CreateF32Param(float Value = 0) => new F32InvocationParameter(Value);
    public IInvocationParameter CreateF64Param(double Value = 0) => new F64InvocationParameter(Value);
    public IInvocationParameter CreateNameParam(string Value) => new NameInvocationParameter(new FName(Value));
    public IInvocationParameter CreateStringParam(string Value) => new StringInvocationParameter(Value, Memory, Objects);
    public IInvocationParameter CreateBoolParam(bool Value = false) => new BoolInvocationParameter(Value);
    
    #endregion
    
    #region Function Invocation Execute (OLD)
    
    private delegate void UObject_ProcessEvent(nint Object, nint TargetFunction, nint Params);
    private uint UObject_ProcessEvent_Offset;

    [Flags]
    private enum ExecutionFlags
    {
        None = 0,
        DoTypeChecking = 1 << 0,
        // NoReturnType = 1 << 1,
    }

    private bool ProcessEventInner<TObject>(ToolkitUObject<TObject> Object, string Name,
        ref List<IInvocationParameter> Parameters, out IUFunction? Function, out nint Alloc, ExecutionFlags Flags) 
        where TObject : unmanaged
    {
        Log.Warning("IUnrealMethods::ProcessEvent is deprecated! Use IUObject::ProcessEvent instead!\n");
        // Get type reflection for object type
        Function = null;
        Alloc = nint.Zero;
        if (!Classes.GetClassInfoFromClass<TObject>(out var Reflection))
        {
            Log.Warning($"ProcessEvent: Could not retrieve reflection information for {nameof(TObject)}");
            return false;
        }
        Function = Reflection.GetFunction(Name);
        if (Function == null)
        {
            Log.Warning($"ProcessEvent: Could not find function in {nameof(TObject)} named {Name}");
            return false;
        }
        // Create allocation
        Alloc = Function.GetTotalParameterSize() switch
        {
            0 => nint.Zero, var Value => Memory.Malloc(Value)
        };
        // Set parameters
        foreach (var (Index, Field) in Function.ChildProperties.Select((x, i) => (i, x)))
        {
            // Assume CPF_Parm
            var Property = Factory.CreateFProperty(Field.Ptr);
            // If the function returns a non-void value, it will always contain a ReturnValue property as the last property.
            if (Property.PropertyFlags.HasFlag(EPropertyFlags.CPF_ReturnParm))
            {
                unsafe
                {
                    NativeMemory.Clear((void*)(Alloc + Property.Offset_Internal), (nuint)Property.ElementSize);
                }
                break;
            }
            var Parameter = Parameters[Index];
            // Type checking: Get type of current parameter, check against expected FProperty type
            if (Flags.HasFlag(ExecutionFlags.DoTypeChecking))
            {
                // var PropName = Property.ClassPrivate.Name;
                if (Parameter.ExpectedSize != Property.ElementSize)
                {
                    Log.Warning($"Could not invoke method {Name} on object with type {nameof(TObject)}: Type for parameter {Index} does not match.");
                    return false;
                }   
            }
            Parameter.ToAlloc(Alloc + Property.Offset_Internal);
        }

        unsafe { CallProcessEvent((nint)Object.Self, Function, Alloc); }
        return true;
    }

    private IInvocationParameter? CreateReturnInvocationParameter(IFProperty Property)
        => Property.ClassPrivate.Name switch
        {
            "Int8Property" => CreateI8Param(),
            "Int16Property" => CreateI16Param(),
            "IntProperty" => CreateI32Param(),
            "Int64Property" => CreateI64Param(),
            "UInt16Property" => CreateU16Param(),
            "UInt32Property" => CreateU32Param(),
            "UInt64Property" => CreateU64Param(),
            "FloatProperty" => CreateF32Param(),
            "DoubleProperty" => CreateF64Param(),
            "NameProperty" => CreateNameParam(string.Empty),
            "StrProperty" => CreateStringParam(string.Empty),
            "BoolProperty" => CreateBoolParam(),
            _ => null
        };

    private bool TypeCheckReturnValueParameter<TReturnType>(IFProperty Property) where TReturnType : unmanaged
        => Property.ClassPrivate.Name switch
        {
            "Int8Property" => typeof(TReturnType) == typeof(sbyte),
            "Int16Property" => typeof(TReturnType) == typeof(short),
            "IntProperty" => typeof(TReturnType) == typeof(int),
            "Int64Property" => typeof(TReturnType) == typeof(long),
            "UInt16Property" => typeof(TReturnType) == typeof(ushort),
            "UInt32Property" => typeof(TReturnType) == typeof(uint),
            "UInt64Property" => typeof(TReturnType) == typeof(ulong),
            "FloatProperty" => typeof(TReturnType) == typeof(float),
            "DoubleProperty" => typeof(TReturnType) == typeof(double),
            "NameProperty" => typeof(TReturnType) == typeof(FName),
            "StrProperty" => typeof(TReturnType) == typeof(Ptr<FString>),
            "BoolProperty" => typeof(TReturnType) == typeof(bool),
            _ => false
        };

    private unsafe TReturnType CastIntoReturnType<TReturnType>(IInvocationParameter Param) where TReturnType : unmanaged
    {
        switch (typeof(TReturnType).Name)
        {
            case "FString":
                var pFString = Objects.CreateFString((string)Param.Value);
                var Return = *(TReturnType*)pFString;
                Memory.Free((nint)pFString);
                return Return;
            default:
                return (TReturnType)Param.Value;
        }
    }

    public void ProcessEvent<TObject>(ToolkitUObject<TObject> Object, string Name,
        ref List<IInvocationParameter> Parameters) where TObject : unmanaged
    {
        if (!ProcessEventInner(Object, Name, ref Parameters, out var Function, out var Alloc, ExecutionFlags.None))
            return;
        foreach (var (Index, Field) in Function.ChildProperties.Select((x, i) => (i, x)))
        {
            var Property = Factory.CreateFProperty(Field.Ptr);
            if (Property.PropertyFlags.HasFlag(EPropertyFlags.CPF_ReferenceParm))
                Parameters[Index].FromAlloc(Alloc + Property.Offset_Internal);
        }
        if (Alloc != nint.Zero)
            Memory.Free(Alloc);
    }

    public TReturnType ProcessEvent<TObject, TReturnType>(ToolkitUObject<TObject> Object, string Name,
        ref List<IInvocationParameter> Parameters) 
        where TObject : unmanaged
        where TReturnType : unmanaged
    {
        TReturnType ReturnValue = default;
        if (!ProcessEventInner(Object, Name, ref Parameters, out var Function, out var Alloc, ExecutionFlags.None))
            return ReturnValue;
        foreach (var (Index, Field) in Function.ChildProperties.Select((x, i) => (i, x)))
        {
            var Property = Factory.CreateFProperty(Field.Ptr);
            if (Property.PropertyFlags.HasFlag(EPropertyFlags.CPF_ReferenceParm))
                Parameters[Index].FromAlloc(Alloc + Property.Offset_Internal);
            else if (Property.PropertyFlags.HasFlag(EPropertyFlags.CPF_ReturnParm))
            {
                // if (!TypeCheckReturnValueParameter(Property))
                var Return = CreateReturnInvocationParameter(Property);
                if (Return != null)
                {
                    Return.FromAlloc(Alloc + Property.Offset_Internal);
                    ReturnValue = CastIntoReturnType<TReturnType>(Return);
                }
                else
                {
                    Log.Warning($"Unimplemented ProcessEvent return value parameter type {Property.ClassPrivate.Name}");
                }
                break;
            }
        }

        if (Alloc != nint.Zero)
            Memory.Free(Alloc);
        return ReturnValue;
    }
    
    #endregion

    internal unsafe void CallProcessEvent(nint Object, IUFunction Function, nint Alloc)
    {
        var ProcessEventWrapper = Hooks.CreateWrapper<UObject_ProcessEvent>(*(nint*)(Function.VTable + UObject_ProcessEvent_Offset), out _);
        ProcessEventWrapper(Object, Function.Ptr, Alloc);   
    }

    internal unsafe IFunctionParam CreateReturnParam(IFProperty property)
    {
        return property.ClassPrivate.Name switch
        {
            "Int8Property" => new Int8Param(new((byte*)Memory.Malloc(sizeof(byte))), Memory),
            "Int16Property" => new Int16Param(new((short*)Memory.Malloc(sizeof(short))), Memory),
            "Int32Property" => new Int32Param(new((int*)Memory.Malloc(sizeof(int))), Memory),
            "IntProperty" => new IntParam(new((int*)Memory.Malloc(sizeof(int))), Memory),
            "Int64Property" => new Int64Param(new((long*)Memory.Malloc(sizeof(long))), Memory),
            "UInt16Property" => new UInt16Param(new((ushort*)Memory.Malloc(sizeof(ushort))), Memory),
            "UInt32Property" => new UInt32Param(new((uint*)Memory.Malloc(sizeof(uint))), Memory),
            "UInt64Property" => new UInt64Param(new((ulong*)Memory.Malloc(sizeof(ulong))), Memory),
            "FloatProperty" => new FloatParam(new((float*)Memory.Malloc(sizeof(float))), Memory),
            "DoubleProperty" => new DoubleParam(new((double*)Memory.Malloc(sizeof(double))), Memory),
            "NameProperty" => new NameParam(new((FName*)Memory.Malloc(sizeof(FName))), Memory),
            "ObjectProperty" => new ObjectParam(new((nint*)Memory.Malloc(sizeof(nint))), Memory),
            _ => throw new NotSupportedException($"CreateReturnParam with property {property.ClassPrivate.Name}")
        };
    }
    
    #region Unreal Toolkit API References

    private IUnrealFactory Factory;
    private IUnrealMemory Memory;
    private IUnrealClasses Classes;
    private IUnrealObjects Objects;
    #endregion
    
    #region Reloaded-II API References

    private IReloadedHooks Hooks;
    #endregion
    
    public UnrealMethods(IUnrealFactory factory, IUnrealMemory memory, IUnrealClasses classes, IUnrealObjects objects, IReloadedHooks hooks)
    {
        Factory = factory;
        Memory = memory;
        Classes = classes;
        Objects = objects;
        Hooks = hooks;
        
        Project.Inis.UsingSetting<uint>(Constants.UnrealIniId, "ProcessEvent", "UObject",
            value => UObject_ProcessEvent_Offset = value);
    }
}