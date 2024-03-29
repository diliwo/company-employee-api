namespace Shared.DataTransferObjects;

public record EmployeeDto(Guid Id, string Firstname,string Lastname, int Age, string Position);