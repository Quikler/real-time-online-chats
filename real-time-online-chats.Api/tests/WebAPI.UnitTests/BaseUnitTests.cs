using AutoFixture;

namespace WebAPI.UnitTests;

public class BaseUnitTests
{
    protected static Fixture Fixture { get; }

    static BaseUnitTests()
    {
        Fixture = new Fixture();
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
}