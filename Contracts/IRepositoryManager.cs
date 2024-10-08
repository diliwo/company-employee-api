﻿namespace Contracts;

public interface IRepositoryManager
{
    ICompanyRepository Company { get; }
    IEmployeeRepository Employee { get; }

    void save();
    Task SaveAsync();
}