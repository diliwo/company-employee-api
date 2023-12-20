using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace CompanyEmployees.Presentation.Controllers;

[ApiVersion("2.0", Deprecated = true)]
[Route("api/companies")]
[ApiController]
public class CompaniesV2Controller : ControllerBase
{
    private readonly IServiceManager _services;

    public CompaniesV2Controller(IServiceManager services) => _services = services;

    [HttpGet]
    public async Task<IActionResult> getCompanies()
    {
        var companies = await _services.CompanyService.GetAllCompaniesAsync(trackChanges: false);

        var companiesV2 = companies.Select(x => $"{x.Name} v2");

        return Ok(companiesV2);
    }
}