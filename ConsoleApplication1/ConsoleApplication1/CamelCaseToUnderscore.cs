using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1
{
    public static class CamelCaseTranslator
    {
        public static string ToUnderScore(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            return name.Substring(1).Aggregate(name[0].ToString(), 
                (a, b) => a + ((char.IsUpper(b) || char.IsNumber(b) && !char.IsNumber(a.LastOrDefault())) && a.LastOrDefault() != '_' ? "_" : "") + b);
        }
    }

    [TestFixture]
    public class CamelCaseTranslatorTests
    {
        [Test]
        public void SimpleUnitNameTests()
        {
            Assert.AreEqual("This_Is_A_Unit_Test", CamelCaseTranslator.ToUnderScore("ThisIsAUnitTest"));
            Assert.AreEqual("This_Should_Be_Splitted_Correct_Into_Underscore", CamelCaseTranslator.ToUnderScore("ThisShouldBeSplittedCorrectIntoUnderscore"));
        }

        [Test]
        public void CalculationUnitNameTests()
        {
            Assert.AreEqual("Calculate_1_Plus_1_Equals_2", CamelCaseTranslator.ToUnderScore("Calculate1Plus1Equals2"));
            Assert.AreEqual("Calculate_15_Plus_5_Equals_20", CamelCaseTranslator.ToUnderScore("Calculate15Plus5Equals20"));
            Assert.AreEqual("Calculate_500_Divided_By_5_Equals_100", CamelCaseTranslator.ToUnderScore("Calculate500DividedBy5Equals100"));
        }

        [Test]
        public void SpecialUnitNameTests()
        {
            Assert.AreEqual("This_Is_Already_Splitted_Correct", CamelCaseTranslator.ToUnderScore("This_Is_Already_Splitted_Correct"));
            Assert.AreEqual("This_Is_Not_Splitted_Correct", CamelCaseTranslator.ToUnderScore("ThisIs_Not_SplittedCorrect"));
        }
    }
}
