using Microsoft.AspNetCore.Http;
using Shared.RequestFeatures;

namespace Entities.LinkModels;
// Is used to transfered required parameters from our controller to the service layer and avoid the installation
// of an additional
public record LinkParameters(EmployeeParameters EmployeeParameters, HttpContext Context);