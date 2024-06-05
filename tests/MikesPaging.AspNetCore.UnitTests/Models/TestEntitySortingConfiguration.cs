using MikesPaging.AspNetCore.Common;

namespace MikesPaging.AspNetCore.UnitTests.Models;

internal class TestEntitySortingConfiguration : SortingConfiguration<TestEntity, TestEntitySortingEnum>
{
    public TestEntitySortingConfiguration()
    {
        SortFor(TestEntitySortingEnum.ByRelatedCollectionCount, e => e.RelatedCollection.Count());
    }
}