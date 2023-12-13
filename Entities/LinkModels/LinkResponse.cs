using Entities.Models;

namespace Entities.LinkModels;

public class LinkResponse
{
    public bool HasLink { get; set; }
    public List<Entity> ShapedEntities { get; set; }
    public LinkCollectionWrapper<Entity> LinkedEntities { get; set; }
    public LinkResponse()
    {
        LinkedEntities = new LinkCollectionWrapper<Entity>(); // If response has links
        ShapedEntities = new List<Entity>(); // Otherwise
    }
}