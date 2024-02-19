using MediatR;

namespace Application.Company.Notifications;

public sealed record CompanyDeletedNotification(Guid Id, bool TrackChanges) : INotification;
