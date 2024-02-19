using MediatR;
using Shared.DataTransferObjects;

namespace Application.Company.Commands;

public sealed record UpdateCompanyCommand
	(Guid Id, CompanyForUpdateDto Company, bool TrackChanges) : IRequest;
