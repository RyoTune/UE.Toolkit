using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class SoftClassNode(IFProperty property, Ptr<FSoftObjectPtr> value) 
    : TextPrimitiveNode<IFProperty, FSoftObjectPtr>(property, value)
{
    protected override unsafe void SetField(string text)
    {
        Value.Value->Super.ObjectId.AssetPath.PackageName = new FName(text);
        Log.Debug($"{nameof(SoftClassNode)} || Field '{property.NamePrivate}' at 0x{Value:X} set to: {text}");
    }

    protected override unsafe void SetInitialValue()
    {
        if (InitialValue == null)
        {
            InitialValue = new byte[sizeof(FSoftObjectPtr)];
            Marshal.Copy((nint)Value.Value, InitialValue, 0, sizeof(FSoftObjectPtr));
        }
    }
}