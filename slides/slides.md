---
theme: default
background: https://images.unsplash.com/photo-1583095117095-adaeabc401ab?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1744&q=80
# https://unsplash.com/photos/T_fYMj6H-pE
class: text-center
highlighter: shiki
lineNumbers: false
info: |
  ## High Performance Logging in .NET

  This is a talk about how to do high performance logging in .NET.
drawings:
  persist: false
transition: slide-left
title: High Performance Logging in .NET
---

# High Performance Logging in .NET

<div class="pt-12">
  <span @click="$slidev.nav.next" class="px-2 py-1 rounded cursor-pointer" hover="bg-white bg-opacity-10">
    Press Space for next page <carbon:arrow-right class="inline"/>
  </span>
</div>

<div class="abs-br m-6 flex gap-2">
  <button @click="$slidev.nav.openInEditor()" title="Open in Editor" class="text-xl slidev-icon-btn opacity-50 !border-none !hover:text-white">
    <carbon:edit />
  </button>
  <a href="https://github.com/slidevjs/slidev" target="_blank" alt="GitHub"
    class="text-xl slidev-icon-btn opacity-50 !border-none !hover:text-white">
    <carbon-logo-github />
  </a>
</div>

<style>
* {
  text-shadow: 2px 2px 4px #000000;
}
</style>

---
layout: image-right
image: https://www.finxit.at/img/profile_full.jpg
---

# About me

The world's okayest developer

- <carbon-location class="text-blue" /> Linz, Austria
- 👨‍💻 &nbsp;Freelancer
- <logos-dotnet /> Developer
- <logos-mastodon-icon /> [@cschabetsberger@mstdn.social](https://mstdn.social/@cschabetsberger)

<v-click>

  <span class="absolute b-l-3 b-red top-33 left-19.2 h-31 hidden-print">
  </span>

  <span class="absolute b-l-3 b-red top-33 left-25.5 h-31 hidden-print">
  </span>

</v-click>

<style>
h1 {
  background-color: #2B90B6;
  background-image: linear-gradient(45deg, #4EC5D4 10%, #146b8c 20%);
  background-size: 100%;
  -webkit-background-clip: text;
  -moz-background-clip: text;
  -webkit-text-fill-color: transparent;
  -moz-text-fill-color: transparent;
}

@media print {
  .hidden-print {
    display: none !important;
  }
}
</style>

---
layout: center
---

# Who of you have written a line like this?

```csharp
_logger.LogInformation($"Retailed vehicle with id {vehicleId}.");
```

---

# String Interpolation is awesome!

Except when it isn't

<v-click>

Let's check out, what the problem with this is:

```csharp
_logger.LogInformation($"Retailed vehicle with id {vehicleId}.");
```

</v-click>

<v-clicks>

- A new string is allocated each and every time
- Strings 🧵 go onto the heap
- Garbage Collection Time 🗑️🛻⏰

</v-clicks>

---
layout: fact
---

# Garbage Collection is expensive!

<!--
Garbage Collector suspends all threads to collect all the unused reference variables.
-->

---

# `string.Format` style instead?

```csharp
_logger.LogInformation("Retailed vehicle with id {vehicleId}.", vehicleId);
```

<v-clicks>

✅ Enables structured logging

### Does this make you happy?

</v-clicks>

---

# Let's check the method definition

```csharp
public static void LogInformation (
  this Microsoft.Extensions.Logging.ILogger logger,
  string? message,
  params object?[] args);
```

<Arrow v-click x1="190" y1="90" x2="155" y2="160" class="c-red" />

<v-clicks>

- Value types get boxed
- Boxed types go onto the heap
- Garbage Collection Time 🗑️🛻⏰

</v-clicks>

<!--
Value types are the most logged parameters.

IDs (int, long, Guid)

DateTimes

Enums

And well sometimes strings
-->

---

# There would even be a warning

[CA1848: Use the LoggerMessage delegates](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1848)

<code>For improved performance, use the LoggerMessage delegates instead of calling 'LoggerExtensions.LogInformation(ILogger, string?, params object?[])'</code>

❌ __NOT__ activated by default.

---

# Time for some high performance stuff

```csharp {all|2|3-5|1|7-10|all}
private static readonly Action<ILogger, Guid, Exception?> _retailedVehicle =
    LoggerMessage.Define<Guid>(
        LogLevel.Information,
        new EventId(364),
        "Retailed vehicle with id {vehicleId}.");

public static void LogRetailedVehicle(
    this ILogger logger,
    Guid vehicleId)
    => _retailedVehicle(logger, vehicleId, default);
```

<br />

<v-clicks>

### Does this make you happy?

That's an awful lot of boiler plate code for logging one line...

</v-clicks>

<!--
Also in my opinion it is very hard to ready, there is no clear flow.
-->

---

# Better: use source code generation

```csharp
namespace Microsoft.Extensions.Logging;

public static partial class LoggerExtensions
{
    [LoggerMessage(
      EventId = 364,
      Level = LogLevel.Information,
      Message = "Retailed vehicle with id {vehicleId}.")]
    public static partial void LogRetailedVehicle(this ILogger logger, Guid vehicleId);
}
```

<!--
partial class and method definition.

Better reading flow.

A lot less boilerplate code.
-->

---

# Let's check the generated code

```csharp
namespace Microsoft.Extensions.Logging
{
    partial class LoggerExtensions
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "7.0.8.32018")]
        private static readonly global::System.Action<global::Microsoft.Extensions.Logging.ILogger, global::System.Guid, global::System.Exception?> __LogRetailedVehicleCallback =
            global::Microsoft.Extensions.Logging.LoggerMessage.Define<global::System.Guid>(global::Microsoft.Extensions.Logging.LogLevel.Information, new global::Microsoft.Extensions.Logging.EventId(364, nameof(LogRetailedVehicle)), "Retailed vehicle with id {vehicleId}.", new global::Microsoft.Extensions.Logging.LogDefineOptions() { SkipEnabledCheck = true }); 

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Extensions.Logging.Generators", "7.0.8.32018")]
        public static partial void LogRetailedVehicle(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.Guid vehicleId)
        {
            if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Information))
            {
                __LogRetailedVehicleCallback(logger, vehicleId, null);
            }
        }
    }
}
```

---
layout: fact
---

# Benchmark

---

# Pitfalls

[There are some official constraints](https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator#log-method-constraints)

<v-click>

However, something that is not in the documentation:

❌ `LoggerMessageAttribute` doesn't work with the destruct formatter:

```csharp
[LoggerMessage(
  EventId = 999,
  Level = LogLevel.Error,
  Message = "Unhandled exception for request {@Request}")]
public static partial void LogUnhandledException(this ILogger logger, Exception exception, Request request);
```

<Arrow x1="440" y1="200" x2="400" y2="270" class="c-red" />

✅ Needs to be defined with `LoggerMessage.Define` instead.

</v-click>

---

# Scopes

❌ No source code generator attribute for scopes:

```csharp
[LoggerScope(Message = "Correlation Id: {CorrelationId}")]
public static partial IDisposeable? BeginCorrelationId(this ILogger logger, Guid correlationId);
```

✅ Needs to be defined with `LoggerMessage.DefineScope` instead:

```csharp
public static class LoggerExtensions
{
    private static readonly Func<ILogger, Guid, IDisposable?> CorrelationIdScope =
        LoggerMessage.DefineScope<Guid>("Correlation Id: {CorrelationId}");

    public static IDisposable? BeginCorrelationIdScope(this ILogger logger, Guid correlationId)
        => CorrelationIdScope(logger, correlationId);
}
```

<style>
.slidev-layout h1 + p {
  opacity: 1;
}
</style>

---
layout: fact
---

# Structured Logging Demo

---
layout: fact
---

# Woudldn't it be cool, if...

---

# we could do string interpolation with structured logging?

<v-clicks>

- ✅ Technology wise - yes
- ❌ Performance wise - hell no

</v-clicks>

<p v-click class="c-red fw-700">❌ So don't do this!</p>
<P v-click>But I'm gonna show it anyway.</P>

---

# String Interpolation since C#10 / .NET 6

- In .NET 6, the behaviour was changed
  - from string concatenation
  - to use a `StringBuilder`

<v-click>

- Implemented via `DefaultInterpolatedStringHandler`
- and the `InterpolatedStringHandler` attribute

</v-click>

<v-click>

And we can replace it 🎉

</v-click>

---

# So let's build our own `InterpolatedStringHandler`


```csharp
[InterpolatedStringHandler]
public ref struct StructuredLoggingInterpolatedStringHandler
{
    private DefaultInterpolatedStringHandler _innerHandler;
    private readonly List<object?> _arguments = new();

    ///...

    public void AppendFormatted<T>(
        T message,
        [CallerArgumentExpression(nameof(message))] string callerName = "")
    )
    {
        _innerHandler.AppendFormatted("{" + callerName + "}");
        _arguments.Add(message);
    }
}
```

And then we ➡️

---

# And a new extension method

```csharp
public static void Information(
    this ILogger logger,
    ref StructuredLoggingInterpolatedStringHandler messageTemplate))
{
    logger.Information(message.ToString(), message.Parameters);
}
```

And then we ➡️

---
layout: center
---

# Wait<span v-click>, what the hell are we even doing?</span>

<v-click>

- `List`s without a capacity?
- `objects` again?
- Value type boxing again?

</v-click>

---

# We could manually optimize all of that

- Making sure `ILogger.Log` isn't called (by checking the log level before)
- Allocations of `object[]` representing the parameters are avoided
- Value type boxing is avoided

<v-click>

Or we use a more sophisticated solution...

</v-click>

---

# ISLE

https://github.com/fedarovich/isle

- Interpolated String Logging Extensions
- It allows structured logging with string interpolation
- Add package reference and initialization configuration
```xml
<PackageReference Include="Isle.Extensions.Logging" Version="1.5.10" />
```
```csharp
IsleConfiguration.Configure(builder => {});
```

- It automatically replaces the existing extension methods
```csharp
_logger.LogInformation($"Retailed vehicle with id {vehicleId}.");
```

---
layout: fact
---

# Benchmark

---

# Conclusion ISLE

- ⁉️ Can be used wrong (inline code in string interpolation)
- ✅ Not logged messages are surprisingly fast
- ❌ But logged messages are even worse than normal string interpolation

<br />

<v-clicks>

### __Maybe, don't get to smart with your tooling__

### __And stay performant instead__

</v-clicks>

---

# Closing

- ⏩ Use .NET High Performance Logging
- ✅ Use the `LoggerMessage` attribute
- 🆓 It's free optimization

---
layout: center
class: text-center
---

# Learn More

[High-performance logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging) · [Compile-time logging source generation](https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator) · [Improving logging performance with source generators ](https://andrewlock.net/exploring-dotnet-6-part-8-improving-logging-performance-with-source-generators/) · [Tutorial: Write a custom string interpolation handler](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/interpolated-string-handler)
