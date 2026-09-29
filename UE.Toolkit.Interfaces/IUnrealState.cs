using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Interfaces;

/// <summary>
/// API for functions related to the game/engine state.
/// </summary>
public interface IUnrealState
{
    /// <summary>
    /// Try to get the currently active world. This is useful in cases where there is no available
    /// World Context Object, which is usually the recommended approach.
    /// </summary>
    /// <param name="TargetWorld">The target world.</param>
    /// <returns>If a target world could be found.</returns>
    public bool GetCurrentPlayWorld(out IUObject? TargetWorld);

    /// <summary>
    /// Retrieve a subsystem from the current game instance.
    /// </summary>
    /// <param name="GameInstance">Current Game Instance.</param>
    /// <param name="Subsystem">The subsystem if it was successfully retrieved.</param>
    /// <typeparam name="TSubsystem">The subsystem type</typeparam>
    /// <returns>Whether a subsystem of the type TSubsystem could be retrieved.</returns>
    public bool GetSubsystem<TSubsystem>(IUGameInstance? GameInstance, out IUObject? Subsystem) 
        where TSubsystem : unmanaged;

    /// <summary>
    /// Retrieve a subsystem from the current game instance.
    /// </summary>
    /// <param name="GameInstance">Current Game Instance.</param>
    /// <param name="SubsystemType">The type of the subsystem.</param>
    /// <param name="SubsystemObj">The subsystem if it was successfully retrieved.</param>
    /// <returns>Whether a subsystem of the type SubsystemType could be retrieved.</returns>
    public bool GetSubsystem(IUGameInstance? GameInstance, IUClass? SubsystemType, out IUObject? SubsystemObj);
}