using System.Collections.Concurrent;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Common;

public static unsafe class ToolkitUtils
{
    private static readonly ConcurrentDictionary<FName, string> PrivateToNativeNameMap = [];

    public static nint GetGlobalAddress(nint address) => *(int*)address + address + 4;
    
    public static string GetPrivateName(nint objPtr) => ((UObjectBase*)objPtr)->NamePrivate.ToString();

    /// <summary>
    /// Gets the <c>UObject</c>'s (expected) native name.<br/>
    /// This is different from its private name, which has no type prefix (the <c>U</c> in <c>UObject</c> for example),
    /// typically...<br/>
    /// VERY SLOW!!!
    /// </summary>
    /// <param name="objPtr">UObject instance</param>
    /// <returns><c>UObject</c> native name.</returns>
    public static string GetNativeName(nint objPtr)
    {
        var uobj = (UObjectBase*)objPtr;
        if (PrivateToNativeNameMap.TryGetValue(uobj->NamePrivate, out var nameNative)) return nameNative;
        
        var namePrivate = uobj->NamePrivate.ToString();
        nameNative = namePrivate;
        
        if (uobj->IsChildOf<UClass>())
        {
            nameNative = $"U{namePrivate}";
        }
        if (uobj->IsChildOf<AActor>())
        {
            nameNative = $"A{namePrivate}";
        }
        if (uobj->IsChildOf<UScriptStruct>())
        {
            // Already has the F struct prefix, UserDefinedStructs usually I think.
            var hasStructPrefix = namePrivate[0] == 'F' && char.IsUpper(namePrivate[1]);
            if (!hasStructPrefix)
            {
                nameNative = $"F{namePrivate}";
            }
        }

        PrivateToNativeNameMap[uobj->NamePrivate] = nameNative;
        return nameNative;
    }
    
    public static string GetNativeName(IUObject uobj) => GetNativeName(uobj.Ptr);

    /// <summary>
    /// Returns the fully qualified pathname for this object, in the format "Outermost.[Outer:]Name"
    /// </summary>
    /// <param name="objPtr">Pointer to the object</param>
    /// <returns>Fully qualified pathname for this object</returns>
    public static string GetPathName(nint objPtr)
    {
        // From UObjectBaseUtility::GetPathName
        var Object = (UObjectBase*)objPtr;
        if (Object == null) return "None";
        
        var Outer = Object->OuterPrivate;
        var Result = Object->NamePrivate.ToString();
        var OuterResult = string.Empty;
        if (Outer != null)
        {
            var bIsOuterPackage = ((UObjectBase*)Outer->ClassPrivate)->NamePrivate.ToString() != "Package"
                                  && ((UObjectBase*)Outer->OuterPrivate->ClassPrivate)->NamePrivate.ToString() == "Package";
            OuterResult = GetPathName((nint)Outer) + (bIsOuterPackage ? ":" : ".");
        }
        return OuterResult + Result;
    }
    
    public static string GetPathName(IUObject uobj) => GetPathName(uobj.Ptr);
}