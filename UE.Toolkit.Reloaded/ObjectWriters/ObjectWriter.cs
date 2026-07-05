using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Xml;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Reloaded.ObjectWriters.Nodes;

namespace UE.Toolkit.Reloaded.ObjectWriters;

public unsafe class ObjectWriter
{
    private static readonly EventLoopScheduler WriterScheduler = new();
    
    private readonly Type _objType;
    private readonly string _objFile;
    private readonly FieldNodeFactory _nodeFactory;
    private readonly FileSystemWatcher _xmlFileWatcher;
    private readonly Subject<Unit> _xmlChanged = new();
    private readonly IDisposable _xmlSub;

    private byte[] _xmlContent;
    private UObjectBase* _currObj;

    public ObjectWriter(string objName, Type objType, string objFile, FieldNodeFactory nodeFactory, string? objectPath)
    {
        _objType = objType;
        _objFile = objFile;
        _nodeFactory = nodeFactory;
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

        // TODO: Possibly rework XML node tree creation to return
        // a collection of generated writers to allow resetting values on rewrites.
        if (_nodeFactory.TryCreate(ObjectName, objPtr, 0, _objType, out var rootNode))
        {
            rootNode.ConsumeNode(reader);
        }
        else
        {
            Log.Error($"Failed to create root node from Object XML file.\nFile: {_objFile}");
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
