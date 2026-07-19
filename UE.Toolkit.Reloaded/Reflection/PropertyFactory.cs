using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Reflection.UE5_4_4;

namespace UE.Toolkit.Reloaded.Reflection;

public abstract class BasePropertyFactory(IUnrealFactory factory, IUnrealMemory memory, 
    IUnrealClasses classes, IPropertyFlagsBuilder flags)
{
    private Dictionary<string, FName>? PropertyNames = null;
    
    // Lazy load property name cache, we need to do this to ensure that FName::FName() is valid
    private static Dictionary<string, FName> InitializePropertyNames() => new()
        {
            { "ByteProperty", new FName("ByteProperty", EFindName.FNAME_Find) },
            { "IntProperty", new FName("IntProperty", EFindName.FNAME_Find) },
            { "BoolProperty", new FName("BoolProperty", EFindName.FNAME_Find) },
            { "FloatProperty", new FName("FloatProperty", EFindName.FNAME_Find) },
            { "ObjectProperty", new FName("ObjectProperty", EFindName.FNAME_Find) },
            { "NameProperty", new FName("NameProperty", EFindName.FNAME_Find) },
            { "DelegateProperty", new FName("DelegateProperty", EFindName.FNAME_Find) },
            { "DoubleProperty", new FName("DoubleProperty", EFindName.FNAME_Find) },
            { "ArrayProperty", new FName("ArrayProperty", EFindName.FNAME_Find) },
            { "StructProperty", new FName("StructProperty", EFindName.FNAME_Find) },
            { "VectorProperty", new FName("VectorProperty", EFindName.FNAME_Find) },
            { "RotatorProperty", new FName("RotatorProperty", EFindName.FNAME_Find) },
            { "StrProperty", new FName("StrProperty", EFindName.FNAME_Find) },
            { "TextProperty", new FName("TextProperty", EFindName.FNAME_Find) },
            { "InterfaceProperty", new FName("InterfaceProperty", EFindName.FNAME_Find) },
            { "MulticastDelegateProperty", new FName("MulticastDelegateProperty", EFindName.FNAME_Find) },
            { "LazyObjectProperty", new FName("LazyObjectProperty", EFindName.FNAME_Find) },
            { "SoftObjectProperty", new FName("SoftObjectProperty", EFindName.FNAME_Find) },
            { "Int64Property", new FName("Int64Property", EFindName.FNAME_Find) },
            { "Int32Property", new FName("Int32Property", EFindName.FNAME_Find) },
            { "Int16Property", new FName("Int16Property", EFindName.FNAME_Find) },
            { "Int8Property", new FName("Int8Property", EFindName.FNAME_Find) },
            { "UInt64Property", new FName("UInt64Property", EFindName.FNAME_Find) },
            { "UInt32Property", new FName("UInt32Property", EFindName.FNAME_Find) },
            { "UInt16Property", new FName("UInt16Property", EFindName.FNAME_Find) },
            { "MapProperty", new FName("MapProperty", EFindName.FNAME_Find) },
            { "SetProperty", new FName("SetProperty", EFindName.FNAME_Find) },
        };

    public bool CheckPropertyEquality(string Name, uint OtherValue)
    {
        PropertyNames ??= InitializePropertyNames();
        if (!PropertyNames.TryGetValue(Name, out var Value))
            return false;
        return Value.ComparisonIndex.Value == OtherValue;
    }

    protected bool GetProperty(string Name, out FieldClassGlobal? FieldClass)
    {
        FieldClass = null;
        PropertyNames ??= InitializePropertyNames();
        return PropertyNames!.TryGetValue(Name, out var SerialName) 
               && Classes.GetFieldClassGlobal(SerialName, out FieldClass);
    }
    
    protected bool TryGetClassAndProperty<TOwner>(string PropertyName, out IUClass? ClassReflection,
        out FieldClassGlobal? PropertyClass) where TOwner: unmanaged
    {
        ClassReflection = null;
        PropertyClass = null;
        return Classes.GetClassInfoFromClass<TOwner>(out ClassReflection)
               && GetProperty(PropertyName, out PropertyClass);
    }
    
    #region INTERNAL INTERFACE
    
    protected abstract void LinkToPropertyList(IFProperty Property, IUClass? Reflect);
    
    #region Properties Owned by a Class

    protected abstract void SetPropertySuperFields(IFField Field, string Name, IUClass ClassReflection,
        FieldClassGlobal PropertyClass);
    
    protected abstract void SetCopyPropertyFields<T>(IFProperty Property, int Offset, PropertyVisibility Visibility)
        where T : unmanaged;
    
    protected abstract void SetStringPropertyFields<T>(IFProperty Property, int Offset, PropertyVisibility Visibility)
        where T : unmanaged;
    
    protected abstract void SetTextPropertyFields<T>(IFProperty Property, int Offset, PropertyVisibility Visibility)
        where T : unmanaged;

    private bool CreatePropertyInner<TOwner, TProperty>(out IFProperty? NewProperty,
        string Name, int Offset, string PropertyName, PropertyVisibility Visibility,
        Action<IFProperty, int, PropertyVisibility> Callback)
        where TOwner : unmanaged
        where TProperty : unmanaged
    {
        NewProperty = null;
        if (!TryGetClassAndProperty<TOwner>(PropertyName, out var ClassReflection, out var PropertyClass))
            return false;
        var Alloc = Memory.Malloc(Marshal.SizeOf<TProperty>(), FIELD_ALIGNMENT);
        SetPropertySuperFields(Factory.CreateFField(Alloc), Name, ClassReflection!, PropertyClass!);
        NewProperty = Factory.CreateFProperty(Alloc);
        Callback(NewProperty, Offset, Visibility);
        LinkToPropertyList(NewProperty, ClassReflection!);
        return true;
    }
    
    protected bool CreateCopyPropertyInner<TOwner, TField, TProperty>(out IFProperty? NewProperty,
        string Name, int Offset, string PropertyName, PropertyVisibility Visibility)
        where TOwner : unmanaged
        where TField : unmanaged
        where TProperty : unmanaged
        => CreatePropertyInner<TOwner, TProperty>(out NewProperty, Name,
            Offset, PropertyName, Visibility, SetCopyPropertyFields<TField>);
    
    protected bool CreateStringPropertyInner<TOwner, TField, TProperty>(out IFProperty? NewProperty,
        string Name, int Offset, string PropertyName, PropertyVisibility Visibility)
        where TOwner : unmanaged
        where TField : unmanaged
        where TProperty : unmanaged
        => CreatePropertyInner<TOwner, TProperty>(out NewProperty, Name,
            Offset, PropertyName, Visibility, SetStringPropertyFields<TField>);
    
    protected bool CreateTextPropertyInner<TOwner, TField, TProperty>(out IFProperty? NewProperty,
        string Name, int Offset, string PropertyName, PropertyVisibility Visibility)
        where TOwner : unmanaged
        where TField : unmanaged
        where TProperty : unmanaged
        => CreatePropertyInner<TOwner, TProperty>(out NewProperty, Name,
            Offset, PropertyName, Visibility, SetTextPropertyFields<TField>);
    
    #endregion
    
    // Used in cases where we're constructing a type parameter for a generic type such as an array or map,
    // so we don't want the element linked to a class. Set no parent initially, then let the array/map
    // property initializer set the owners for the type params.
    #region Unowned Properties
    
    protected abstract void SetPropertySuperFieldsNoOwner(IFField Field, string Name, FieldClassGlobal PropertyClass);

    private bool CreatePropertyInner<TProperty>(out IFProperty? NewProperty,
        string Name, int Offset, string PropertyName, PropertyVisibility Visibility,
        Action<IFProperty, int, PropertyVisibility> Callback)
        where TProperty : unmanaged
    {
        NewProperty = null;
        if (!GetProperty(PropertyName, out var PropertyClass))
            return false;
        var Alloc = Memory.Malloc(Marshal.SizeOf<TProperty>(), FIELD_ALIGNMENT);
        SetPropertySuperFieldsNoOwner(Factory.CreateFField(Alloc), Name, PropertyClass!);
        NewProperty = Factory.CreateFProperty(Alloc);
        Callback(NewProperty, Offset, Visibility);
        LinkToPropertyList(NewProperty, null);
        return true;
    }
    
    protected bool CreateCopyPropertyInner<TField, TProperty>(out IFProperty? NewProperty,
        string Name, int Offset, string PropertyName, PropertyVisibility Visibility)
        where TField : unmanaged
        where TProperty : unmanaged
        => CreatePropertyInner<TProperty>(out NewProperty, Name,
            Offset, PropertyName, Visibility, SetCopyPropertyFields<TField>);
    
    protected bool CreateStringPropertyInner<TField, TProperty>(out IFProperty? NewProperty,
        string Name, int Offset, string PropertyName, PropertyVisibility Visibility)
        where TField : unmanaged
        where TProperty : unmanaged
        => CreatePropertyInner<TProperty>(out NewProperty, Name,
            Offset, PropertyName, Visibility, SetStringPropertyFields<TField>);
    
    protected bool CreateTextPropertyInner<TField, TProperty>(out IFProperty? NewProperty,
        string Name, int Offset, string PropertyName, PropertyVisibility Visibility)
        where TField : unmanaged
        where TProperty : unmanaged
        => CreatePropertyInner<TProperty>(out NewProperty, Name,
            Offset, PropertyName, Visibility, SetTextPropertyFields<TField>);
    
    #endregion

    protected abstract void SetBoolPropertyFields(IFBoolProperty Property, BooleanMask Mask);
    
    protected bool CreateBoolPropertyInner<TOwner>(out IFBoolProperty? NewProperty, string Name, int Offset, 
        PropertyVisibility Visibility, BooleanMask Mask) where TOwner: unmanaged
    {
        NewProperty = null;
        if (!CreateCopyPropertyInner<TOwner, byte, FBoolProperty>(out var BaseProperty, Name, Offset, 
                "BoolProperty", Visibility))
            return false;
        NewProperty = Factory.CreateFBoolProperty(BaseProperty!.Ptr);
        SetBoolPropertyFields(NewProperty, Mask);
        return true;
    }

    protected IFProperty GetPreviousProperty(IFProperty Property, IUClass Reflect)
    {
        IFProperty? TargetProp = null;
        var MinimumOffset = Reflect.SuperStruct?.PropertiesSize ?? 0;
        foreach (var Prop in Reflect.PropertyLink)
        {
            // We're the last element in the chain
            var StopConditions = !Prop.PropertyLinkNext.Any() ||
                                 // The next element will have a larger offset than our new field, so insert before them
                                 Prop.PropertyLinkNext.First().Offset_Internal > Property.Offset_Internal ||
                                 // The next element is from the super class, stop since the property chain of a supertype
                                 // is shared between it and all subtypes
                                 Prop.PropertyLinkNext.First().Offset_Internal < MinimumOffset;
            if (StopConditions)
            {
                TargetProp = Prop;
                break;               
            }
        }
        return TargetProp!;
    }
    
    #endregion
   
    #region PUBLIC INTERFACE
    
    public abstract bool CreateI8<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateI16<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateI32<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateI64<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateU8<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateU16<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateU32<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateU64<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateF32<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateF64<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateI8(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility);
    
    public abstract bool CreateI16(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility);
    
    public abstract bool CreateI32(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility);
    
    public abstract bool CreateI64(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility);
    
    public abstract bool CreateU8(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility);
    
    public abstract bool CreateU16(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility);
    
    public abstract bool CreateU32(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility);
    
    public abstract bool CreateU64(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility);
    
    public abstract bool CreateF32(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility);
    
    public abstract bool CreateF64(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility);
    
    public bool CreateCBool<TOwner>(out IFBoolProperty? NewProperty, string Name, int Offset, 
        PropertyVisibility Visibility) where TOwner: unmanaged
        => CreateBoolPropertyInner<TOwner>(out NewProperty, Name, Offset, Visibility, new(1, 255));

    public bool CreateBitBool<TOwner>(out IFBoolProperty? NewProperty, string Name, int Offset, 
        int Bit, PropertyVisibility Visibility) where TOwner: unmanaged
    {
        NewProperty = null;
        if (Bit > 7) return false;
        var Mask = (byte)(1 << Bit);
        return CreateBoolPropertyInner<TOwner>(out NewProperty, Name, Offset, Visibility, new(Mask, Mask));
    }
   
    /*
    public abstract bool CreateCBool<TOwner>(out IFBoolProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner : unmanaged;
    
    public abstract bool CreateBitBool<TOwner>(out IFBoolProperty? NewProperty, string Name, int Offset,
        int Bit, PropertyVisibility Visibility) where TOwner : unmanaged;
    */

    public abstract bool CreateStruct<TOwner, TField>(out IFStructProperty? NewProperty,
        string Name, int Offset, PropertyVisibility Visibility)
        where TOwner : unmanaged
        where TField : unmanaged;
    
    public abstract bool CreateStruct<TField>(out IFStructProperty? NewProperty,
        string Name, int Offset, PropertyVisibility Visibility)
        where TField : unmanaged;

    public abstract bool CreateStruct(out IFStructProperty? NewProperty,
        string Name, string TypeName, int Offset, PropertyVisibility Visibility);
    
    public abstract bool CreateStructDTSpecial(out IFObjectProperty? NewProperty,
        string Name, string TypeName, int Offset, PropertyVisibility Visibility);
    
    public abstract bool CreateObject<TOwner, TField>(out IFObjectProperty? NewProperty,
        string Name, int Offset, PropertyVisibility Visibility)
        where TOwner : unmanaged
        where TField : unmanaged;

    public bool CreateClass<TOwner, TField>(out IFClassProperty? NewProperty,
        string Name, int Offset, PropertyVisibility Visibility)
        where TOwner : unmanaged
        where TField : unmanaged
    {
        NewProperty = null;
        if (!CreateObject<TOwner, TField>(out var ObjectProperty, Name, Offset, Visibility))
            return false;
        NewProperty = Factory.CreateFClassProperty(ObjectProperty!.Ptr);
        return true;
    }
    
    public abstract bool CreateName<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateString<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateText<TOwner>(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) where TOwner: unmanaged;
    
    public abstract bool CreateName(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) ;
    
    public abstract bool CreateString(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) ;
    
    public abstract bool CreateText(out IFProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility) ;

    public abstract bool CreateArray<TObject>(out IFArrayProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility, IFProperty Inner) where TObject : unmanaged;
    
    // public abstract bool CreateMap<TObject>(out IFArrayProperty? NewProperty, string Name, int Offset,
    //     PropertyVisibility Visibility, IFProperty Key, IFProperty Value) where TObject : unmanaged;
    
     public abstract bool CreateMap(out IFMapProperty? NewProperty, string Name, int Offset,
         PropertyVisibility Visibility, IFProperty Key, IFProperty Value);
    #endregion

    protected readonly IUnrealFactory Factory = factory;
    protected readonly IUnrealMemory Memory = memory;
    protected readonly IUnrealClasses Classes = classes;
    protected readonly IPropertyFlagsBuilder Flags = flags;

    protected const int FIELD_ALIGNMENT = 0x80;
}

[Flags]
public enum PropertyBuilderFlags
{
    None = 0,
    NoCtor = 1 << 0, // ZeroConstructor, can just be memset
    Copy = 1 << 1, // If property can be memcpy'd.
    NoDtor = 1 << 2, // No destructor
    Hash = 1 << 3, // Can be hashed
}

public struct BooleanMask(byte byteMask, byte fieldMask)
{
    public byte ByteMask { get; } = byteMask;
    public byte FieldMask { get; } = fieldMask;
}