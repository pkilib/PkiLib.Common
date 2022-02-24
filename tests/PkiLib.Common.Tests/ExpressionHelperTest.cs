using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Org.PkiLib.Common.Tests
{
    [TestFixture]
    public class ExpressionHelperTest
    {
        public class BaseClass
        {
            public BaseClass(bool boolean)
            {
                Boolean = boolean;
            }

            public bool Boolean { get; set; }
        }

        public class DerivedClass : BaseClass
        {
            public DerivedClass(bool boolean, int integer) : base(boolean)
            {
                Integer = integer;
            }

            public int Integer { get; set; }
        }

        [Test]
        public void ConvertResult_Simple_Test()
        {
            Expression<Func<int, bool>> notNullExpression = i => i != 0;
            Expression<Func<int, object>> notNullObjectResultExpression = ExpressionHelper.ConvertResult<int, bool, object>(notNullExpression);

            Func<int, bool> notNull = notNullExpression.Compile();
            Func<int, object> notNullObjectResult = notNullObjectResultExpression.Compile();

            Assert.True(notNull(1));
            Assert.False(notNull(0));
            Assert.AreEqual(true, notNullObjectResult(1));
            Assert.AreEqual(false, notNullObjectResult(0));

            Assert.Pass();
        }

        [Test]
        public void ConvertResult_Inheritance_Test()
        {
            Expression<Func<(bool boolean, int integer), DerivedClass>> derivedClassExpression = x => new DerivedClass(x.boolean, x.integer);
            Expression<Func<(bool boolean, int integer), BaseClass>> baseClassExpression = ExpressionHelper.ConvertResult<(bool boolean, int integer), DerivedClass, BaseClass>(derivedClassExpression);
            Expression<Func<(bool boolean, int integer), DerivedClass>> derivedClassFromBaseClassExpression = ExpressionHelper.ConvertResult<(bool boolean, int integer), BaseClass, DerivedClass>(baseClassExpression);

            Func<(bool boolean, int integer), DerivedClass> derivedClass = derivedClassExpression.Compile();
            Func<(bool boolean, int integer), BaseClass> baseClass = baseClassExpression.Compile();
            Func<(bool boolean, int integer), DerivedClass> derivedClassFromBaseClass = derivedClassFromBaseClassExpression.Compile();

            (bool boolean, int integer) trueAndOne = (true, 1);
            (bool boolean, int integer) falseAndZero = (false, 0);

            Assert.True(derivedClass(trueAndOne).Boolean);
            Assert.False(derivedClass(falseAndZero).Boolean);
            Assert.AreEqual(1, derivedClass(trueAndOne).Integer);
            Assert.AreEqual(0, derivedClass(falseAndZero).Integer);

            Assert.True(baseClass(trueAndOne).Boolean);
            Assert.False(baseClass(falseAndZero).Boolean);

            Assert.True(derivedClassFromBaseClass(trueAndOne).Boolean);
            Assert.False(derivedClassFromBaseClass(falseAndZero).Boolean);
            Assert.AreEqual(1, derivedClassFromBaseClass(trueAndOne).Integer);
            Assert.AreEqual(0, derivedClassFromBaseClass(falseAndZero).Integer);

            Assert.Pass();
        }
    }
}
