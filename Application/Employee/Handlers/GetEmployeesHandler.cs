using Application.Company.Queries;
using Application.Employee.Queries;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using MediatR;
using Shared.DataTransferObjects;

namespace Application.Employee.Handlers;

internal sealed class GetEmployeesHandler : IRequestHandler<GetEmployeesQuery, IEnumerable<EmployeeDto>>
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public GetEmployeesHandler(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

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

    private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }
    }
}