# NServiceBus.AspNetCore

[![Build Status](https://dev.azure.com/NathanArnott/GitHub/_apis/build/status/NServiceBus.AspNetCore?branchName=master)](https://dev.azure.com/NathanArnott/GitHub/_build/latest?definitionId=1&branchName=master)
[![NuGet package](https://img.shields.io/nuget/v/NServiceBus.AspNetCore.svg)](https://nuget.org/packages/NServiceBus.AspNetCore)
[![NuGet downloads](https://img.shields.io/nuget/dt/NServiceBus.AspNetCore.svg)](https://nuget.org/packages/NServiceBus.AspNetCore)

## Overview

This package provides a common framework for adding NServiceBus to AspNetCore applications. It adds various services to IServiceCollection to make accessing NServiceBus inside your AspNetCore controllers really easy. It also automatically wires up all your services into NServiceBus as well so you can inject them into NSB Message Handlers.

## Usage Example for Startup.cs

```CSharp
public void ConfigureServices(IServiceCollection services)
{
    //...Add services
    
    services.AddScoped<SomeDataStore>();

    //add NSB Endpoint
    services.AddNServiceBusEndpoint(EndpointName, endpointConfiguration =>
    {
        var transport = endpointConfiguration.UseTransport<SomeNsbTransport>();
        
        //...continue configuring endpoint
    });
    
    //...Add services
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
     //...Call various Use methods.
     
     //start NSB endpoint.
     app.UseNServiceBusEndpoints();
}

```

## Key Benefits

### IMessageSession

IMessageSession can be injected into controller or other AspNetCore pipeline services. It can then be used to Send or Publish messages.

Example:
```CSharp
public class TestController : ControllerBase
{
    private readonly IMessageSession _nsb;

    public TestController(IMessageSession nsb) 
    { 
        _nsb = nsb;
    }

    [HttpPost]
    public async Task SendCommand()
    {
        await _nsb.Send(new TestCommand());
    }
}
```

### IIncomingNsbMessageContextAccessor

IIncomingNsbMessageContextAccessor can be injected into services used by the NSericeBus pipeline to Send, Publish and Reply to messages without having to pass IMessageHandlerContext from the handler around.

Example:
```CSharp
public class SomeDataStore
{
    private readonly IMessageProcessingContext _context;

    public SomeDataStore(IIncomingNsbMessageContextAccessor contextAccessor) 
    { 
        _context = contextAccessor.Context;
    }

    public async Task DoSomethingAsync()
    {
        //...

        await _context.Publish(new NotifyOfSomethingEvent());
    }
}
```

### INsbContext

INsbContext can be injected into services used by the AspNetCore **and** NSericeBus pipelines to Send, Publish and Reply to messages without having to pass IMessageHandlerContext or IMessageSession around. Using INsbContext.Send and INsbContext.Publish will use an existing message context, if it exists. In other words, INsbContext wraps IMessageSession and IIncomingNsbMessageContextAccessor into one service, so you don't have to worry about which one to use in which context.

In the context of a AspNetCore request, a call to INsbContext.Reply will fail with an InvalidOperationException. You can check for this using the property INsbContext.IsInNsbPipeline.

Example:
```CSharp
public class SomeDataStore
{
    private readonly INsbContext _context;

    public SomeDataStore(INsbContext context) 
    { 
        _context = context;
    }

    public async Task DoSomethingAsync()
    {
        //...

        await _context.Publish(new NotifyOfSomethingEvent());
    }
}
```
