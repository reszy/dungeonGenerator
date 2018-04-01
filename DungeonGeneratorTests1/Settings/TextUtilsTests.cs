using Microsoft.VisualStudio.TestTools.UnitTesting;
using DungeonGenerator.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator.Settings.Tests
{
    [TestClass()]
    public class TextUtilsTests
    {
        [TestMethod()]
        public void ConvertFromHumanReadableTest()
        {
            string input = "Test string";
            string output = TextUtils.ConvertFromHumanReadable(input);
            Assert.AreEqual("TestString", output);
        }

        [TestMethod()]
        public void ConvertToHumanReadableTest()
        {
            string input = "TestString";
            string output = TextUtils.ConvertToHumanReadable(input);
            Assert.AreEqual("Test string", output);
        }
    }
}