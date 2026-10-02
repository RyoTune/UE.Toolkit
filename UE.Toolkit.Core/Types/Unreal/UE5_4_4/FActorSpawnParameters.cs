using System.Runtime.InteropServices;

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Explicit, Size = 0x80)]
public unsafe struct FActorSpawnParameters
{
    /* A name to assign as the Name of the Actor being spawned. If no value is specified, the name of the spawned Actor will be automatically generated using the form [Class]_[Number]. */
    [FieldOffset(0x0)] public FName Name;
    
    /* An Actor to use as a template when spawning the new Actor. The spawned Actor will be initialized using the property values of the template Actor. If left NULL the class default object (CDO) will be used to initialize the spawned Actor. */
    [FieldOffset(0x8)] public AActor* Template;
    
    /* The Actor that spawned this Actor. (Can be left as NULL). */
    [FieldOffset(0x10)] public AActor* Owner;
    
    /* The APawn that is responsible for damage done by the spawned Actor. (Can be left as NULL). */
    [FieldOffset(0x18)] public AActor* Instigator;
    
    /* The ULevel to spawn the Actor in, i.e. the Outer of the Actor. If left as NULL the Outer of the Owner is used. If the Owner is NULL the persistent level is used. */
    [FieldOffset(0x20)] public UObjectBase* OverrideLevel;
    
    [FieldOffset(0x28)] public UObjectBase* OverrideParentComponent;
    
    /* The parent component to set the Actor in. */
    [FieldOffset(0x34)] public EObjectFlags ObjectFlags;
}