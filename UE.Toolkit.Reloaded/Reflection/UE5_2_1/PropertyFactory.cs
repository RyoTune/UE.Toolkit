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

    protected override unsafe void LinkToPropertyList(IFProperty Property, IUClass Reflect)
    {
        var pProperty = (FProperty*)Property.Ptr;
        pProperty->prop_link_next = null;
        pProperty->next_ref = null;
        pProperty->dtor_link_next = null;
        pProperty->post_ct_link_next = null;
        
        var pClass = (UClass*)Reflect.Ptr;
        if (((UStruct*)pClass)->PropertyLink == null)
        {
            ((UStruct*)pClass)->PropertyLink = (UE.Toolkit.Core.Types.Unreal.UE5_4_4.FProperty*)pProperty;
            ((UStruct*)pClass)->ChildProperties = (UE.Toolkit.Core.Types.Unreal.UE5_4_4.FField*)pProperty;
        }
        else
        {
            var TargetProp = GetPreviousProperty(Property, Reflect);
            var NextProp = TargetProp.PropertyLinkNext.Any() ? TargetProp.PropertyLinkNext.First() : null;
            var pTargetProp = (FProperty*)TargetProp.Ptr;
            pTargetProp->prop_link_next = pProperty;
            ((FField*)pTargetProp)->next = (FField*)pProperty;
            if (NextProp != null)
            {
                var pNextProp = (FProperty*)NextProp.Ptr;
                pProperty->prop_link_next = pNextProp;
                ((FField*)pProperty)->next = (FField*)pNextProp;
            }
        }       
    }
    
    protected override unsafe void SetPropertySuperFields(IFField Field, string Name, IUClass ClassReflection, 
        FieldClassGlobal PropertyClass)
    {
        var pField = (FField*)Field.Ptr;
        pField->_vtable = PropertyClass.Vtable;
        pField->class_private = (FFieldClass*)PropertyClass.Params.Ptr;
        pField->owner.Object = (UObjectBase*)ClassReflection.Ptr; // UClass*
        pField->next = null;
        pField->name_private = new FName(Name);
        pField->flags_private = EObjectFlags.RF_Public | EObjectFlags.RF_MarkAsNative | EObjectFlags.RF_Transient;
    }
    
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
        => CreateCopyPropertyInner<TOwner, byte, FProperty>(out NewProperty, Name, Offset, "UInt8Property", Visibility);
    
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

    public override bool CreateStruct<TOwner, TField>(out IFStructProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility)
    {
        NewProperty = null;
        if (!TryGetClassAndProperty<TOwner>("StructProperty", out var ClassReflection, out var PropertyClass)
            || !Classes.GetScriptStructInfoFromType<TField>(out var ScriptStruct))
            return false;
        var Alloc = Memory.Malloc(Marshal.SizeOf<FStructProperty>(), FIELD_ALIGNMENT);
        NewProperty = Factory.CreateFStructProperty(Alloc);
        SetPropertySuperFields(Factory.CreateFField(Alloc), Name, ClassReflection!, PropertyClass!);
        unsafe
        {
            var pProperty = (FProperty*)Alloc;
            pProperty->array_dim = 1;
            pProperty->element_size = ScriptStruct!.PropertiesSize; // FExampleStruct mExampleField; 
            pProperty->property_flags = Flags.CreatePropertyFlags(Visibility, PropertyBuilderFlags.None);
            SetPropertyFieldDefaults(pProperty, Offset);
        }
        LinkToPropertyList(NewProperty, ClassReflection!);
        unsafe { ((FStructProperty*)NewProperty.Ptr)->Struct = (UScriptStruct*)ScriptStruct.Ptr; }
        return true;
    }

    public override bool CreateObject<TOwner, TField>(out IFObjectProperty? NewProperty, string Name, int Offset,
        PropertyVisibility Visibility)
    {
        NewProperty = null;
        if (!TryGetClassAndProperty<TOwner>("ObjectProperty", out var ClassReflection, out var PropertyClass)
            || !Classes.GetClassInfoFromClass<TField>(out var FieldClass))
            return false;
        var Alloc = Memory.Malloc(Marshal.SizeOf<FObjectProperty>(), FIELD_ALIGNMENT);
        NewProperty = Factory.CreateFObjectProperty(Alloc);
        SetPropertySuperFields(Factory.CreateFField(Alloc), Name, ClassReflection!, PropertyClass!);
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
        LinkToPropertyList(NewProperty, ClassReflection!);
        unsafe { ((FObjectProperty*)NewProperty.Ptr)->PropertyClass = (UClass*)FieldClass!.Ptr; }
        return true;
    }

    public override bool CreateName<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateCopyPropertyInner<TOwner, FName, FProperty>(out NewProperty, Name, Offset, "NameProperty", Visibility);
    
    public override bool CreateString<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateStringPropertyInner<TOwner, FName, FProperty>(out NewProperty, Name, Offset, "StrProperty", Visibility);
    
    public override bool CreateText<TOwner>(out IFProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility) 
        => CreateTextPropertyInner<TOwner, FName, FProperty>(out NewProperty, Name, Offset, "TextProperty", Visibility);

    public override bool CreateArray<TOwner>(out IFArrayProperty? NewProperty, string Name, int Offset, PropertyVisibility Visibility,
        IFProperty Inner)
    {
        NewProperty = null;
        if (!TryGetClassAndProperty<TOwner>("ArrayProperty", out var ClassReflection, out var PropertyClass))
            return false;
        var Alloc = Memory.Malloc(Marshal.SizeOf<FArrayProperty>(), FIELD_ALIGNMENT);
        NewProperty = Factory.CreateFArrayProperty(Alloc);
        SetPropertySuperFields(Factory.CreateFField(Alloc), Name, ClassReflection!, PropertyClass!);
        unsafe
        {
            var pProperty = (FProperty*)Alloc;
            pProperty->array_dim = 1;
            pProperty->element_size = 0x10; // sizeof(TArray<T>)
            pProperty->property_flags = Flags.CreatePropertyFlags(Visibility, PropertyBuilderFlags.NoCtor);
            SetPropertyFieldDefaults(pProperty, Offset);
        }
        LinkToPropertyList(NewProperty, ClassReflection!);
        unsafe { ((FArrayProperty*)Alloc)->Inner = (UE.Toolkit.Core.Types.Unreal.UE5_4_4.FProperty*)Inner.Ptr; }
        return true;
    }
}