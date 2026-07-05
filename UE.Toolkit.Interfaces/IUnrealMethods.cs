using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Interfaces;

/// <summary>
/// API for invoking Blueprint methods on an object.
/// </summary>
public interface IUnrealMethods
{

    /// <summary>
    /// Create a Int8 parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateI8Param(sbyte Value);
    
    /// <summary>
    /// Create a Int16 parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateI16Param(short Value);
    
    /// <summary>
    /// Create a Int32 parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateI32Param(int Value);
    
    /// <summary>
    /// Create a Int64 parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateI64Param(long Value);
   
    /// <summary>
    /// Create a UInt8 parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateU8Param(byte Value);
    
    /// <summary>
    /// Create a UInt16 parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateU16Param(ushort Value);
    
    /// <summary>
    /// Create a UInt32 parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateU32Param(uint Value);
    
    /// <summary>
    /// Create a UInt64 parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateU64Param(ulong Value);
    
    /// <summary>
    /// Create a float parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateF32Param(float Value);
    
    /// <summary>
    /// Create a double parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateF64Param(double Value);
    
    /// <summary>
    /// Create a FName parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateNameParam(string Value);
    
    /// <summary>
    /// Create a FString parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateStringParam(string Value);
    
    /// <summary>
    /// Create a bool parameter, which can be passed into ProcessEvent to call a blueprint exposable function
    /// with the specified value.
    /// </summary>
    /// <param name="Value">Target value.</param>
    /// <returns>An invocation parameter containing the value.</returns>
    public IInvocationParameter CreateBoolParam(bool Value);
    
    /// <summary>
    /// Invoke a blueprint exposable function of the given name on the target object. Assumes that the function has
    /// no return value.
    /// </summary>
    /// <param name="Object">Object to invoke function on.</param>
    /// <param name="Name">Name of the function to invoke.</param>
    /// <param name="Parameters">The list of parameters used by the function.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public void ProcessEvent<TObject>(ToolkitUObject<TObject> Object, string Name,
        ref List<IInvocationParameter> Parameters) where TObject : unmanaged;
    
    /// <summary>
    /// Invoke a blueprint exposable function of the given name on the target object. Assumes that the function has
    /// a return value.
    /// </summary>
    /// <param name="Object">Object to invoke function on.</param>
    /// <param name="Name">Name of the function to invoke.</param>
    /// <param name="Parameters">The list of parameters used by the function.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <typeparam name="TReturnType">Return type.</typeparam>
    public TReturnType ProcessEvent<TObject, TReturnType>(ToolkitUObject<TObject> Object, string Name,
        ref List<IInvocationParameter> Parameters) 
        where TObject : unmanaged
        where TReturnType: unmanaged;
}