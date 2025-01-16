using System;
using R3;

namespace ProcessedValue.R3
{
    #region  Shortnames
    [Serializable] public class RProcessed<TSort, TValue> : ReactiveProcessed<TSort, TValue>
    where TSort : IComparable<TSort>
    {
        public RProcessed() {} 
        public RProcessed(TValue initialValue) : base(initialValue)
    }
    
    [Serializable] public class RProcessed<TValue> : ReactiveProcessed<TValue>
    { 
        public RProcessed() {}
        public RProcessed(TValue initialValue) : base(initialValue) {} 
    }

    [Serializable] public class ReactiveProcessed<TValue> : ReactiveProcessed<int, TValue> 
    {
        public ReactiveProcessed() {}
        public ReactiveProcessed(TValue initialValue) : base(initialValue) {}
    }
    #endregion

    [Serializable] public class ReactiveProcessed<TSort, TValue> : DisposableProcessed<TSort, TValue>
    where TSort : IComparable<TSort>
    {
        public new bool IsDirty
        {
            get => base.IsDirty;
            set => ProcessValue();
        }
        private ReactiveProperty<TValue> _reactiveValue = new ReactiveProperty<TValue>();
        public new ReactiveProperty<TValue> Value
        {
            get => _reactiveValue;
        }

        public ReactiveProcessed() {} 

        public ReactiveProcessed(TValue initialValue) : base(initialValue) =>
            _reactiveValue = new ReactiveProperty<TValue>(initialValue);

        public override void ProcessValue()
        {
            base.ProcessValue();
            _reactiveValue.Value = _value;
        }
    }
}