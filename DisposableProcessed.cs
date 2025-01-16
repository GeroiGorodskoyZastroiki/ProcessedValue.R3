using System;

namespace ProcessedValue.R3
{
    #region Shortnames
    [Serializable] public class DProcessed<TValue> : DisposableProcessed<TValue> 
    {
        public DProcessed() {}
        public DProcessed(TValue initialValue) : base(initialValue) {}
    }
    
    [Serializable] public class DProcessed<TSort, TValue> : DisposableProcessed<TSort, TValue>
    where TSort : IComparable<TSort>
    {
        public DProcessed() {}
        public DProcessed(TValue initialValue) : base(initialValue) {}
    }

    [Serializable] public class DisposableProcessed<TValue> : DisposableProcessed<int, TValue> 
    {
        public DisposableProcessed() {}
        public DisposableProcessed(TValue initialValue) : base(initialValue) {}
    }
    #endregion

    [Serializable] public class DisposableProcessed<TSort, TValue> : Processed<TSort, TValue>, IDisposable
    where TSort : IComparable<TSort>
    {
        public DisposableProcessed() {}

        public DisposableProcessed(TValue initialValue)
        {
            BaseValue = initialValue;
            _value = initialValue;
        }

        ~DisposableProcessed() => Dispose();

        public void Dispose()
        {
            foreach (var processorList in Processors.Values)
                foreach (var processor in processorList)
                    if (processor is DisposableProcessor<TSort, TValue> disposableProcessor)
                        disposableProcessor.Processeds.Remove(this);
        }

        public override void AddProcessor(Processor<TValue> processor, TSort priority)
        {
            if (processor is DisposableProcessor<TSort, TValue> disposableProcessor)
                if (!disposableProcessor.Processeds.Contains(this))
                    disposableProcessor.Processeds.Add(this);

            base.AddProcessor(processor, priority);
        }

        public override void RemoveProcessor(Processor<TValue> processor, TSort priority)
        {
            base.RemoveProcessor(processor, priority);

            if (processor is DisposableProcessor<TSort, TValue> disposableProcessor)
                if (!ContainsProcessor(disposableProcessor)) 
                    disposableProcessor.Processeds.Remove(this);           
        }

        public override void RemoveProcessorAtAll(Processor<TValue> processor)
        {
            base.RemoveProcessorAtAll(processor);

            if (processor is DisposableProcessor<TSort, TValue> disposableProcessor)
                disposableProcessor.Processeds.Remove(this);         
        }
    }
}