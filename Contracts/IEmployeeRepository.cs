using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts;

public interface IEmployeeRepository
{
    IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges);
    Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges);
    Employee GetEmployee(Guid companyId, Guid employeeId, bool trackChanges);
    Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges);
    void CreateEmployeeForCompany(Guid companyId, Employee employee); //Not async because of changing the state of the entity to Added
    void DeleteEmployee(Employee employee); // Not async because of Changing the state of the entity to Deleted
}