using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Common.GameConfigs;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class TextNode(IFProperty property, Ptr<FText> value, IUnrealObjects unrealObjects) 
    : TextPrimitiveNode<IFProperty, FText>(property, value)
{
    private IUnrealObjects UnrealObjects => unrealObjects;
    
    protected override unsafe void SetField(string text)
    {
        var ftext = UnrealObjects.CreateFText(text);
        *Value.Value = *ftext;
    }

    protected override unsafe void SetInitialValue()
    {
        if (InitialValue == null)
        {
            var allocSize = GameConfig.Instance.GetFTextSize();
            InitialValue = new byte[allocSize];
            Marshal.Copy((nint)Value.Value, InitialValue, 0, allocSize);   
        }
    }
}