using System;
using System.Linq;
using System.Collections.Generic;
using R3;

namespace ProcessedValue.R3
{
    #region Shortnames
    public class DProcessor<TValue> : DisposableProcessor<TValue>
    {
        public DProcessor() {}
        public DProcessor(ProcessorDelegate<TValue> processor, IEnumerable<object> reactiveProperties) : base(processor, reactiveProperties) {}
    }

    public class DProcessor<TSort, TValue> : DisposableProcessor<TSort, TValue> 
    where TSort : IComparable<TSort> 
    {
        public DProcessor() {}
        public DProcessor(ProcessorDelegate<TValue> processor, IEnumerable<object> reactiveProperties) : base(processor, reactiveProperties) {}
    }

    public class DisposableProcessor<TValue> : DisposableProcessor<int, TValue>
    {
        public DisposableProcessor() {}
        public DisposableProcessor(ProcessorDelegate<TValue> processor, IEnumerable<object> reactiveProperties) : base(processor, reactiveProperties) {} 
    }
    #endregion

    public class DisposableProcessor<TSort, TValue> : Processor<TValue>, IDisposable
    where TSort : IComparable<TSort>
    {
        private DisposableBag Subscriptions = new DisposableBag();
        public List<DisposableProcessed<TSort, TValue>> Processeds = new List<DisposableProcessed<TSort, TValue>>();
    
        public DisposableProcessor() {}

        public DisposableProcessor(ProcessorDelegate<TValue> processor, IEnumerable<object> reactiveProperties)
        {
            Delegate = processor;
            AddSubscriptionsToReactiveProperties(reactiveProperties);
        }

        public void Dispose()
        {
            foreach (var processed in Processeds)
                processed.RemoveProcessorAtAll(this);
            Subscriptions.Dispose();
        }

        ~DisposableProcessor() => Dispose();

        public void AddSubscriptionToReactiveProperty<TReactive>(ref ReactiveProperty<TReactive> changingValue) =>
            Subscriptions.Add(changingValue.Subscribe(value => SetDirty()));

        public void AddSubscriptionsToReactiveProperties(IEnumerable<object> reactiveProperties)
        {
            bool IsTypeReactiveProperty(Type type) =>
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ReactiveProperty<>)) || 
                (type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(ReactiveProperty<>));

            for (int i = 0; i < reactiveProperties.Count(); i++)
            {
                var item = reactiveProperties.ElementAt(i);

                if (item == null) 
                {
                    Console.Error.WriteLine($"Array element at {i} is skipped because it's null.");
                    continue;
                }

                Type itemType = item.GetType();

                if (IsTypeReactiveProperty(itemType))
                {
                    Type innerType = itemType.GetGenericArguments()[0];

                    var method = this.GetType().GetMethod("AddSubscriptionToReactiveProperty")
                                        .MakeGenericMethod(innerType);

                    method.Invoke(this, new object[] { item });
                }
                else Console.Error.WriteLine($"Array element at {i} is skipped because it's not ReactiveProperty<> or its descendant.");
            }
        }

        void SetDirty()
        {
            foreach (var processed in Processeds)
            {
                processed.IsDirty = true;
                if (processed is ReactiveProcessed<TSort, TValue> reactiveProcessed)
                    reactiveProcessed.IsDirty = true;
            }
        }
    }
}
