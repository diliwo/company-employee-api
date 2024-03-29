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

internal sealed class GetEmployeesHandler : EmployeesTools, IRequestHandler<GetEmployeesQuery, IEnumerable<EmployeeDto>>
{

    public GetEmployeesHandler(IRepositoryManager repository, IMapper mapper) : base(repository, mapper) {}

    public async Task<IEnumerable<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        // We check if the age is valide
        if (!request.linkParameters.EmployeeParameters.ValidAgeRange)
        {
            throw new MaxAgeRangeBadRequestException();
        }

        // We check if company exist in db
        await CheckIfCompanyExists(request.companyId, request.trackChanges);

        var employeesWithMetaDatab = 
            await _repository.Employee.GetEmployeesAsync(request.companyId, request.linkParameters.EmployeeParameters, request.trackChanges);

        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaDatab);

        return employeesDto;
    }
}