using Entities.LinkModels;
using MediatR;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Employee.Queries;
public sealed record GetEmployeeQuery(Guid companyId, Guid id, bool trackChanges) : IRequest<EmployeeDto>;
