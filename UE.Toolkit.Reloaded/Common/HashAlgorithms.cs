using HashifyNet;
using HashifyNet.Algorithms.CityHash;

namespace UE.Toolkit.Reloaded.Common;

public static class HashAlgorithms
{
    public static ICityHash CityHash = HashFactory<ICityHash>.Create();
}