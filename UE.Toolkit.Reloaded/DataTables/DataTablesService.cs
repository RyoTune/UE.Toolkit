using UE.Toolkit.Core.Common;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Reloaded.DataTables;

public unsafe class DataTablesService : IDataTables
{
    private delegate void UDataTable_HandleDataTableChanged(UDataTable<UObjectBase>* self, FName changedRowName);
    private readonly SHFunction<UDataTable_HandleDataTableChanged>? _HandleDataTableChanged;
    
    private Action<ToolkitDataTable<UObjectBase>>? _onDataTableChanged;
    
    public DataTablesService()
    {
        _HandleDataTableChanged = new(HandleDataTableChangedImpl);
    }

    public void OnDataTableChanged<TRow>(string name, Action<ToolkitDataTable<TRow>> callback)
        where TRow : unmanaged
    {
        _onDataTableChanged += table =>
        {
            if (table.Name == name)
                callback(new((UDataTable<TRow>*)table.Self));
        };
    }
    
    public void OnDataTableChangedByPath<TRow>(string objectPath, Action<ToolkitDataTable<TRow>> callback)
        where TRow : unmanaged
    {
        _onDataTableChanged += table =>
        {
            if (ToolkitUtils.GetPathName((nint)table.Self) == objectPath)
                callback(new((UDataTable<TRow>*)table.Self));
        };
    }
    
    private void HandleDataTableChangedImpl(UDataTable<UObjectBase>* self, FName changedRowName)
    {
        _HandleDataTableChanged!.Hook!.OriginalFunction(self, changedRowName);
        if (self->BaseObj.NamePrivate.ToString() == "None") return;

        var table = new ToolkitDataTable<UObjectBase>(self);
        
        if (Mod.Config.LogTablesEnabled)
        {
            Log.Information($"{nameof(UDataTable_HandleDataTableChanged)} || Table: {table.Name} || Struct: {table.RowStructName}");

            var numRowsLogged = 0;
            foreach (var row in table)
            {
                if (numRowsLogged <= 5)
                {
                    Log.Information($"\t{row.Key}");
                }
                else
                {
                    Log.Information($"\t...with {table.Count - numRowsLogged} more.");
                    break;
                }

                numRowsLogged++;
            }
        }
        
        _onDataTableChanged?.Invoke(table);
    }
}