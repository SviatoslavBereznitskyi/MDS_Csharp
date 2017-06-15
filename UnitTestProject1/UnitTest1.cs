using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using MDS;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestIsIntesect()
        {
            MDS.MainForm f = new MDS.MainForm();
            Point p1 = new Point(0, 0);
            Point p2 = new Point(5, 5);
            Point p3 = new Point(0, 5);
            Point p4 = new Point(5, 0);
            bool isIntersect = true;
            bool real = f.InteresectOfLine(p1, p2, p3, p4);
            Assert.AreEqual(isIntersect, real);
        }
        [TestMethod]
        public void TestGetDistances()
        {
            MDS.MainForm f = new MDS.MainForm();
            List<string> input = new List<string>{ "BMW_Motorrad:59.46:4.61:4.26:4.32:4.87:2.98:3.70:3.11:1.98:6.29:1.13:3.14:0.17",
                                    "Bugatti_Veyron:69.01:6.08:1.77:1.74:1.74:3.07:1.65:1.66:3.61:4.04:1.34:3.79:0.50",
                                    "Ferrari166:61.55:5.20:2.61:4.31:5.48:1.80:3.45:1.95:2.85:5.93:1.34:2.88:0.64"};
           double[,] distancesMap = f.GetDistances(input);
           double[,] ex = { { 0.0, 11.429, 3.44 }, { 11.429, 0.0, 9.363 }, { 3.44, 9.363, 0.0 } };
           CollectionAssert.AreEqual(distancesMap, ex);
        }
    }
}
