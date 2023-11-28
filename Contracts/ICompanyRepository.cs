using Entities.Models;

namespace Contracts;

public interface ICompanyRepository
{
    IEnumerable<Company> getAllCompanies(bool trackChanges);
    Task<IEnumerable<Company>> getAllCompaniesAsync(bool trackChanges);
    Company GetCompany(Guid companyId, bool trackChanges);
    Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges);
    IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
    Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
    void CreateCompany(Company company); //Not async because of changing the state of the entity to Added
    void DeleteCompany(Company company); // Not async because of Changing the state of the entity to Deleted
}