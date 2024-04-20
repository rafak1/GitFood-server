using NUnit.Framework;
using Server.Logic.Abstract;
using Server.Controllers;

namespace Server.Controllers.Tests;

[TestFixture]
public class ManagerActionResultMapperTest
{
    [Test]
    public void AllPossibleValuesOfResultEnumAreHandled()
    {
        foreach(ResultEnum resultEnum in Enum.GetValues(typeof(ResultEnum)))
            Assert.DoesNotThrow(() => resultEnum.MapStatusCodeForResultEnum());
    }
}