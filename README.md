# ProcessedValue.R3
ProcessedValue.R3 is the addon to ProcessedValue. ProcessedValue.R3 allows using reactive variables from R3 in delegates for Processors. Processors now store subscriptions to reactive variables. When reactive variable is updated Processor marks ProcessedValues as dirty.

The main methods of the class remain the same as in ProcessedValue.   
Check it out: https://github.com/GeroiGorodskoyZastroiki/ProcessedValue

## Usage Example
```c#
//Create DisposableProcessor and pass ReactiveProperties with constructor
ReactiveProperty<bool> reactiveProperty = new ReactiveProperty<bool>();
DisposableProcessor<bool> disposableProcessor = new DisposableProcessor<bool>((ref bool value) => value = reactiveProperty.Value, new object[] { reactiveProperty });

//Pass ReactiveProperties manually
disposableProcessor.AddSubscriptionsToReactiveProperties(new object[] { reactiveProperty });

//Pass single ReactiveProperty manually
disposableProcessor.AddSubscriptionToReactiveProperty<bool>(ref reactiveProperty);

//Create DisposableProcessed and add DisposableProcessor
DisposableProcessed<bool> disposableProcessed = new DisposableProcessed<bool>();
disposableProcessed.AddProcessor(disposableProcessor, 0);

//Add Processors along with DisposableProcessors
Processor<bool> processor = new Processor<bool>((ref bool x) => x = false);
disposableProcessed.AddProcessor(processor, 0);

//Create ReactiveProcessed and acess it's value
ReactiveProcessed<bool> reactiveProcessed = new ReactiveProcessed<bool>();
//ReactiveProcessed recalculates immediately when isDirty
Console.Writeline(reactiveProcessed.Value)

//Dispose all Processor subs on ReactiveProperties
disposableProcessor.Dispose();

//Dispose Processed - Remove Processed from list to set dirty on ReactiveProperties update for each Processor
disposableProcessed.Dispose();
```

Every class have shortnames. You can see them in the source code.