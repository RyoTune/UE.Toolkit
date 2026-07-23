using UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Core.Types.Unreal.UE5_6_1;

namespace UE.Toolkit.Core.Types.Interfaces;

public interface IFunctionParam
{
    string PropertyType { get; }
    void Write(nint Destination);
    void Read(nint Destination);
}

public abstract class FunctionParamCopyable<T>(Ptr<T> rvalue, string propertyType, IUnrealMemoryInternal? memory = null) 
    : IFunctionParam, IDisposable where T : unmanaged
{
    public unsafe T Value => *RValue.Value;
    
    protected Ptr<T> RValue => rvalue;

    protected IUnrealMemoryInternal? Memory => memory;

    public string PropertyType => propertyType;

    private static unsafe void Copy(T* source, T* destination) => *destination = *source;

    public unsafe void Write(nint Destination) => Copy(RValue.Value, (T*)Destination);

    public unsafe void Read(nint Destination) => Copy((T*)Destination, RValue.Value);

    protected bool IsDisposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing) { }
            unsafe { Memory?.Free((nint)RValue.Value); }
            IsDisposed = true;
        }
    }
    
    ~FunctionParamCopyable() => Dispose(false);
}

public enum ProcessEventResult
{
    Success,
    CouldNotFindFunction,
    ParameterTypeMismatch,
}

public static class FunctionParamFactory
{

    private static Dictionary<string, string> PropertyNameToParamName = [];
    
    private static IFunctionParam CreateStructParam(IFStructProperty property, IUnrealMemoryInternal? Memory) 
        => new StructParam(Memory?.Malloc(property.ElementSize) ?? nint.Zero, property.ElementSize, Memory);

    private static unsafe IFunctionParam CreateBoolParam(IFBoolProperty property, IUnrealMemoryInternal? Memory)
        => new BoolParam(new((bool*)(Memory?.Malloc(sizeof(bool)) ?? nint.Zero)), property.FieldMask, Memory);
    
    public static unsafe IFunctionParam CreateParam(IFProperty property, IUnrealFactory factory, 
        ITypeReflectionInternal reflection, IUnrealMemoryInternal? Memory)
    {
        return property.ClassPrivate.Name switch
        {
            "BoolProperty" => CreateBoolParam(factory.CreateFBoolProperty(property.Ptr), Memory),
            "Int8Property" => new Int8Param(new((byte*)(Memory?.Malloc(sizeof(byte)) ?? nint.Zero)), Memory),
            "ByteProperty" => new ByteParam(new((byte*)(Memory?.Malloc(sizeof(byte)) ?? nint.Zero)), Memory),
            "Int16Property" => new Int16Param(new((short*)(Memory?.Malloc(sizeof(short)) ?? nint.Zero)), Memory),
            "Int32Property" => new Int32Param(new((int*)(Memory?.Malloc(sizeof(int)) ?? nint.Zero)), Memory),
            "IntProperty" => new IntParam(new((int*)(Memory?.Malloc(sizeof(int)) ?? nint.Zero)), Memory),
            "Int64Property" => new Int64Param(new((long*)(Memory?.Malloc(sizeof(long)) ?? nint.Zero)), Memory),
            "UInt16Property" => new UInt16Param(new((ushort*)(Memory?.Malloc(sizeof(ushort)) ?? nint.Zero)), Memory),
            "UInt32Property" => new UInt32Param(new((uint*)(Memory?.Malloc(sizeof(uint)) ?? nint.Zero)), Memory),
            "UInt64Property" => new UInt64Param(new((ulong*)(Memory?.Malloc(sizeof(ulong)) ?? nint.Zero)), Memory),
            "FloatProperty" => new FloatParam(new((float*)(Memory?.Malloc(sizeof(float)) ?? nint.Zero)), Memory),
            "DoubleProperty" => new DoubleParam(new((double*)(Memory?.Malloc(sizeof(double)) ?? nint.Zero)), Memory),
            "NameProperty" => new NameParam(new((FName*)(Memory?.Malloc(sizeof(FName)) ?? nint.Zero)), Memory),
            "StrProperty" => new StringParam(new((FString*)(Memory?.Malloc(sizeof(FString)) ?? nint.Zero)), Memory),
            "StructProperty" => CreateStructParam(factory.CreateFStructProperty(property.Ptr), Memory),
            "TextProperty" => new TextParam(Memory?.Malloc(reflection.GetFTextSize()) ?? nint.Zero, reflection.GetFTextSize(), Memory),
            "ObjectProperty" or "ClassProperty" or "ClassPtrProperty" 
                => new ObjectParam(new((nint*)(Memory?.Malloc(sizeof(nint)) ?? nint.Zero)), Memory),
            "ArrayProperty" => new ArrayParam(new((TArray<int>*)(Memory?.Malloc(sizeof(TArray<char>)) ?? nint.Zero)), Memory),
            "EnumProperty" => new EnumParam(new(Memory?.Malloc(property.ElementSize) ?? nint.Zero), property.ElementSize, Memory),
            "MapProperty" => new MapParam(new((TMap<int, int>*)(Memory?.Malloc(sizeof(TMap<int, int>)) ?? nint.Zero)), Memory),
            "SetProperty" => new SetParam(new((TSet<int>*)(Memory?.Malloc(sizeof(TSet<int>)) ?? nint.Zero)), Memory),
            "InterfaceProperty" => new InterfaceParam(new((TScriptInterface<int>*)(Memory?.Malloc(sizeof(TScriptInterface<char>)) ?? nint.Zero)), Memory),
            "SoftClassProperty" => new SoftClassParam(new((TSoftClassPtr<int>*)(Memory?.Malloc(sizeof(TSoftClassPtr<char>)) ?? nint.Zero)), Memory),
            "SoftObjectProperty" => new SoftObjectParam(new((TSoftObjectPtr<int>*)(Memory?.Malloc(sizeof(TSoftObjectPtr<char>)) ?? nint.Zero)), Memory),
            "Utf8StrProperty" => new Utf8StringParam(new((FUtf8String*)(Memory?.Malloc(sizeof(FUtf8String)) ?? nint.Zero)), Memory),
            "AnsiStrProperty" => new AnsiStringParam(new((FAnsiString*)(Memory?.Malloc(sizeof(FAnsiString)) ?? nint.Zero)), Memory),
            _ => throw new NotSupportedException($"CreateParam with property {property.ClassPrivate.Name}")
        };
    }

    public static string? GetParamNameFromProperty(IFProperty property, IUnrealFactory factory, ITypeReflectionInternal reflection)
    {
        try
        {
            var PropertyName = property.ClassPrivate.Name;
            if (PropertyNameToParamName.TryGetValue(PropertyName, out var ParamName)) return ParamName;
            var BlankParam = CreateParam(property, factory, reflection, null);
            PropertyNameToParamName[PropertyName] = BlankParam.GetType().Name;
            return PropertyNameToParamName[PropertyName];
        }
        catch (NotSupportedException ex)
        {
            return null;
        }
    }
}