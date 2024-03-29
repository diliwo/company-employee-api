using AutoMapper;
using Contracts;
using Entities.Exceptions;

namespace Application.Employee.Commons;

public abstract class EmployeesTools
{
    protected readonly IRepositoryManager _repository;
    protected readonly IMapper _mapper;

    public EmployeesTools(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Entities.Models.Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
    {

        var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
        if (employeeEntity is null)
        {
            throw new EmployeeNotFoundException(id);
        }

        return employeeEntity;
    }

    public async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }
    }
}