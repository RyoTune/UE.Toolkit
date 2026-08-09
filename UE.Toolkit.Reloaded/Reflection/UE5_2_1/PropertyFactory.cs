using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;

using FField = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FField;
using FFieldClass = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FFieldClass;
using FProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FProperty;
using UObjectBase = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UObjectBase;

namespace UE.Toolkit.Reloaded.Reflection.UE5_2_1;

public class PropertyFactory(IUnrealFactory factory, IUnrealMemory memory, 
    IUnrealClasses classes, IPropertyFlagsBuilder flags)
    : BasePropertyFactory(factory, memory, classes, flags)
{
    
    private static unsafe void InsertPropertyLink(IFProperty Target, IFProperty Prop)
    {
        var pProperty = (FProperty*)Prop.Ptr;
        var pTargetProp = (FProperty*)Target.Ptr;
        pTargetProp->prop_link_next = pProperty;
        ((FField*)pTargetProp)->next = (FField*)pProperty;
    }

    protected override unsafe void LinkToPropertyList(IFProperty Property, IUStruct? Reflect)
    {
        var pProperty = (FProperty*)Property.Ptr;
        pProperty->prop_link_next = null;
        pProperty->next_ref = null;
        pProperty->dtor_link_next = null;
        pProperty->post_ct_link_next = null;

        if (Reflect == null) return;
        var pClass = (UClass*)Reflect.Ptr;
        // Is this the only field?
        if (!AnyDirectlyDefinedProperties(Reflect))
        {
            if (Reflect.PropertyLink.Any())
            {
                var TargetProp = Reflect.PropertyLink.First();
                InsertPropertyLink(Property, TargetProp);
            }
            ((UStruct*)pClass)->PropertyLink = (UE.Toolkit.Core.Types.Unreal.UE5_4_4.FProperty*)pProperty;
            ((UStruct*)pClass)->ChildProperties = (UE.Toolkit.Core.Types.Unreal.UE5_4_4.FField*)pProperty;
        }
        else
        {
            var TargetProp = GetPreviousProperty(Property, Reflect);
            var NextProp = TargetProp.PropertyLinkNext.Any() ? TargetProp.PropertyLinkNext.First() : null;
            InsertPropertyLink(TargetProp, Property);
            if (NextProp != null) InsertPropertyLink(Property, NextProp);
        }       
    }
    
    protected override unsafe void SetPropertySuperFieldsNoOwner(IFField Field, string Name, FieldClassGlobal PropertyClass)
    {
        var pField = (FField*)Field.Ptr;
        pField->_vtable = PropertyClass.Vtable;
        pField->class_private = (FFieldClass*)PropertyClass.Params.Ptr;
        pField->next = null;
        pField->name_private = new FName(Name);
        pField->flags_private = EObjectFlags.RF_Public | EObjectFlags.RF_MarkAsNative | EObjectFlags.RF_Transient;
    }

    private unsafe void SetPropertySuperFieldsUObject(IFField Field, string Name, IUStruct ClassReflection,
        FieldClassGlobal PropertyClass)
    {
        SetPropertySuperFieldsNoOwner(Field, Name, PropertyClass);
        var pField = (FField*)Field.Ptr;
        pField->owner.Object = (UObjectBase*)ClassReflection.Ptr; // UClass*
        pField->owner.bIsUObject = true;
    }
    
    protected override void SetPropertySuperFields(IFField Field, string Name, IUStruct ClassReflection,
        FieldClassGlobal PropertyClass) => SetPropertySuperFieldsUObject(Field, Name, ClassReflection, PropertyClass);
    
    private unsafe void SetPropertyFieldDefaults(FProperty* pProperty, int Offset)
    {
        pProperty->rep_index = 0;
        pProperty->blueprint_rep_cond = 0;
        pProperty->offset_internal = Offset;
    }
    
    private unsafe void SetPropertyFieldsInner<T>(IFProperty Property, int Offset, 
        PropertyVisibility Visibility, PropertyBuilderFlags PropertyFlags)
    {
        var pProperty = (FProperty*)Property.Ptr;
        pProperty->array_dim = 1;
        pProperty->element_size = Marshal.SizeOf<T>();
        pProperty->property_flags = Flags.CreatePropertyFlags(Visibility, PropertyFlags);
        SetPropertyFieldDefaults(pProperty, Offset);
    }
    
    protected override unsafe void SetCopyPropertyFields<T>(IFProperty Property, int Offset, 
        PropertyVisibility Visibility)
    {
        var PropertyFlags = PropertyBuilderFlags.NoCtor | PropertyBuilderFlags.Copy | PropertyBuilderFlags.NoDtor;
        SetPropertyFieldsInner<T>(Property, Offset, Visibility, PropertyFlags);
    }

    protected override void SetStringPropertyFields<T>(IFProperty Property, int Offset,
        PropertyVisibility Visibility)
        => SetPropertyFieldsInner<T>(Property, Offset, Visibility, PropertyBuilderFlags.NoCtor);
    
    protected override void SetTextPropertyFields<T>(IFProperty Property, int Offset,
        PropertyVisibility Visibility)
        => SetPropertyFieldsInner<T>(Property, Offset, Visibility, PropertyBuilderFlags.None);
    
    protected override unsafe void SetBoolPropertyFields(IFBoolProperty Property, BooleanMask Mask) 
    {
        var pBoolProperty = (FBoolProperty*)Property.Ptr;
        pBoolProperty->FieldSize = (byte)Marshal.SizeOf<byte>();
        pBoolProperty->ByteOffset = 0;
        pBoolProperty->ByteMask = Mask.ByteMask;
        pBoolProperty->FieldMask = Mask.FieldMask;       
    }
    
    public override bool CreateI8<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<TOwner, byte, FProperty>(out NewProperty, Name, Offset, "Int8Property", Visibility);
    
    public override bool CreateI16<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<TOwner, short, FProperty>(out NewProperty, Name, Offset, "Int16Property", Visibility);
    
    public override bool CreateI32<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<TOwner, int, FProperty>(out NewProperty, Name, Offset, "IntProperty", Visibility);
    
    public override bool CreateI64<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<TOwner, long, FProperty>(out NewProperty, Name, Offset, "Int64Property", Visibility);
    
    public override bool CreateU8<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<TOwner, byte, FProperty>(out NewProperty, Name, Offset, "Int8Property", Visibility);
    
    public override bool CreateU16<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<TOwner, short, FProperty>(out NewProperty, Name, Offset, "UInt16Property", Visibility);
    
    public override bool CreateU32<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<TOwner, int, FProperty>(out NewProperty, Name, Offset, "UInt32Property", Visibility);
    
    public override bool CreateU64<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<TOwner, long, FProperty>(out NewProperty, Name, Offset, "UInt64Property", Visibility);
    
    public override bool CreateF32<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<TOwner, float, FProperty>(out NewProperty, Name, Offset, "FFloatProperty", Visibility);
    
    public override bool CreateF64<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<TOwner, double, FProperty>(out NewProperty, Name, Offset, "FDoubleProperty", Visibility);
    
    public override bool CreateI8(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<byte, FProperty>(out NewProperty, Name, Offset, "Int8Property", Visibility);
    
    public override bool CreateI16(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<short, FProperty>(out NewProperty, Name, Offset, "Int16Property", Visibility);
    
    public override bool CreateI32(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<int, FProperty>(out NewProperty, Name, Offset, "IntProperty", Visibility);
    
    public override bool CreateI64(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<long, FProperty>(out NewProperty, Name, Offset, "Int64Property", Visibility);
    
    public override bool CreateU8(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<byte, FProperty>(out NewProperty, Name, Offset, "Int8Property", Visibility);
    
    public override bool CreateU16(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<short, FProperty>(out NewProperty, Name, Offset, "UInt16Property", Visibility);
    
    public override bool CreateU32(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<int, FProperty>(out NewProperty, Name, Offset, "UInt32Property", Visibility);
    
    public override bool CreateU64(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<long, FProperty>(out NewProperty, Name, Offset, "UInt64Property", Visibility);
    
    public override bool CreateF32(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<float, FProperty>(out NewProperty, Name, Offset, "FFloatProperty", Visibility);
    
    public override bool CreateF64(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<double, FProperty>(out NewProperty, Name, Offset, "FDoubleProperty", Visibility);
    
    private bool CreateStructInner(
        out IFStructProperty? NewProperty, 
        string Name, 
        int Offset,
        PropertyVisibility Visibility,
        IUStruct? ClassReflection,
        FieldClassGlobal PropertyClass,
        IUScriptStruct ScriptStruct
    )
    {
        NewProperty = null;
        var Alloc = Memory.Malloc(Marshal.SizeOf<FStructProperty>(), FIELD_ALIGNMENT);
        NewProperty = Factory.CreateFStructProperty(Alloc);
        if (ClassReflection != null) SetPropertySuperFields(Factory.CreateFField(Alloc), Name, ClassReflection, PropertyClass);
        else SetPropertySuperFieldsNoOwner(Factory.CreateFField(Alloc), Name, PropertyClass);
        unsafe
        {
            var pProperty = (FProperty*)Alloc;
            pProperty->array_dim = 1;
            pProperty->element_size = ScriptStruct.PropertiesSize; // FExampleStruct mExampleField; 
            pProperty->property_flags = Flags.CreatePropertyFlags(Visibility, PropertyBuilderFlags.None);
            SetPropertyFieldDefaults(pProperty, Offset);
        }
        unsafe { ((FStructProperty*)NewProperty.Ptr)->Struct = (UScriptStruct*)ScriptStruct.Ptr; }
        LinkToPropertyList(NewProperty, ClassReflection);
        return true;
    }

    public override bool CreateStruct<TOwner, TField>(out IFStructProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility)
    {
        NewProperty = null;
        if (!TryGetClassAndProperty<TOwner>("StructProperty", out var ClassReflection, out var PropertyClass)
            || !Classes.GetScriptStructInfoFromType<TField>(out var ScriptStruct))
            return false;
        return CreateStructInner(out NewProperty, Name, Offset, Visibility, ClassReflection, PropertyClass, ScriptStruct);
    }
    
    public override bool CreateStruct<TField>(out IFStructProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility)
    {
        NewProperty = null;
        if (!GetProperty("StructProperty", out var PropertyClass)
            || !Classes.GetScriptStructInfoFromType<TField>(out var ScriptStruct))
            return false;
        return CreateStructInner(out NewProperty, Name, Offset, Visibility, null, PropertyClass, ScriptStruct);
    }
    
    public override bool CreateStruct(out IFStructProperty? NewProperty, string Name, string TypeName, int Offset,
        PropertyVisibility Visibility)
    {
        NewProperty = null;
        if (!GetProperty("StructProperty", out var PropertyClass)
            || !Classes.GetScriptStructInfoFromName($"F{TypeName}", out var ScriptStruct))
            return false;
        return CreateStructInner(out NewProperty, Name, Offset, Visibility, null, PropertyClass, ScriptStruct);
    }
    
    public override bool CreateStructDTSpecial(out IFObjectProperty? NewProperty,
        string Name, string TypeName, int Offset, PropertyVisibility Visibility)
    {
        NewProperty = null;
        if (!GetProperty("ObjectProperty", out var PropertyClass)
            || !Classes.GetScriptStructInfoFromName($"F{TypeName}", out var FieldClass))
            return false;
        var Alloc = Memory.Malloc(Marshal.SizeOf<FObjectProperty>(), FIELD_ALIGNMENT);
        NewProperty = Factory.CreateFObjectProperty(Alloc);
        SetPropertySuperFieldsNoOwner(Factory.CreateFField(Alloc), Name, PropertyClass!);
        unsafe
        {
            var pProperty = (FProperty*)Alloc;
            pProperty->array_dim = 1;
            pProperty->element_size = Marshal.SizeOf<nint>();
            pProperty->property_flags = Flags.CreatePropertyFlags(Visibility, PropertyBuilderFlags.None);
            SetPropertyFieldDefaults(pProperty, Offset);
        }
        LinkToPropertyList(NewProperty, null);
        unsafe { ((FObjectProperty*)NewProperty.Ptr)->PropertyClass = (UClass*)FieldClass!.Ptr; } // Our "class"
        return true;
    }
    
    private bool CreateObjectInner(
        out IFObjectProperty? NewProperty, 
        string Name, 
        int Offset,
        PropertyVisibility Visibility,
        IUStruct? ClassReflection,
        FieldClassGlobal PropertyClass,
        IUClass FieldClass
    )
    {
        NewProperty = null;
        var Alloc = Memory.Malloc(Marshal.SizeOf<FObjectProperty>(), FIELD_ALIGNMENT);
        NewProperty = Factory.CreateFObjectProperty(Alloc);
        if (ClassReflection != null) SetPropertySuperFields(Factory.CreateFField(Alloc), Name, ClassReflection, PropertyClass);
        else SetPropertySuperFieldsNoOwner(Factory.CreateFField(Alloc), Name, PropertyClass);
        unsafe
        {
            var pProperty = (FProperty*)Alloc;
            pProperty->array_dim = 1;
            pProperty->element_size = Marshal.SizeOf<nint>();
            // For ObjectProperty:  UExampleClass* pExampleObject;
            // For ClassProperty:  TSubclassOf<class UExampleClass> pExampleClass;
            pProperty->property_flags = Flags.CreatePropertyFlags(Visibility, PropertyBuilderFlags.None);
            SetPropertyFieldDefaults(pProperty, Offset);
        }
        unsafe { ((FObjectProperty*)NewProperty.Ptr)->PropertyClass = (UClass*)FieldClass.Ptr; }
        LinkToPropertyList(NewProperty, ClassReflection!);
        return true;
    }

    public override bool CreateObject<TOwner, TField>(out IFObjectProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility)
    {
        NewProperty = null;
        if (!TryGetClassAndProperty<TOwner>("ObjectProperty", out var ClassReflection, out var PropertyClass)
            || !Classes.GetClassInfoFromClass<TField>(out var FieldClass))
            return false;
        return CreateObjectInner(out NewProperty, Name, Offset, Visibility, ClassReflection, PropertyClass, FieldClass);
    }

    public override bool CreateObject<TField>(out IFObjectProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility)
    {
        NewProperty = null;
        if (!GetProperty("ObjectProperty", out var PropertyClass)
            || !Classes.GetClassInfoFromClass<TField>(out var Class))
            return false;
        return CreateObjectInner(out NewProperty, Name, Offset, Visibility, null, PropertyClass, Class);
    }
    
    public override bool CreateObject(out IFObjectProperty? NewProperty, string Name, string TypeName, int Offset,
        PropertyVisibility Visibility)
    {
        NewProperty = null;
        if (!GetProperty("ObjectProperty", out var PropertyClass)
            || !Classes.GetClassInfoFromName($"U{TypeName}", out var Class))
            return false;
        return CreateObjectInner(out NewProperty, Name, Offset, Visibility, null, PropertyClass, Class);
    }

    public override bool CreateName<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<TOwner, FName, FProperty>(out NewProperty, Name, Offset, "NameProperty", Visibility);
    
    public override bool CreateString<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateStringPropertyInner<TOwner, FName, FProperty>(out NewProperty, Name, Offset, "StrProperty", Visibility);
    
    public override bool CreateText<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateTextPropertyInner<TOwner, FName, FProperty>(out NewProperty, Name, Offset, "TextProperty", Visibility);
    
    public override bool CreateName(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<FName, FProperty>(out NewProperty, Name, Offset, "NameProperty", Visibility);
    
    public override bool CreateString(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateStringPropertyInner<FName, FProperty>(out NewProperty, Name, Offset, "StrProperty", Visibility);
    
    public override bool CreateText(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateTextPropertyInner<FName, FProperty>(out NewProperty, Name, Offset, "TextProperty", Visibility);

    private bool CreateArrayInner(
        out IFArrayProperty? NewProperty,
        string Name,
        int Offset,
        PropertyVisibility Visibility,
        IUStruct? ClassReflection,
        FieldClassGlobal PropertyClass,
        IFProperty Inner
    )
    {
        var Alloc = Memory.Malloc(Marshal.SizeOf<FArrayProperty>(), FIELD_ALIGNMENT);
        NewProperty = Factory.CreateFArrayProperty(Alloc);
        if (ClassReflection != null) SetPropertySuperFields(Factory.CreateFField(Alloc), Name, ClassReflection, PropertyClass);
        else SetPropertySuperFieldsNoOwner(Factory.CreateFField(Alloc), Name, PropertyClass);
        unsafe
        {
            var pProperty = (FProperty*)Alloc;
            pProperty->array_dim = 1;
            pProperty->element_size = 0x10; // sizeof(TArray<T>)
            pProperty->property_flags = Flags.CreatePropertyFlags(Visibility, PropertyBuilderFlags.NoCtor);
            SetPropertyFieldDefaults(pProperty, Offset);
        }
        unsafe { ((FArrayProperty*)Alloc)->Inner = (UE.Toolkit.Core.Types.Unreal.UE5_4_4.FProperty*)Inner.Ptr; }
        LinkToPropertyList(NewProperty, ClassReflection);
        Inner.SetOwnerFField(NewProperty);
        return true;
    }

    public override bool CreateArray<TOwner>(out IFArrayProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility,
        IFProperty Inner)
    {
        NewProperty = null;
        if (!TryGetClassAndProperty<TOwner>("ArrayProperty", out var ClassReflection, out var PropertyClass))
            return false;
        return CreateArrayInner(out NewProperty, Name, Offset, Visibility, ClassReflection, PropertyClass, Inner);
    }
    
    public override bool CreateArray(out IFArrayProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility,
        IFProperty Inner)
    {
        NewProperty = null;
        if (!GetProperty("ArrayProperty", out var PropertyClass))
            return false;
        return CreateArrayInner(out NewProperty, Name, Offset, Visibility, null, PropertyClass, Inner);
    }
    
    private bool CreateMapInner(
        out IFMapProperty? NewProperty, 
        string Name, 
        int Offset,
        PropertyVisibility Visibility,
        IUStruct? ClassReflection,
        FieldClassGlobal PropertyClass,
        IFProperty Key,
        IFProperty Value
    )
    {
        NewProperty = null;
        var Alloc = Memory.Malloc(Marshal.SizeOf<FMapProperty>(), FIELD_ALIGNMENT);
        NewProperty = Factory.CreateFMapProperty(Alloc);
        if (ClassReflection != null) SetPropertySuperFields(Factory.CreateFField(Alloc), Name, ClassReflection, PropertyClass);
        else SetPropertySuperFieldsNoOwner(Factory.CreateFField(Alloc), Name, PropertyClass);
        unsafe
        {
            var pProperty = (FProperty*)Alloc;
            pProperty->array_dim = 1;
            pProperty->element_size = 0x50; // sizeof(TMap<K, V>), usually
            pProperty->property_flags = Flags.CreatePropertyFlags(Visibility, PropertyBuilderFlags.NoCtor);
            SetPropertyFieldDefaults(pProperty, Offset);
        }
        unsafe { ((FMapProperty*)Alloc)->KeyProp = (UE.Toolkit.Core.Types.Unreal.UE5_4_4.FProperty*)Key.Ptr; }
        unsafe { ((FMapProperty*)Alloc)->ValueProp = (UE.Toolkit.Core.Types.Unreal.UE5_4_4.FProperty*)Value.Ptr; }
        LinkToPropertyList(NewProperty, ClassReflection);
        Key.SetOwnerFField(NewProperty);
        Value.SetOwnerFField(NewProperty);
        return true;
    }
    
    public override bool CreateMap<TOwner>(out IFMapProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility,
        IFProperty Key, IFProperty Value)
    {
        NewProperty = null;
        if (!TryGetClassAndProperty<TOwner>("MapProperty", out var ClassReflection, out var PropertyClass))
            return false;
        return CreateMapInner(out NewProperty, Name, Offset, Visibility, ClassReflection, PropertyClass, Key, Value);
    }
    
    public override bool CreateMap(out IFMapProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility,
        IFProperty Key, IFProperty Value)
    {
        NewProperty = null;
        if (!GetProperty("MapProperty", out var PropertyClass))
            return false;
        return CreateMapInner(out NewProperty, Name, Offset, Visibility, null, PropertyClass, Key, Value);
    }
    
    private bool CreateEnumInner(
        out IFEnumProperty? NewProperty,
        string Name,
        int Offset,
        PropertyVisibility Visibility,
        IUStruct? ClassReflection,
        FieldClassGlobal PropertyClass,
        IUEnum Enum
    )
    {
        var Alloc = Memory.Malloc(Marshal.SizeOf<FEnumProperty>(), FIELD_ALIGNMENT);
        NewProperty = Factory.CreateFEnumProperty(Alloc);
        if (ClassReflection != null) SetPropertySuperFields(Factory.CreateFField(Alloc), Name, ClassReflection, PropertyClass);
        else SetPropertySuperFieldsNoOwner(Factory.CreateFField(Alloc), Name, PropertyClass);
        unsafe
        {
            var pProperty = (FProperty*)Alloc;
            pProperty->array_dim = 1;
            pProperty->element_size = Enum.SizeOf();
            pProperty->property_flags = Flags.CreatePropertyFlags(Visibility, PropertyBuilderFlags.NoCtor);
            SetPropertyFieldDefaults(pProperty, Offset);
        }
        IFProperty? ReprProp;
        CreateEnumUnderlyingType CreateNewProp = NewProperty.ElementSize switch
        {
            1 => CreateU8,
            2 => CreateU16,
            4 => CreateU32,
            _ => CreateU64
        };
        CreateNewProp(out ReprProp, Name, 0, Visibility);
        unsafe { ((FEnumProperty*)Alloc)->UnderlyingProp = (UE.Toolkit.Core.Types.Unreal.UE5_4_4.FProperty*)ReprProp.Ptr; }
        unsafe { ((FEnumProperty*)Alloc)->Enum = (UEnum*)Enum.Ptr; }
        LinkToPropertyList(NewProperty, ClassReflection);
        ReprProp.SetOwnerFField(NewProperty);
        return true;
    }
    
    public override bool CreateEnum<TOwner, TEnum>(out IFEnumProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility)
    {
        NewProperty = null;
        if (!TryGetClassAndProperty<TOwner>("EnumProperty", out var ClassReflection, out var PropertyClass)
            || !Classes.GetEnumInfoFromType<TEnum>(out var Enum))
            return false;
        return CreateEnumInner(out NewProperty, Name, Offset, Visibility, ClassReflection, PropertyClass, Enum);
    }

    public override bool CreateEnum<TEnum>(out IFEnumProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility)
    {
        NewProperty = null;
        if (!GetProperty("EnumProperty", out var PropertyClass)
            || !Classes.GetEnumInfoFromType<TEnum>(out var Enum))
            return false;
        return CreateEnumInner(out NewProperty, Name, Offset, Visibility, null, PropertyClass, Enum);
    }

    public override bool CreateEnum(out IFEnumProperty? NewProperty, string Name, string TypeName, int Offset,
        PropertyVisibility Visibility)
    {
        NewProperty = null;
        if (!GetProperty("EnumProperty", out var PropertyClass)
            || !Classes.GetEnumInfoFromName(TypeName, out var Enum))
            return false;
        return CreateEnumInner(out NewProperty, Name, Offset, Visibility, null, PropertyClass, Enum);
    }
}