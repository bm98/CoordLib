using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using CoordLib;

namespace NTEST_CoordLib
{
  [TestClass]
  public class LatLonBasicTest
  {
    [TestMethod]
    public void TestMethod1( )
    {
      LatLon ll;
      Assert.AreEqual( new LatLon( double.NaN, double.NaN, double.NaN ), LatLon.Empty );
      ll = new LatLon( 12.20, 32.40, 4563.5 );
      Assert.AreEqual( 12.20, ll.Lat );
      Assert.AreEqual( 32.40, ll.Lon );
      Assert.AreEqual( 4563.5, ll.Altitude );

      Assert.AreEqual( ll, new LatLon( ll ) );

    }

  }
}
