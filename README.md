# YetAnotherStatsDClient

Yet Another StatsD .NET Client

## What is this?

As described, this is Yet Another StatsD .NET Client. 

Why? At [Audiu](https://audiu.net) and [RepostExchange](https://repostexchange.com), our infrastructure is spread far and wide. 
This gives us an unfortunate situation where we end up with UDP packets with statistics being sent over the internet between datacenters.

There are a few options on the table, such as using relays within the DCs to pass the statistics on, but for us, I chucked together a version which uses a TCP connection on its own thread, and stats are buffered in there, and sent as and when they can.

If there is a disruption, when it reconnects it will drop all the previous statistics on the floor.

It's designed to be used within DI containers (no statics!)

## How do we use it?

You can get it from [NuGet](https://www.nuget.org/packages/Audiu.YetAnotherStatsDClient). Just install using your favourite package manager (shout out to the brilliant [Paket](https://fsprojects.github.io/Paket/) package manager!)

This is a quick example of our setup using the wonderful [SimpleInjector](https://simpleinjector.org/index.html) library 

```csharp
var statsdHostName = ConfigurationManager.AppSettings["statsd.hostname"];
var statsdPort = int.Parse(ConfigurationManager.AppSettings["statsd.port"]);
var statsdPrefix = ConfigurationManager.AppSettings["statsd.prefix"];

var statsClient = new StatsClient(
    new StatsConfig
    {
        Host = statsdHostName,
        Port = statsdPort,
        Prefix = statsdPrefix
    });

var statsNoOpClient = new StatsNoOpClient();

container.Register<IStatsClient>(
    () =>
    {
        // We use feature flags, but feel free just to register statsClient as a singleton
        if (iShouldBeUsingStatsD) 
        {
            return statsClient;
        }

        return statsNoOpClient;

    }, Lifestyle.Scoped);
```

Inject `IStatsClient` wherever you might need it

Then invoke something like:

```csharp
_statsClient.Count("mycounter.count");
_statsClient.Count("mycounter.count", 10);
_statsClient.Gauge("mygauge.value", 10.25);

var timer = Stopwatch.StartNew();
// Do something
timer.Stop();
_statsClient.Timing("somefunction.timing", timer.ElapsedMilliseconds);
```

Using disposible timers is also super useful:

```csharp
using (_statsClient.StartTimer("somefunction.timing")) 
{
    myFunc();
}
```

And functional styles in sync and async style:

```csharp
_statsClient.Time("somefunction.timing", t => myFunc());

var myValue = _statsClient.Time("somefunction.timing", t => myFunc());

await _statsClient.Time("asyncfunction.timing", async t => await myAsyncFunc());

var myValue = await _statsClient.Time("asyncfunction.timing", async t => await myAsyncFunc());
```

## Credits

I have taken bits of code from the following great OS libs: [statsd-csharp-client](https://github.com/Pereingo/statsd-csharp-client), [JustEat.StatsD](https://github.com/justeat/JustEat.StatsD) and [StatsN](https://github.com/TryStatsN/StatsN)

