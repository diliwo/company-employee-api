using MediatR;
using Shared.DataTransferObjects;

namespace Application.Company.Queries;

public sealed record GetCompanyQuery(Guid Id, bool TrackChanges) : IRequest<CompanyDto>;
