namespace Service.Contracts;

/**
 * Deprecatated because of the CQRS Approach
 */
public interface IServiceManager
{
    ICompanyService CompanyService { get; }
    IEmployeeService EmployeeService { get; }
}