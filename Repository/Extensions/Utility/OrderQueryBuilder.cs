using Entities.Models;
using System.Reflection;
using System.Text;

namespace Repository.Extensions.Utility;

public static class OrderQueryBuilder
{
    public static string CreateOrderQuery<T>(string orderByQueryString)
    {
        var orderParams = orderByQueryString.Trim().Split(',');

        // We get all properties from the Employee object
        var propertyInfos = typeof(Employee).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var orderQueryBuilder = new StringBuilder();

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                continue;
            }

            // We split the query string to have individual fields
            var propertyFromQueryName = param.Split(" ")[0];

            // We check if the fields exist in the property class
            var objectProperty = propertyInfos.FirstOrDefault(pi =>
                pi.Name.Equals(propertyFromQueryName, StringComparison.CurrentCultureIgnoreCase));

            if (objectProperty == null)
            {
                continue;
            }

            // We get the direction from query string
            var direction = param.EndsWith(" desc") ? "descending" : "ascending";

            // We build the query in each loop
            orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction},");
        }

        // We remove excess commmas
        var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

        return orderQuery;
    }
}