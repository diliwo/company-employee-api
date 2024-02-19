using MediatR;
using Shared.DataTransferObjects;

namespace Application.Company.Commands;

public sealed record CreateCompanyCommand(CompanyForCreationDto Company) : IRequest<CompanyDto>;
