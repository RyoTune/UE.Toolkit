using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using UE.Toolkit.Core.Types.Unreal.Common;

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Sequential)]
public struct Vector4Double
{
    public double X;
    public double Y;
    public double Z;
    public double W;

    public Vector4Double(Vector4 value)
    {
        X = value.X;
        Y = value.Y;
        Z = value.Z;
        W = value.W;
    }

    public Vector4Double(Quaternion value)
    {
        X = value.X;
        Y = value.Y;
        Z = value.Z;
        W = value.W;
    }

    public Vector4 AsVector4() => new((float)X, (float)Y, (float)Z, (float)W);
    public Quaternion AsQuaternion() => new((float)X, (float)Y, (float)Z, (float)W);
}

[StructLayout(LayoutKind.Explicit, Size = 0x60, Pack = 16)]
public unsafe struct FTransform
{
    [FieldOffset(0x0)] public Vector4Double Rotation;
    [FieldOffset(0x20)] public Vector4Double Position;
    [FieldOffset(0x40)] public Vector4Double Scale3D;

    public FTransform()
    {
        Rotation = new (Quaternion.Identity);
        Position = new(Vector4.Zero);
        Scale3D = new(Vector4.One - Vector4.UnitW); 
    }
    
    public FTransform(Vector4 position)
    {
        Rotation = new(Quaternion.Identity);
        Position = new(position);
        Scale3D = new(Vector4.One - Vector4.UnitW);
    }
    
    public FTransform(Quaternion rotation)
    {
        Rotation = new(rotation);
        Position = new(Vector4.Zero);
        Scale3D = new(Vector4.One - Vector4.UnitW);
    }
    
    public FTransform(Vector4 position, Quaternion rotation)
    {
        Rotation = new(rotation);
        Position = new(position);
        Scale3D = new(Vector4.One - Vector4.UnitW);
    }

    public FTransform(Vector4 position, Quaternion rotation, Vector4 scale3D)
    {
        Rotation = new(rotation);
        Position = new(position);
        Scale3D = new(scale3D);
    }
}

public class Transform(Ptr<FTransform> inner) : ITransform
{
    private Ptr<FTransform> Inner { get; } = inner;

    public unsafe nint Ptr => (nint)Inner.Value;
}