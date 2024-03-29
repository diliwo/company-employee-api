using Application.Company.Queries;
using Application.Employee.Commons;
using Application.Employee.Queries;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using MediatR;
using Shared.DataTransferObjects;

namespace Application.Employee.Handlers;

internal sealed class GetEmployeeHandler : EmployeesTools, IRequestHandler<GetEmployeeQuery, EmployeeDto>
{
    public GetEmployeeHandler(IRepositoryManager repository, IMapper mapper):base(repository, mapper) {}

    public async Task<EmployeeDto> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
    {
        await CheckIfCompanyExists(request.companyId, request.trackChanges);

        var employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(request.companyId, request.id, request.trackChanges);

        var employee = _mapper.Map<EmployeeDto>(employeeDb);

        return employee;
    }
}