using MediatR;
using Shared.DataTransferObjects;

namespace Application.Company.Queries;

public sealed record GetCompaniesQuery(bool TrackChanges) : IRequest<IEnumerable<CompanyDto>>;
