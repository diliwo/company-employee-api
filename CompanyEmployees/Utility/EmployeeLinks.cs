using Contracts;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Utility;

public class EmployeeLinks : IEmployeeLinks
{
    private readonly LinkGenerator _linkGenerator; // Generate a link for our response
    private readonly IDataShaper<EmployeeDto> _dataShaper; // shape our data

    public EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDto> dataShaper)
    {
        _linkGenerator = linkGenerator;
        _dataShaper = dataShaper;
    }
    
    // fields : are used to shape the previous collection.
    // companyId: route to the employee resources contain the Id from company.
    // httpContext : holds information about media types.
    public LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeesDto, string fields, Guid companyId, HttpContext HttpContext)
    {
        // We shape the collection
        var shapedEmployees = ShapeData(employeesDto, fields);

        // if httpContext contains the required mediaType
        if (ShouldGenerateLinks(HttpContext))
        {
            //  We add links to the response.
            return ReturnLinkedEmployees(employeesDto, fields,companyId, HttpContext, shapedEmployees);
        }

        // On the other hand we just return our shaped data
        return ReturnShapedEmployees(shapedEmployees);
    }

    private List<Entity> ShapeData(IEnumerable<EmployeeDto> employeesDto, string fields) =>
    _dataShaper.ShapeData(employeesDto, fields)
        .Select(e => e.Entity)
        .ToList();

    private bool ShouldGenerateLinks(HttpContext httpContent)
    {
        // We extract the mediaType from the httpContext
        var mediaType = (MediaTypeHeaderValue)httpContent.Items["AcceptHeaderMediaType"];

        // If that media type ends with hateoas, the method returns true; otherwise, it returns false.
        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas",
            StringComparison.InvariantCultureIgnoreCase);
    }

    // returns a new LinkResponse with the ShapedEntities property populated. By default, the HasLinks property is false.
    private LinkResponse ReturnShapedEmployees(List<Entity> shapedEmployees) =>
        new LinkResponse { ShapedEntities = shapedEmployees };

    private LinkResponse ReturnLinkedEmployees(
        IEnumerable<EmployeeDto> employeesDto, 
        string fields, 
        Guid companyId, 
        HttpContext httpContext,
        List<Entity> shapedEmployees)
    {
        var employeeDtoList = employeesDto.ToList();

        for (int index = 0; index < employeeDtoList.Count; index++)
        {
            // We create links for each employee
            var employeeLinks = CreateLinksForEmployee(httpContext, companyId, employeeDtoList[index].Id, fields);
            // We add it to the Collection
            shapedEmployees[index].Add("Links", employeeLinks);
        }

        // We wrap the collection
        var employeeCollection = new LinkCollectionWrapper<Entity>(shapedEmployees);

        // We create links that are important for the entire collection
        var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);

        return new LinkResponse { HasLink = true, LinkedEntities = linkedEmployees };
    }

    private List<Link> CreateLinksForEmployee(HttpContext httpContext, Guid companyId, Guid id, string fields= "")
    {
        // We create the links
        var links = new List<Link>
        {
            new Link(_linkGenerator.GetUriByAction(httpContext, "GetEmployeeForCompany", 
                values: new { companyId, id, fields }),
                "self",
                "GET"),
            new Link(_linkGenerator.GetUriByAction(httpContext, "DeleteEmployeeForCompany",
                    values: new { companyId, id }),
                "delete_employee",
                "DELETE"),
            new Link(_linkGenerator.GetUriByAction(httpContext, "UpdateEmployeeForCompany",
                    values: new { companyId, id }),
                "update_employee",
                "PUT"),
            new Link(_linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateEmployeeForCompany",
                    values: new { companyId, id }),
                "partially_update_employee",
                "PATCH")
        };

        return links;
    }

    private LinkCollectionWrapper<Entity> CreateLinksForEmployees(HttpContext httpContext, LinkCollectionWrapper<Entity> employeesWrapper)
    {
        employeesWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, "GetEmployeesForCompany",
            values: new {}),
            "self",
            "GET"));

        return employeesWrapper;
    }
}