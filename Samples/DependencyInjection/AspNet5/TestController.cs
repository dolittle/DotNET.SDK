// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Samples.DependencyInjection.Shared;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    readonly IScoped _scopedService;
    readonly ITenantSpecific _tenantSpecificService;
    readonly ITenantSpecificScoped _tenantSpecificScopedService;

    public TestController(IScoped scopedService, ITenantSpecific tenantSpecificService, ITenantSpecificScoped tenantSpecificScopedService)
    {
        _scopedService = scopedService;
        _tenantSpecificService = tenantSpecificService;
        _tenantSpecificScopedService = tenantSpecificScopedService;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        _scopedService.SayHello();
        _tenantSpecificService.SayHello();
        _tenantSpecificScopedService.SayHello();
        return Accepted();
    }
}