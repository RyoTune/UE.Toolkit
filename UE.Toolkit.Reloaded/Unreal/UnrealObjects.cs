using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;
using UE.Toolkit.Core.Common;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;
using Void = Reloaded.Hooks.Definitions.Structs.Void;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Reloaded.Unreal;

public unsafe class UnrealObjects : IUnrealObjects
{
    private delegate FText* FText_FromString(FText* ptr, FString* str);
    private readonly SHFunction<FText_FromString>? _FText_FromString = new();

    private delegate FString* FText_ToString(FText* text);
    private readonly SHFunction<FText_ToString> _FText_ToString = new();
    
    private delegate FText* UEnum_GetDisplayNameTextByIndex(UUserDefinedEnum* userEnum, FText* outName, int index);
    private readonly SHFunction<UEnum_GetDisplayNameTextByIndex> _getDispNameTextByIdx;
    
    private static IHook<PostLoadSubobjectsFunction>? _UObject_PostLoadSubobjects;
    private static Action<nint>? _onObjectLoaded;
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void UObject_PostLoadSubobjects(nint self, nint outerInstanceGraph)
    {
        if (Mod.Config.LogObjectsEnabled)
            Log.Information($"{nameof(UObject_PostLoadSubobjects)} || {ToolkitUtils.GetPrivateName(self)} || {ToolkitUtils.GetPrivateName((nint)((UObjectBase*)self)->ClassPrivate)}");
        
        _UObject_PostLoadSubobjects!.OriginalFunction.Value.Invoke(self, outerInstanceGraph);
        _onObjectLoaded?.Invoke(self);
    }

    public UnrealObjects(IUnrealFactory factory)
    {
        Project.Scans.AddScanHook(nameof(UObject_PostLoadSubobjects),
            (result, hooks) => _UObject_PostLoadSubobjects = hooks.CreateHook<PostLoadSubobjectsFunction>((delegate* unmanaged[Stdcall]<nint, nint, void>)&UObject_PostLoadSubobjects, result).Activate());
        
        Project.Scans.AddScan(nameof(GUObjectArray),
            result => GUObjectArray = factory.CreateUObjectArray(result));
        
        Project.Scans.AddScanHook(nameof(UStruct_IsChildOf),
            (result, hooks) => UStruct.UStruct_IsChildOf = hooks.CreateWrapper<UStruct_IsChildOf>(result, out _));
        
        _getDispNameTextByIdx = new();

        _onObjectLoaded += objPtr => OnObjectLoaded?.Invoke(new((UObjectBase*)objPtr));
    }

    public Action<ToolkitUObject<UObjectBase>>? OnObjectLoaded { get; set; }
    
    public IUObjectArray GUObjectArray { get; private set; } = null!;

    public void OnObjectLoadedByName<TObject>(string objName, Action<ToolkitUObject<TObject>> callback)
        where TObject : unmanaged
    {
        var ansiNameBytes = Marshal.StringToHGlobalAnsi(objName);
        _onObjectLoaded += objPtr =>
        {
            if (((UObjectBase*)objPtr)->NamePrivate.ToSpanAnsi().SequenceEqual(new((void*)ansiNameBytes, objName.Length)))
            {
                callback(new((TObject*)objPtr));
            }
        };
    }

    public void OnObjectLoadedByName<TObject>(Action<ToolkitUObject<TObject>> callback)
        where TObject : unmanaged => OnObjectLoadedByName(typeof(TObject).Name, callback);

    public void OnObjectLoadedByClass<TObject>(string objClass, Action<ToolkitUObject<TObject>> callback)
        where TObject : unmanaged
    {
        _onObjectLoaded += objPtr =>
        {
            if (((UObjectBase*)objPtr)->IsChildOf(objClass)) callback(new((TObject*)objPtr));
        };
    }

    public void OnObjectLoadedByClass<TObject>(Action<ToolkitUObject<TObject>> callback)
        where TObject : unmanaged => OnObjectLoadedByClass(typeof(TObject).Name, callback);

    public FText* CreateFText(string content)
    {
        var fstring = CreateFString(content);
        var ftext = (FText*)UnrealMemory._FMemory!.Malloc(sizeof(FText));
        _FText_FromString!.Wrapper(ftext, fstring);
        UnrealMemory._FMemory.Free((nint)fstring);
        
        return ftext;
    }
    
    public string FTextToString(FText* text) => _FText_ToString.Wrapper(text)->ToString();

    public FString* CreateFString(string content)
    {
        content += '\0';
        
        var fstring = (FString*)UnrealMemory._FMemory!.Malloc(sizeof(FString));
        fstring->Data.ArrayNum = content.Length;
        fstring->Data.ArrayMax = content.Length;
        
        var strBytes = Encoding.Unicode.GetBytes(content);
        fstring->Data.AllocatorInstance = (char*)UnrealMemory._FMemory.Malloc(strBytes.Length);
        Marshal.Copy(strBytes, 0, (nint)fstring->Data.AllocatorInstance, strBytes.Length);
        
        return fstring;
    }

    public string UEnumGetDisplayNameTextByIndex(nint userEnum, int index)
    {
        var outText = (FText*)Marshal.AllocHGlobal(sizeof(FText));
        
        var dispNameFText =
            _getDispNameTextByIdx.Wrapper((UUserDefinedEnum*)userEnum, outText, index);
        var dispName = FTextToString(dispNameFText);
        
        Marshal.FreeHGlobal((nint)outText);
        return dispName;
    }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    private struct PostLoadSubobjectsFunction
    {
        public FuncPtr<nint, nint, Void> Value;
    }
}