using System.Collections;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.UE5_4_4;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UEngine = UE.Toolkit.Core.Types.Unreal.UE5_5_4.UEngine;

namespace UE.Toolkit.Core.Types.Unreal.Factories.UE5_5_4;

public class UnrealFactory : UE.Toolkit.Core.Types.Unreal.Factories.UE5_4_4.UnrealFactory
{
    
    public override IUEngine CreateUEngine(nint ptr) => new UEngine_UE5_5_4(ptr, this, Memory);
    
    public unsafe class FWorldContextEnumerator(UEngine_UE5_5_4 owner, IUnrealFactory factory) 
        : IEnumerator<IFWorldContext>, IEnumerable<IFWorldContext>
    {
        private int CurrentIndex = -1;
    
        #region impl IEnumerator 
    
        public bool MoveNext() => ++CurrentIndex < owner.GetWorldListInner()->ArrayNum;

        public void Reset() => CurrentIndex = -1;
        object? IEnumerator.Current => Current;

        public IFWorldContext Current => factory.CreateFWorldContext((nint)owner.GetWorldListInner()->AllocatorInstance[CurrentIndex].Value);


        public void Dispose() {}
    
        #endregion
    
        #region impl IEnumerable
    
        public IEnumerator<IFWorldContext> GetEnumerator() => this;
    
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
        #endregion
    }

    public unsafe class UEngine_UE5_5_4(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory) 
        : UObject_UE5_4_4(ptr, factory, memory), IUEngine
    {
        private readonly UEngine* _self = (UEngine*)ptr;

        internal TArray<Ptr<FWorldContext>>* GetWorldListInner() => &_self->WorldList;
    
        public IEnumerable<IFWorldContext> GetWorldList() => new FWorldContextEnumerator(this, factory);
    }
}