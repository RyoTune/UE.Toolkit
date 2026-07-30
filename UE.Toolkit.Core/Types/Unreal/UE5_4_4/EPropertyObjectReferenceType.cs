namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

public enum EPropertyObjectReferenceType : uint
{
	None = 0,
	Strong = 1 << 0,
	Weak = 1 << 1
};