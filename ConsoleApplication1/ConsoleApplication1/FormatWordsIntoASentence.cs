using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1
{
    class FormatWordsIntoASentence
    {
        public static string FormatWords(string[] words)
        {
            if(words == null) return string.Empty;
            List<string> filterEmpty = words.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            List<string> exceptLast = new List<string>(filterEmpty);
            if(exceptLast.Count > 1) exceptLast.RemoveAt(exceptLast.Count-1);

            string retVal = exceptLast.Any() ? exceptLast.Aggregate((a, b) => a + ", " + b) : string.Empty;
            if (filterEmpty.Count > 1)
                return retVal + " and " + filterEmpty.Last();
            return retVal;
        }
    }

    [TestFixture]
    public class Sample_Tests
    {
        private static IEnumerable<TestCaseData> testCases
        {
            get
            {
                yield return new TestCaseData(new[] { new string[] { "one", "two", "three", "four" } })
                    .Returns("one, two, three and four")
                    .SetDescription("{\"one\", \"two\", \"three\", \"four\"} should return \"one, two, three and four\"");
                yield return new TestCaseData(new[] { new string[] { "one" } })
                    .Returns("one")
                    .SetDescription("{\"one\"} should return \"one\"");
                yield return new TestCaseData(new[] { new string[] { "one", "", "three" } })
                    .Returns("one and three")
                    .SetDescription("{\"one\", \"\", \"three\"} should return \"one and three\"");
                yield return new TestCaseData(new[] { new string[] { "", "", "three" } })
                    .Returns("three")
                    .SetDescription("{\"\", \"\", \"three\"} should return \"three\"");
                yield return new TestCaseData(new[] { new string[] { "one", "two", "" } })
                    .Returns("one and two")
                    .SetDescription("{\"one\", \"two\", \"\"} should return \"one and two\"");
                yield return new TestCaseData(new[] { new string[] { } })
                    .Returns("")
                    .SetDescription("{} should return \"\"");
                yield return new TestCaseData(null)
                    .Returns("")
                    .SetDescription("null should return \"\"");
                yield return new TestCaseData(new[] { new string[] { "" } })
                    .Returns("")
                    .SetDescription("{\"\"} should return \"\"");
            }
        }

        [Test, TestCaseSource("testCases")]
        public string Test(string[] words) => FormatWordsIntoASentence.FormatWords(words);
    }
}
