namespace Entities.Exceptions;

public class IdParametersBadRequestException : BadRequestException
{
    public IdParametersBadRequestException() : base("Parameter is null")
    {
    }
}