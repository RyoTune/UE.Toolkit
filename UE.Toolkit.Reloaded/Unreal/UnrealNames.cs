using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;
using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Reloaded.Unreal;

public unsafe class UnrealNames : IUnrealNames
{
    private static IHook<FNameCtorWideFunction>? _fnameCtorWideHook;
    private static readonly Dictionary<string, string> _redirectedFNames = [];
    private static IHook<FNamePoolStoreFunction>? _fnamePoolStoreHook;
    
    public UnrealNames()
    {
        Project.Scans.AddScan(nameof(FName.GFNamePool), result => FName.GFNamePool = (FNamePool*)result);
        
        // TODO: While UE games should all have a FName Unicode ctor,
        //       it seems like UE5 itself does not use it often? Update: Still used a lot, hmmm...
        //       For FName modding, hooking the below function might be the better option in UE5.
        // Project.Scans.AddScanHook(nameof(FNameHelper_FindOrStoreString),
        //     (result, hooks) =>
        //     {
        //         FName.FNameHelper_FindOrStoreString =
        //             hooks.CreateWrapper<FNameHelper_FindOrStoreString>(result, out _);
        //     });
        
        Project.Scans.AddScanHook(nameof(FName_Ctor_Wide),
            (result, hooks) =>
            {
                FName.FName_Constructor = hooks.CreateWrapper<FName_Ctor_Wide>(result, out _);
                
                if (Mod.Config.FNamesEnabled)
                    _fnameCtorWideHook = hooks.CreateHook<FNameCtorWideFunction>((delegate* unmanaged[Stdcall]<nint, nint, EFindName, nint>)&FName_Ctor_Wide, result).Activate();
            });
        
        Project.Scans.AddScanHook(nameof(FNamePool_Store), (result, hooks) =>
        {
            _fnamePoolStoreHook = hooks.CreateHook<FNamePoolStoreFunction>((delegate* unmanaged[Stdcall]<Ptr<FNamePool>, nint, nint, int>)&FNamePool_Store, result).Activate();
        });
    }

    public void RedirectFName(string modName, string fname, string newValue)
    {
        if (fname == newValue)
        {
            Log.Error($"{nameof(RedirectFName)} || Attempted to redirect FName to itself. This is considered an error and should be fixed.\nMod: {modName} || String: {newValue}");
            return;
        }

        _redirectedFNames[fname] = newValue;
        Log.Debug($"{nameof(RedirectFName)} || Redirected FName: {fname}\nMod: {modName} || New Value: {newValue}");
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint FName_Ctor_Wide(nint self, nint name, EFindName findType)
    {
        // Only deference names if it's actually used.
        if (Mod.Config.LogFNames || _redirectedFNames.Count > 0)
        {
            var nameStr = Marshal.PtrToStringUni(name);
            if (nameStr != null)
            {
                if (Mod.Config.LogFNames) Log.Information(nameStr);

                if (!string.IsNullOrEmpty(nameStr) &&_redirectedFNames.TryGetValue(nameStr, out var newValue))
                {
                    name = newValue.AsPointerUni(true);
                }
            }
        }
        
        return _fnameCtorWideHook!.OriginalFunction.Value.Invoke(self, name, findType);
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static int FNamePool_Store(Ptr<FNamePool> self, nint a2, nint a3)
    {
        FName.GFNamePool = self.Value;
        return _fnamePoolStoreHook!.OriginalFunction.Value.Invoke(self, a2, a3);
    }
    
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    private struct FNameCtorWideFunction
    {
        public FuncPtr<nint, nint, EFindName, nint> Value;
    }

    private struct FNamePoolStoreFunction
    {
        public FuncPtr<Ptr<FNamePool>, nint, nint, int> Value;
    }
}