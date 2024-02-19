using Application.Company.Queries;
using Application.Employee.Queries;
using Entities.LinkModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesV2Controller : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IPublisher _publisher;

        public EmployeesV2Controller(ISender sender, IPublisher publisher)
        {
            _sender = sender;
            _publisher = publisher;
        }

        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId, [FromQuery] EmployeeParameters employeeParameters)
        {
            var linkParams = new LinkParameters(employeeParameters, HttpContext);

            var companies = await _sender.Send(new GetEmployeesQuery(companyId, linkParams, trackChanges: false));

            return Ok(companies);
        }
    }
}
