using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;

namespace MikesPaging.AspNetCore.UnitTests.Models;

internal class TestEntityFilteringConfiguration : FilteringConfiguration<TestEntity, TestEntityFilteringEnum>
{
    public TestEntityFilteringConfiguration()
    {
        FilterFor(TestEntityFilteringEnum.ByComplexTypeTitle, FilteringOperators.Contains, e =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(e);
            return i => i.ComplexType.Title.Contains(e);
        });
        FilterFor(TestEntityFilteringEnum.ByComplexTypeValue, FilteringOperators.StartsWith, e =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(e);
            return i => i.ComplexType.Value.StartsWith(e);
        });
        FilterFor(TestEntityFilteringEnum.ByRelatedCollection, FilteringOperators.Contains, e =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(e);
            return x => x.RelatedCollection.Any(o => o.Title == e);
        });
    }
}