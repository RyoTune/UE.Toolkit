using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class StrNode(IFProperty property, Ptr<FString> value, IUnrealObjects unrealObjects, IUnrealMemoryInternal unrealMemory) 
    : TextPrimitiveNode<IFProperty, FString>(property, value)
{
    private IUnrealObjects UnrealObjects => unrealObjects;
    private IUnrealMemoryInternal UnrealMemory => unrealMemory;
    
    protected override unsafe void SetField(string text)
    {
        var fstring = UnrealObjects.CreateFString(text);
        *Value.Value = *fstring;
        UnrealMemory.Free((nint)fstring);
    }

    protected override unsafe void SetInitialValue()
    {
        if (InitialValue == null)
        {
            InitialValue = new byte[sizeof(FString)];
            Marshal.Copy((nint)Value.Value, InitialValue, 0, sizeof(FString));
        }
    }
}