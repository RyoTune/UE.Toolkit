using System.Diagnostics.CodeAnalysis;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

public interface IUEnum : IUField
{
    string CppType { get; }
    
    TArray<TPair<FName, long>> Names { get; }

    unsafe bool TryParse(string name, bool ignoreCase, [NotNullWhen(true)] out long? value)
    {
        value = null;
        if (ignoreCase) name = name.ToLower();
        for (var i = 0; i < Names.ArrayNum; i++)
        {
            var Discriminant = &Names.AllocatorInstance[i];
            var CheckName = Discriminant->Key.ToString();
            var CheckNameParts = CheckName.Split("::", 2);
            // 1.10.2: Don't break fully qualified names in case there are mods out there that define their enums like that
            CheckName = CheckNameParts.Length > 1 && !name.Contains("::") ? CheckNameParts[1] : CheckName;
            if (ignoreCase)
                CheckName = CheckName.ToLower();
            if (CheckName == name)
            {
                value = Discriminant->Value;
                return true;
            }
        }
        return false;
    }

    int SizeOf()
    {
        long maxValue = 0;
        for (var i = 0; i < Names.ArrayNum; i++)
        {
            unsafe
            {
                var currentValue = Names.AllocatorInstance[i].Value;
                if (currentValue > maxValue)
                    maxValue = currentValue;
            }
        }
        return maxValue switch
        {
            <= byte.MaxValue => 1,
            <= ushort.MaxValue => 2,
            <= uint.MaxValue => 4,
            _ => 8
        };   
    }
}