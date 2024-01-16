namespace MikesPaging.AspNetCore.Common;

public abstract class SortingEnum : MikesPagingEnum
{
    protected SortingEnum(string propertyName, IReadOnlyCollection<string> allowedNames) : base(propertyName, allowedNames)
    {

    }

    protected SortingEnum(string propertyName, IReadOnlyCollection<string> allowedNames, bool ignoreCase = true) : base(propertyName, allowedNames, ignoreCase)
    {

    }
    protected sealed override IEnumerable<object?> GetEqualityComponents()
    {
        return base.GetEqualityComponents();
    }
}