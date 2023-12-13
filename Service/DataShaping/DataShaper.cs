using System.Collections;
using System.Dynamic;
using System.Reflection;
using Contracts;
using Entities.Models;

namespace Service.DataShaping;

public class DataShaper<T> : IDataShaper<T> where T : class
{
    // Array of properties pulled out from the input type
    public PropertyInfo[] Properties { get; set; }

    // We get all the properties of an input class
    public DataShaper()
    {
        Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }


    /**
     * Parses the input string  and returns just the properties needed to return to controller
     */
    public IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
    {
        var requiredProperties = new List<PropertyInfo>();

        // If the fieldsstring is not empty 
        if (!string.IsNullOrWhiteSpace(fieldsString))
        {
            //we split it.
            var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var field in fields)
            {
                // we check if the fields match the properties in the entity
                var property = Properties.FirstOrDefault(pi =>
                    pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));

                if (property == null)
                {
                    continue;
                }
                
                // We add them to the list of required properties
                requiredProperties.Add(property);
            }
        }
        else
        {
            // Otherwise all properties are required
            requiredProperties = Properties.ToList();
        }

        return requiredProperties;
    }

    public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString)
    {
        var requiredProperties = GetRequiredProperties(fieldsString);

        return FetchData(entities, requiredProperties);
    }

    public ShapedEntity ShapeData(T entity, string fieldsString)
    {
        var requiredProperties = GetRequiredProperties(fieldsString);

        return FetchDataForEntity(entity, requiredProperties);
    }

    // Extract value from required properties in multiple entities
    private IEnumerable<ShapedEntity> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
    {
        var shapedData = new List<ShapedEntity>();

        foreach (var entity in entities)
        {
            var shapedObject = FetchDataForEntity(entity, requiredProperties);
            shapedData.Add(shapedObject);
        }

        return shapedData;
    }

    // Extract value from required properties in a single entity
    private ShapedEntity FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredPropertyInfos)
    {
        var shapedObject = new ShapedEntity();

        foreach (var property in requiredPropertyInfos)
        {
            // We extract the property value
            var objectPropertyValue = property.GetValue(entity);

            // add the property object and value to dictionary
            shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
        }

        var objectProperty = entity.GetType().GetProperty("Id");
        shapedObject.Id = (Guid)objectProperty.GetValue(entity);

        return shapedObject;
    }
}