﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Employee
{
    [Column("EmployeeId")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Employee name is a required field.")]
    [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters")]
    public string? Firstname { get; set; }

    [Required(ErrorMessage = "Employee name is a required field.")]
    [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters")]
    public string? Lastname { get; set; }

    [Required(ErrorMessage = "Employee age is a required field.")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Employee position is a required field.")]
    [MaxLength(30, ErrorMessage = "Maximum length for the Position is 20 characters")]
    public string? Position { get; set; }

    [ForeignKey(nameof(Company))]
    public Guid CompanyId { get; set; }
    public Company? Company { get; set; }
}