namespace MikesPaging.AspNetCore.Common;

public abstract class FilteringEnum : MikesPagingEnum
{
    protected FilteringEnum(string propertyName, IReadOnlyCollection<string> allowedNames) : base(propertyName, allowedNames)
    {

    }

    protected FilteringEnum(string propertyName, IReadOnlyCollection<string> allowedNames, bool ignoreCase = true) : base(propertyName, allowedNames, ignoreCase)
    {

    }
}