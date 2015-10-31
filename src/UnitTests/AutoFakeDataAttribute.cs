using System;
using System.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.NUnit2;

namespace UnitTests
{
    public class AutoFakeDataAttribute : AutoDataAttribute
    {
        public AutoFakeDataAttribute() :
            base(new Fixture().Customize(new AutoFakeItEasyCustomization()))
        {
            
        }
    }
}