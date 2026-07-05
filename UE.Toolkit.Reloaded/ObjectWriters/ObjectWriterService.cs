using System.Xml;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Interfaces.ObjectWriters;
using UE.Toolkit.Reloaded.ObjectWriters.Nodes;

// ReSharper disable NotAccessedPositionalProperty.Local

namespace UE.Toolkit.Reloaded.ObjectWriters;

public class ObjectWriterService(ITypeRegistry typeReg, IUnrealObjects uobjs, IDataTables dt, IUnrealMemory memory)
{
    private readonly Dictionary<TypeKey, Type> _types = [];
    private readonly List<ObjectWriter> _objWriters = [];
    private readonly FieldNodeFactory _nodeFactory = new(typeReg, uobjs, memory);

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
        const string any = "any";
        var objName = Path.GetFileName(objFile).Replace(".obj.xml", string.Empty);
            
        using var reader = XmlReader.Create(File.OpenRead(objFile));
        reader.MoveToContent();

        var rootTypeName = reader.Name;
        var rootTypeProvider = reader.GetAttribute("provider");
        var rootTypePath = reader.GetAttribute("path");
            
        var typeKey = new TypeKey(rootTypeName, rootTypeProvider);
        if (!_types.TryGetValue(typeKey, out var objType))
        {
            // Find type if not previously registered.
            var rootTypeHint = reader.GetAttribute(WriterConstants.HintAttr);
            if (typeReg.TryGetType(rootTypeName, rootTypeHint, rootTypeProvider, out objType))
            {
                _types[typeKey] = objType;
                Log.Debug($"{nameof(ObjectWriterService)} || Registered Type: {rootTypeName} || Provider: {rootTypeProvider ?? any}");
            }
            else
            {
                Log.Error($"{nameof(ObjectWriterService)} || Failed to find type '{rootTypeName}' with provider '{rootTypeProvider ?? any}'.\nFile: {objFile}");
                return;
            }
        }

        var objWriter = new ObjectWriter(objName, objType, objFile, _nodeFactory, rootTypePath);
        _objWriters.Add(objWriter);

        if (objType.Name.StartsWith(nameof(UDataTable<byte>)))
        {
            if (objWriter.ObjectPath != null)
                dt.OnDataTableChangedByPath<UObjectBase>(objWriter.ObjectPath, table => objWriter.WriteToObject((nint)table.Self));
            else
                dt.OnDataTableChanged<UObjectBase>(objWriter.ObjectName, table => objWriter.WriteToObject((nint)table.Self));
        }
        else
        {
            if (objWriter.ObjectPath != null)
                uobjs.OnObjectLoadedByPath<UObjectBase>(objWriter.ObjectPath, obj => objWriter.WriteToObject((nint)obj.Self));
            else
                uobjs.OnObjectLoadedByName<UObjectBase>(objWriter.ObjectName, obj => objWriter.WriteToObject((nint)obj.Self));
        }
        
        Log.Information($"{nameof(ObjectWriterService)} || Object XML registered: {objWriter.GetObjectNameOrPath()}\nFile: {objFile}");
    }

    private readonly record struct TypeKey(string TypeName, string? TypeProvider);
}