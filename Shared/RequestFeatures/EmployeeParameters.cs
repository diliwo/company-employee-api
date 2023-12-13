namespace Shared.RequestFeatures;

public class EmployeeParameters : RequestParameters
{
    public EmployeeParameters() => OrderBy = "name";
    public uint MinAge { get; set; } // usigned int is used to avoid negative year values
    public uint MaxAge { get; set; } = int.MaxValue; // usigned int is used to avoid negative year values

    public bool ValidAgeRange => MaxAge > MinAge;

    public string? SearchTerm { get; set; } 
}