using MikesPaging.AspNetCore.UnitTests.Models;

namespace MikesPaging.AspNetCore.UnitTests;

internal static class Data
{
    public static readonly TestEntity[] TestEntities =
    [
       new TestEntity
       {
           Id = Guid.NewGuid(),
           Age = 2,
           Created = DateTimeOffset.UtcNow,
           FirstName = "John",
           LastName = "Doe",
           IQ = 5,
           ComplexType = new ComplexType("Test1", "Value1RealMadrid"),
           RelatedCollection =
           [
               new ComplexType("Jack",
                   "Babba"),
               new ComplexType("Caramel",
                   "Davinchi"),
               new ComplexType("8",
                   "9"),
               new ComplexType("gg",
                   "glhf"),
               new ComplexType("League", 
                   "OfLegends")
           ]
       },
       new TestEntity
       {
           Id = Guid.NewGuid(),
           Age = 3,
           Created = DateTimeOffset.UtcNow,
           FirstName = "Mike",
           LastName = "Vazovskiy",
           IQ = null,
           ComplexType = new ComplexType("Test2", "Value2NEW"),
           RelatedCollection =
           [
               new ComplexType("Jack",
                   "Doe"),
               new ComplexType("Lolipop",
                   "Tasty"),
               new ComplexType("gg",
                   "glhf"), ]
       },
       new TestEntity
       {
           Id = Guid.NewGuid(),
           Age = 1,
           Created = DateTimeOffset.UtcNow,
           FirstName = "Dr.",
           LastName = "ForNever",
           IQ = 8,
           ComplexType = new ComplexType("Test3", "Value3Train"),
           RelatedCollection =
           [
               new ComplexType("Empty",
                   "String"),
               new ComplexType("New",
                   "Guid"),
               new ComplexType("Moscow",
                   "Crime"),
               new ComplexType("11",
                   "3"),
           ],
       }
    ];
}
