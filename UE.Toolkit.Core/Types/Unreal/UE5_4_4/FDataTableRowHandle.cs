using System.Runtime.InteropServices;

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FDataTableRowHandle
{
    public UDataTable<UObjectBase>* DataTable;
    public FName RowName;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FDataTableRowHandle<TRow>
 where TRow : unmanaged
{
    public UDataTable<TRow>* DataTable;
    public FName RowName;
}