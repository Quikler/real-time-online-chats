using AutoFixture;

namespace WebAPI.UnitTests;

public class BaseUnitTests
{
    protected virtual Fixture Fixture { get; init; }

    public BaseUnitTests()
    {
        Fixture = new Fixture();
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
}