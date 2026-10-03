using System.Numerics;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.Common;

namespace UE.Toolkit.Core.Types.Unreal.UE4_27_2;

[StructLayout(LayoutKind.Explicit, Size = 0x30, Pack = 16)]
public struct FTransform : ITransform
{
    [FieldOffset(0x0)] public Quaternion Rotation;
    [FieldOffset(0x10)] public Vector4 Position;
    [FieldOffset(0x20)] public Vector4 Scale3D;

    public FTransform()
    {
        Rotation = Quaternion.Identity;
        Position = Vector4.Zero;
        Scale3D = Vector4.One - Vector4.UnitW; 
    }

    public unsafe nint Ptr
    {
        get
        {
            fixed (FTransform* self = &this)
            {
                return (nint)self;
            }
        }
    }
}

public class Transform(Ptr<FTransform> inner) : ITransform
{
    private Ptr<FTransform> Inner { get; } = inner;

    public unsafe nint Ptr => (nint)Inner.Value;
    
}