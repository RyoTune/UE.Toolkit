using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UE.Toolkit.Core.Common;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Interfaces;

/// <summary>
/// DataTables service.
/// </summary>
public interface IDataTables
{
    /// <summary>
    /// Notify whenever a <see cref="UDataTable{TRow}"/> is changed, typically just when loaded, and receive it in simple wrapper.
    /// </summary>
    /// <param name="name"><see cref="UDataTable{TRow}"/> object name.</param>
    /// <param name="callback">Callback function receiving the <see cref="UDataTable{TRow}"/>.</param>
    /// <typeparam name="TRow"><see cref="UDataTable{TRow}"/> row struct type. Use <see cref="UObjectBase"/> to handle typing yourself.</typeparam>
    void OnDataTableChanged<TRow>(string name, Action<ToolkitDataTable<TRow>> callback)
        where TRow : unmanaged;
    
    /// <summary>
    /// Notify whenever a <see cref="UDataTable{TRow}"/> is changed, typically just when loaded, and receive it in simple wrapper.
    /// </summary>
    /// <param name="name"><see cref="UDataTable{TRow}"/> object path.</param>
    /// <param name="callback">Callback function receiving the <see cref="UDataTable{TRow}"/>.</param>
    /// <typeparam name="TRow"><see cref="UDataTable{TRow}"/> row struct type. Use <see cref="UObjectBase"/> to handle typing yourself.</typeparam>
    void OnDataTableChangedByPath<TRow>(string objectPath, Action<ToolkitDataTable<TRow>> callback)
        where TRow : unmanaged;
}

/// <summary>
/// Simple <see cref="UDataTable{TRow}"/> dictionary wrapper. Read-only and uses linear search.<br/>
/// For more performant or advanced use-cases, see <see cref="UDataTableManaged{TRow}"/>.
/// </summary>
/// <typeparam name="TRow">Row type.</typeparam>
public unsafe class ToolkitDataTable<TRow>(UDataTable<TRow>* self)
    : ToolkitUObject<UDataTable<TRow>>(self),IReadOnlyDictionary<string, ToolkitDataTableRow<TRow>>
    where TRow : unmanaged
{
    /// <summary>
    /// Data table struct name.
    /// </summary>
    public string RowStructName => ToolkitUtils.GetPrivateName((nint)Self->RowStruct);
    
    #region IReadOnlyDictionary

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, ToolkitDataTableRow<TRow>>> GetEnumerator()
        => new ToolkitDataTableEnumerator<TRow>(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int Count => Self->RowMap.MapNum;

    /// <inheritdoc />
    public bool ContainsKey(string key) => Keys.Any(x => x == key);

    /// <inheritdoc />
    public bool TryGetValue(string key, [NotNullWhen(true)] out ToolkitDataTableRow<TRow>? value)
    {
        value = Values.FirstOrDefault(x => x.Name == key);
        return value != null;
    }

    /// <inheritdoc />
    public ToolkitDataTableRow<TRow> this[string key] => Values.First(x => x.Name == key);

    /// <inheritdoc />
    public IEnumerable<string> Keys => this.Select(x => x.Key);

    /// <inheritdoc />
    public IEnumerable<ToolkitDataTableRow<TRow>> Values => this.Select(x => x.Value);

    #endregion
}

/// <summary>
/// Simple <see cref="UDataTable{TRow}"/> row wrapper containing the name and value pointer.
/// </summary>
/// <param name="name">Row name.</param>
/// <param name="value">Row value pointer.</param>
/// <typeparam name="TRow">Row type.</typeparam>
public unsafe class ToolkitDataTableRow<TRow>(string name, TRow* value)
    where TRow : unmanaged
{
    /// <summary>
    /// Row value pointer.
    /// </summary>
    public TRow* Value { get; } = value;

    /// <summary>
    /// Row name.
    /// </summary>
    public string Name { get; } = name;
}

/// <summary>
/// <see cref="ToolkitDataTable{TRow}"/> enumerator.
/// </summary>
/// <param name="table">DataTable instance.</param>
/// <typeparam name="TRow">Row type.</typeparam>
internal unsafe class ToolkitDataTableEnumerator<TRow>(ToolkitDataTable<TRow> table) : IEnumerator<KeyValuePair<string, ToolkitDataTableRow<TRow>>>
    where TRow : unmanaged
{
    private int _position = -1;

    /// <inheritdoc />
    public bool MoveNext() => ++_position < table.Count;

    /// <inheritdoc />
    public void Reset() => _position = -1;

    /// <inheritdoc />
    public KeyValuePair<string, ToolkitDataTableRow<TRow>> Current
    {
        get
        {
            var row = &table.Self->RowMap.Elements[_position];
            var name = row->Key.ToString();
            return new(name, new(name, row->Value));
        }
    }

    object IEnumerator.Current => Current;

    /// <inheritdoc />
    public void Dispose() { }
}