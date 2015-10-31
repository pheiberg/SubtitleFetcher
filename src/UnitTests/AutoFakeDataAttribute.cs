using System;
using System.Collections.Generic;
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

        public AutoFakeDataAttribute(Type[] customizationTypes)
        {
            Fixture.Customize(new AutoFakeItEasyCustomization());
            var customizations = customizationTypes.Select(type => (ICustomization)Activator.CreateInstance(type));
            foreach (var customization in customizations)
            {
                Fixture.Customize(customization);
            }
        }
    }
}