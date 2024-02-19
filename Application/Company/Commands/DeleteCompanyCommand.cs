using MediatR;

namespace Application.Company.Commands;

public record DeleteCompanyCommand(Guid Id, bool TrackChanges) : IRequest;
