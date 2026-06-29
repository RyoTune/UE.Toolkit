using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;
using UE.Toolkit.Core.Common;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Common.GameConfigs;
using Void = Reloaded.Hooks.Definitions.Structs.Void;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Reloaded.Unreal;

public unsafe class UnrealObjects : IUnrealObjects
{
    private delegate FText* FText_FromString(FText* ptr, FString* str);
    private readonly SHFunction<FText_FromString>? _FText_FromString = new();

    private delegate FString* FText_ToString(FText* text);
    private readonly SHFunction<FText_ToString> _FText_ToString = new();
    
    private static IHook<PostLoadSubobjectsFunction>? _UObject_PostLoadSubobjects;
    private static IHook<BeginDestroyFunction>? _UObject_BeginDestroy;
    private static Action<nint>? _onObjectLoaded;
    private static Action<nint>? _onObjectDestroy;

    internal ConcurrentDictionary<nint, Action<IUClass>> OnCDOLoaded = new();
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void UObject_PostLoadSubobjects(nint self, nint outerInstanceGraph)
    {
        if (Mod.Config.LogObjectsEnabled)
            Log.Information($"{nameof(UObject_PostLoadSubobjects)} || {ToolkitUtils.GetPrivateName(self)} || {ToolkitUtils.GetPrivateName((nint)((UObjectBase*)self)->ClassPrivate)}");
        
        _UObject_PostLoadSubobjects!.OriginalFunction.Value.Invoke(self, outerInstanceGraph);
        _onObjectLoaded?.Invoke(self);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void UObject_BeginDestroy(nint self)
    {
        if (Mod.Config.LogObjectsEnabled)
            Log.Information($"{nameof(UObject_BeginDestroy)} || {ToolkitUtils.GetPrivateName(self)} || {ToolkitUtils.GetPrivateName((nint)((UObjectBase*)self)->ClassPrivate)}");
        
        _UObject_BeginDestroy!.OriginalFunction.Value.Invoke(self);
        _onObjectDestroy?.Invoke(self);
    }

    private IUnrealFactory Factory;

    public UnrealObjects(IUnrealFactory factory)
    {
        Factory = factory;
        
        Project.Scans.AddScanHook(nameof(UObject_PostLoadSubobjects),
            (result, hooks) => _UObject_PostLoadSubobjects = hooks.CreateHook<PostLoadSubobjectsFunction>((delegate* unmanaged[Stdcall]<nint, nint, void>)&UObject_PostLoadSubobjects, result).Activate());
        
        Project.Scans.AddScanHook(nameof(UObject_BeginDestroy), 
            (result, hooks) => _UObject_BeginDestroy = hooks.CreateHook<BeginDestroyFunction>((delegate* unmanaged[Stdcall]<nint, void>)&UObject_BeginDestroy, result).Activate());
        
        Project.Scans.AddScan(nameof(GUObjectArray),
            result => GUObjectArray = factory.CreateUObjectArray(result));
        
        Project.Scans.AddScanHook(nameof(UStruct_IsChildOf),
            (result, hooks) => UStruct.UStruct_IsChildOf = hooks.CreateWrapper<UStruct_IsChildOf>(result, out _));
        
        _onObjectLoaded += objPtr => OnObjectLoaded?.Invoke(new((UObjectBase*)objPtr));
        _onObjectDestroy += objPtr => OnObjectBeginDestroy?.Invoke(new((UObjectBase*)objPtr));
    }

    public Action<ToolkitUObject<UObjectBase>>? OnObjectLoaded { get; set; }

    public Action<ToolkitUObject<UObjectBase>>? OnObjectBeginDestroy { get; set; }
    
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
        var ftext = (FText*)UnrealMemory._FMemory!.Malloc(GameConfig.GetFTextSize());
        _FText_FromString!.Wrapper(ftext, fstring);
        UnrealMemory._FMemory.Free((nint)fstring);
        
        return ftext;
    }
    
    public string FTextToString(FText* text) => _FText_ToString.Wrapper(text)->ToString();

    public FString* CreateFString(string content) => UnrealStringsStatic.CreateFString(content);

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    private struct PostLoadSubobjectsFunction
    {
        public FuncPtr<nint, nint, Void> Value;
    }

    private struct BeginDestroyFunction
    {
        public FuncPtr<nint, Void> Value;
    }
}