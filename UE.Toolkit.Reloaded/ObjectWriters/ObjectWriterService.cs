using System.Xml;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Interfaces.ObjectWriters;
using UE.Toolkit.Reloaded.ObjectWriters.Nodes;

// ReSharper disable NotAccessedPositionalProperty.Local

namespace UE.Toolkit.Reloaded.ObjectWriters;

public class ObjectWriterService(IUnrealObjects uobjs, IDataTables dt, IUnrealMemory memory, 
    IUnrealClasses unrealClasses, IUnrealFactory unrealFactory)
{
    private readonly List<ObjectWriter> _objWriters = [];
    private readonly NodeFactory _nodeFactory = new(uobjs, memory, unrealClasses, unrealFactory);

    public void AddPath(string path)
    {
        if (File.Exists(path)) RegisterFile(path);
        else if (Directory.Exists(path)) RegisterFolder(path);
        else Log.Error($"{nameof(ObjectWriterService)} || Invalid path: {path}");
    }
    
    private void RegisterFolder(string folder)
    {
        foreach (var objFile in Directory.EnumerateFiles(folder, "*.obj.xml", SearchOption.AllDirectories))
        {
            try
            {
                RegisterFile(objFile);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(ObjectWriterService)} || Failed to register object file.\nFile: {objFile}");
            }
        }
    }

    private unsafe void RegisterFile(string objFile)
    {
        var objName = Path.GetFileName(objFile).Replace(".obj.xml", string.Empty);
            
        using var reader = XmlReader.Create(File.OpenRead(objFile));
        reader.MoveToContent();

        var rootTypeName = reader.Name;
        var rootTypePath = reader.GetAttribute("path");

        var objWriter = new ObjectWriter(objName, rootTypeName, objFile, _nodeFactory, rootTypePath);
        _objWriters.Add(objWriter);

        if (rootTypeName == "DataTable")
        {
            Action<string, Action<ToolkitDataTable<UObjectBase>>> Callback =
                objWriter.ObjectPath != null ? dt.OnDataTableChangedByPath : dt.OnDataTableChanged;
            Callback(objWriter.ObjectPath ?? objWriter.ObjectName, x => objWriter.WriteToObject((nint)x.Self));
        }
        else
        {
            Action<string, Action<ToolkitUObject<UObjectBase>>> Callback =
                objWriter.ObjectPath != null ? uobjs.OnObjectLoadedByPath : uobjs.OnObjectLoadedByName;
            Callback(objWriter.ObjectPath ?? objWriter.ObjectName, x => objWriter.WriteToObject((nint)x.Self));
        }
        
        Log.Information($"{nameof(ObjectWriterService)} || Object XML registered: {objWriter.GetObjectNameOrPath()}\nFile: {objFile}");
    }
}