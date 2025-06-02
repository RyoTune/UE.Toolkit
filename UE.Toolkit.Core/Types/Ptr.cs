namespace UE.Toolkit.Core.Types;

public readonly unsafe struct Ptr<T>(T* value) : IEquatable<Ptr<T>>
    where T : unmanaged
{
    public readonly T* Value = value;

    public bool Equals(Ptr<T> other) => Value == other.Value;
    public static bool operator ==(Ptr<T> a, Ptr<T> b) => a.Equals(b);
    public static bool operator !=(Ptr<T> a, Ptr<T> b) => !a.Equals(b);
}