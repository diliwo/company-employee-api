using Entities.Models;

namespace Contracts;

public interface IEmployeeRepository
{
    IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges);
    Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges);
    Employee GetEmployee(Guid companyId, Guid employeeId, bool trackChanges);
    Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges);
    void CreateEmployeeForCompany(Guid companyId, Employee employee); //Not async because of changing the state of the entity to Added
    void DeleteEmployee(Employee employee); // Not async because of Changing the state of the entity to Deleted
}