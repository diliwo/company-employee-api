﻿using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public abstract record CompanyForManipulationDto
{
    [Required(ErrorMessage = "Company name is a required field.")]
    [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
    public string? Name { get; init; }

    [Required(ErrorMessage = "Position is a required field.")]
    [MaxLength(100, ErrorMessage = "Maximum length for the Street is 100 characters.")]
    public string? Address { get; init; }

    [Required(ErrorMessage = "Position is a required field.")]
    [MaxLength(30, ErrorMessage = "Maximum length for the Street is 30 characters.")]
    public string? Country { get; init; }
    public IEnumerable<EmployeeForCreationDto> Employees { get; init; }
}