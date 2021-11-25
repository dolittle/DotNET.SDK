// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Samples.DependencyInjection.Shared;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    readonly ITenantSpecific _tenantSpecificService;

    public TestController(ITenantSpecific tenantSpecificService)
    {
        _tenantSpecificService = tenantSpecificService;
        tenantSpecificService.SayHello();
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        _tenantSpecificService.SayHello();
        return Accepted();
    }
}