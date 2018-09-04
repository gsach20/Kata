using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DictionaryMergerKata
{

    public sealed class DictionaryMerger
    {
        public static Dictionary<TKey, TValue[]> Merge<TKey, TValue>(params Dictionary<TKey, TValue>[] dicts)
        {
            return dicts
                .SelectMany(p => p)
                .GroupBy(p => p.Key, p => p.Value)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
    }

    [TestFixture]
    public class AccountTest
    {
        [Test]
        public void Merge_EmptyDictionaries_Returns_Empty_Dictionary()
        {
            var empty1 = new Dictionary<string, int>();
            var empty2 = new Dictionary<string, int>();
            var empty3 = new Dictionary<string, int>();

            Assert.IsEmpty(DictionaryMerger.Merge(empty1), "Single empty dictionary should create an empty dictionary");
            Assert.IsEmpty(DictionaryMerger.Merge(empty1, empty2), "Two empty dictionaries should create an empty dictionary");
            Assert.IsEmpty(DictionaryMerger.Merge(empty1, empty2, empty3), "Multiple empty dictionaries should create an empty dictionary");
        }

        [Test]
        public void Merge_Single_Dictionary_Returns_Dictionary_With_Same_Content()
        {
            var original = new Dictionary<string, int> { { "A", 1 }, { "B", 2 }, { "C", 3 } };
            var merged = DictionaryMerger.Merge(original);
            var expected = new Dictionary<string, int[]> { { "A", new[] { 1 } }, { "B", new[] { 2 } }, { "C", new[] { 3 } } };
            Compare(merged, expected);
        }

        [Test]
        public void Merge_Two_Simple_Dictionaries_Returns_Combined_Dictionary()
        {
            var original1 = new Dictionary<string, int> { { "A", 1 } };
            var original2 = new Dictionary<string, int> { { "B", 2 } };

            var merged = DictionaryMerger.Merge(original1, original2);
            var expected = new Dictionary<string, int[]> { { "A", new[] { 1 } }, { "B", new[] { 2 } } };
            Compare(merged, expected);
        }

        [Test]
        public void Merge_Two_Dictionaries_With_Multiple_Values_Returns_Combined_Dictionary()
        {
            var original1 = new Dictionary<string, int> { { "A", 1 }, { "B", 2 }, { "C", 3 } };
            var original2 = new Dictionary<string, int> { { "A", 4 }, { "D", 5 } };

            var merged = DictionaryMerger.Merge(original1, original2);
            var expected = new Dictionary<string, int[]> { { "A", new[] { 1, 4 } }, { "B", new[] { 2 } }, { "C", new[] { 3 } }, { "D", new[] { 5 } } };
            Compare(merged, expected);
        }

        private void Compare<TKey, TValue>(Dictionary<TKey, TValue[]> actual, Dictionary<TKey, TValue[]> expected)
        {
            Assert.AreEqual(expected.Count, actual.Count, "The dictionary does not contain the expected number of elements");
            foreach (var actualPair in actual)
            {
                TValue[] expectedValue;
                if (!expected.TryGetValue(actualPair.Key, out expectedValue))
                {
                    Assert.Fail("The result dictionary does not contain all required keys");
                }

                CollectionAssert.AreEquivalent(expectedValue, actualPair.Value, "The values in the dictionary are not matching");
            }
        }
    }
}