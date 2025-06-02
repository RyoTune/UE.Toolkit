using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;
using UE.Toolkit.Core.Common;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Core.Types.Wrappers;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Common.GameConfigs;
using Void = Reloaded.Hooks.Definitions.Structs.Void;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Reloaded.Unreal;

public unsafe class UnrealObjects : IUnrealObjects
{
    private delegate FText* FText_FromString(FText* ptr, FString* str);
    private readonly SHFunction<FText_FromString>? _FText_FromString = new("48 89 5C 24 ?? 57 48 83 EC 40 83 7A ?? 01 48 8B D9 48 89 74 24 ?? 4C 89 74 24 ?? C7 44 24 ?? 00 00 00 00 7F ?? E8 ?? ?? ?? ?? 48 8B F0 48 8B 38 48 89 7C 24 ?? 48 85 FF 74 ?? 48 8B 17 48 8B CF FF 52 ?? 8B 46 ?? 4C 8D 74 24 ?? 89 44 24 ?? BE 01 00 00 00 48 8B CF EB ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 7C 24 ?? 4C 8B F0 BE 02 00 00 00 48 8B 08 48 89 0B 48 85 C9 74 ?? 48 8B 11 FF 52 ?? 41 8B 46 ?? 4C 8B 74 24 ?? 89 43 ?? 40 F6 C6 02 74 ?? 48 8B 4C 24 ?? 83 E6 FD 48 85 C9 74 ?? 48 8B 01 FF 50 ?? 40 F6 C6 01 48 8B 74 24 ?? 74 ?? 48 85 FF 74 ?? 48 8B 07 48 8B CF FF 50 ?? 83 4B ?? 12");

    private delegate FString* FText_ToString(FText* text);
    private readonly SHFunction<FText_ToString> _FText_ToString =
        new("40 53 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 48 8B 0B 48 8B 01 48 83 C4 20");
    
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
        ScanHooks.Add(nameof(UObject_PostLoadSubobjects),
            GameConfig.Instance.UObject_PostLoadSubobjects,
            (hooks, result) => _UObject_PostLoadSubobjects = hooks.CreateHook<PostLoadSubobjectsFunction>((delegate* unmanaged[Stdcall]<nint, nint, void>)&UObject_PostLoadSubobjects, result).Activate());
        
        ScanHooks.Add(nameof(GUObjectArray), GameConfig.Instance.GUObjectArray,
            (_, result) => GUObjectArray = factory.CreateUObjectArray(GameConfig.Instance.GUObjectArray_Result(result)));
        
        ScanHooks.Add(nameof(UStruct_IsChildOf), GameConfig.Instance.UStruct_IsChildOf,
            (hooks, result) => UStruct.UStruct_IsChildOf = hooks.CreateWrapper<UStruct_IsChildOf>(result, out _));
        
        _getDispNameTextByIdx = new(GameConfig.Instance.UEnum_GetDisplayNameTextByIndex);
    }

    public IUObjectArray GUObjectArray { get; private set; } = null!;

    public void OnObjectLoadedByName<TObject>(string objName, Action<UObjectWrapper<TObject>> callback)
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

    public void OnObjectLoadedByName<TObject>(Action<UObjectWrapper<TObject>> callback)
        where TObject : unmanaged => OnObjectLoadedByName(typeof(TObject).Name, callback);

    public void OnObjectLoadedByClass<TObject>(string objClass, Action<UObjectWrapper<TObject>> callback)
        where TObject : unmanaged
    {
        _onObjectLoaded += objPtr =>
        {
            if (((UObjectBase*)objPtr)->IsChildOf(objClass)) callback(new((TObject*)objPtr));
        };
    }

    public void OnObjectLoadedByClass<TObject>(Action<UObjectWrapper<TObject>> callback)
        where TObject : unmanaged => OnObjectLoadedByClass(typeof(TObject).Name, callback);

    public FText* CreateFText(string content)
    {
        var fstring = CreateFString(content);
        var ftext = (FText*)UnrealMemory._FMemory!.Malloc(sizeof(FText));
        _FText_FromString!.Wrapper(ftext, fstring);
        UnrealMemory._FMemory!.Free((nint)fstring);
        
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