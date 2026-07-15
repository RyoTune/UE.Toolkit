using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Xml;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.ObjectWriters.Nodes;

namespace UE.Toolkit.Reloaded.ObjectWriters;

public unsafe class ObjectWriter
{
    private static readonly EventLoopScheduler WriterScheduler = new();
    
    private readonly string _objType;
    private readonly string _objFile;
    private readonly NodeFactory _factory;
    private readonly FileSystemWatcher _xmlFileWatcher;
    private readonly Subject<Unit> _xmlChanged = new();
    private readonly IDisposable _xmlSub;

    private byte[] _xmlContent;
    private UObjectBase* _currObj;

    private IUnrealClasses? _unrealClasses;

    public ObjectWriter(string objName, string objType, string objFile, NodeFactory factory, string? objectPath)
    {
        _objType = objType;
        _objFile = objFile;
        _factory = factory;
        _xmlContent = File.ReadAllBytes(objFile);
        _xmlFileWatcher = new(Path.GetDirectoryName(objFile)!, Path.GetFileName(objFile))
        {
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.LastWrite,
        };

        _xmlFileWatcher.Changed += (_, _) => _xmlChanged.OnNext(new());
        _xmlSub = _xmlChanged
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(WriterScheduler)
            .Subscribe(_ => OnXmlChanged());
        
        ObjectName = objName;
        ObjectPath = objectPath;
    }

    public string ObjectName { get; }
    
    public string? ObjectPath { get; }

    public void WriteToObject(nint objPtr)
    {
        _currObj = (UObjectBase*)objPtr;
        
        using var reader = XmlReader.Create(new MemoryStream(_xmlContent));
        reader.MoveToContent();
        
        Log.Information($"TODO: WriteToObject for {_objFile} (Type: {_objType})");
        IFieldNode? rootNode = null;
        // The root object *has* to be a class since UObjects contain the serialization methods needed to convert
        // to and from a file-based representation (UAssets are just binary files representing UObjects).
        if (_factory.Classes.GetClassInfoFromName($"U{_objType}", out var uclass))
        {
            rootNode = uclass.NamePrivate.ToString() == "DataTable"
                ? new DataTableDocument(ObjectName, objPtr, _factory)
                : new StructDocument(ObjectName, uclass, objPtr, _factory);
        }

        if (rootNode != null)
        {
            rootNode.ConsumeNode(reader);
        }
        else
        {
            Log.Error($"{nameof(ObjectWriter)} || Failed to create root node with type '{_objType}' from Object XML file.\nFile: {_objFile}");
        }
    }

    private void OnXmlChanged()
    {
        // Validate object exists and is not garbage.
        if (_currObj == null) return;
        if ((_currObj->ObjectFlags & (EObjectFlags.RF_MirroredGarbage | EObjectFlags.RF_BeginDestroyed |
                                      EObjectFlags.RF_FinishDestroyed)) != 0) return;
        
        // TODO: Reset object. *Maybe* not worth the hassle, we'll see.

        _xmlContent = File.ReadAllBytes(_objFile);
        WriteToObject((nint)_currObj);
        
        Log.Information($"{nameof(ObjectWriter)} || Object XML updated: {GetObjectNameOrPath()}\nFile: {_objFile}");
    }

    public string GetObjectNameOrPath() => ObjectPath ?? ObjectName;
}
