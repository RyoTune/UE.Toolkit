using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class SoftClassNode(IFProperty property, Ptr<FSoftObjectPtr> value, IUnrealClasses classes)
    : TextPrimitiveNode<IFProperty, FSoftObjectPtr>(property, value)
{
    private IUnrealClasses Classes => classes;
    
    protected override unsafe void SetField(string text)
    {
        var SoftObjectPath = Classes.IntoSoftObjectPath((nint)(&Value.Value->Super.ObjectId));
        SoftObjectPath.SetAssetPath(text);
        Log.Debug($"{nameof(SoftClassNode)} || Field '{property.NamePrivate}' at 0x{(nint)Value.Value:X} set to '{text}'");
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