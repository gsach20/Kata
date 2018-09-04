using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ConsoleApplication1
{
    public static class DatasetTrials
    {
        public static void WriteToDataset()
        {
            Console.WriteLine(CultureInfo.CurrentCulture);
            Console.WriteLine();
            Console.WriteLine();
            DataSet dataSet = new DataSet();

            dataSet.Locale = new CultureInfo("de-DE");

            DataTable dataTable = new DataTable("abcd");
            dataSet.Tables.Add(dataTable);
            dataTable.Columns.Add(new DataColumn("Col1", typeof(string)));
            dataTable.Rows.Add(100000000.12345678911);
            //dataTable.Rows.Add("2,5");
            dataTable.Rows.Add(DBNull.Value);
            //dataTable.AcceptChanges();

            dataSet.AcceptChanges();

            dataSet.WriteXml("d:\\temp\\myfile21431.txt", XmlWriteMode.WriteSchema);

            Console.Write(dataSet.GetXml());
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("***************Schema*************");
            Console.WriteLine();
            Console.Write(dataSet.GetXmlSchema());


            DataSet dataSet1 = new DataSet();
            dataSet1.ReadXml("d:\\temp\\myfile21431.txt");

            Console.WriteLine();
            Console.WriteLine();
            Console.Write(dataSet1.GetXml());
        }
    }

    [TestFixture]
    public class TestDatasetTrials
    {
        [Test]
        public void SimpleUnitNameTests()
        {
            DatasetTrials.WriteToDataset();
        }
    }
}
