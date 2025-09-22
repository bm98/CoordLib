using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using CoordLib;
using CoordLib.Extensions;

using System.Globalization;

namespace NTEST_CoordLib
{
  [TestClass]
  public class LLATests
  {
    [TestMethod]
    public void TestLLA( )
    {
      Assert.AreEqual( true, LLA.TryParseLLA( "N45° 20' 24\", W67° 44' 19\",+032000", out double lat, out double lon, out double alt ) );
      Assert.AreEqual( 45.34, lat, 0.0000001 );
      Assert.AreEqual( -67.738611111111111111111111111111, lon, 0.0000001 );
      Assert.AreEqual( 32000, alt, 0.1 );

      Assert.AreEqual( true, LLA.TryParseLLA( "S45° 20' 24\", E67° 44' 19\",+032000", out lat, out lon, out alt ) );
      Assert.AreEqual( -45.34, lat, 0.0000001 );
      Assert.AreEqual( 67.738611111111111111111111111111, lon, 0.0000001 );
      Assert.AreEqual( 32000, alt, 0.1 );

      Assert.AreEqual( true, LLA.TryParseLLA( "N45° 20' 24.65\", W67° 44' 19.11\",+032000.00", out lat, out lon, out alt ) );
      Assert.AreEqual( 45.340180555555555555555555555556, lat, 0.0000001 );
      Assert.AreEqual( -67.738641666666666666666666666667, lon, 0.0000001 );
      Assert.AreEqual( 32000, alt, 0.1 );

      Assert.AreEqual( true, LLA.TryParseLLA( "S45° 20' 24.65\", E67° 44' 19.11\",+032000.00", out lat, out lon, out alt ) );
      Assert.AreEqual( -45.340180555555555555555555555556, lat, 0.0000001 );
      Assert.AreEqual( 67.738641666666666666666666666667, lon, 0.0000001 );
      Assert.AreEqual( 32000, alt, 0.1 );

      Assert.AreEqual( true, LLA.TryParseLL( "N45° 20' 24.65\", W67° 44' 19.11\"", out lat, out lon ) );
      Assert.AreEqual( 45.340180555555555555555555555556, lat, 0.0000001 );
      Assert.AreEqual( -67.738641666666666666666666666667, lon, 0.0000001 );

      Assert.AreEqual( true, LLA.TryParseLL( "S45° 20' 24.65\", E67° 44' 19.11\"", out lat, out lon ) );
      Assert.AreEqual( -45.340180555555555555555555555556, lat, 0.0000001 );
      Assert.AreEqual( 67.738641666666666666666666666667, lon, 0.0000001 );

      // to LLA  has no leading zeroes but for ALT ...
      // eg from FSIM SDK:   <WorldPosition>N53° 53' 55.87",W166° 32' 40.78",+000000.00</WorldPosition>
      // eg from FSIM SDK:   <WorldPosition>N53° 3' 5.87",W166° 32' 40.78",+000000.00</WorldPosition>
      Assert.AreEqual( "N45° 20' 24.00\",W67° 44' 19.00\",+032000.00", LLA.ToLLA( 45.34, -67.738611111111111111111111111111, 32000 ) );
      Assert.AreEqual( "N45° 20' 24.00\",W67° 44' 19.00\",+032000.00", new LatLon( 45.34, -67.738611111111111111111111111111, 32000 ).AsLLA( ) );

      Assert.AreEqual( "N45° 20' 4.00\",W67° 44' 9.00\",+032000.00", LLA.ToLLA( 45.334444, -67.735833, 32000 ) );
      Assert.AreEqual( "N45° 20' 4.00\",W67° 44' 9.00\",+032000.00", new LatLon( 45.334444, -67.735833, 32000 ).AsLLA( ) );

      Assert.AreEqual( "S45° 20' 24.00\",E67° 44' 19.00\",+032000.00", LLA.ToLLA( -45.34, 67.738611111111111111111111111111, 32000 ) );
      Assert.AreEqual( "S45° 20' 24.00\",E67° 44' 19.00\",+032000.00", new LatLon( -45.34, 67.738611111111111111111111111111, 32000 ).AsLLA( ) );

    }


    [TestMethod]
    public void TestLLA_Global( )
    {
      T_GlobalTools.SetTestCulture( );

      Assert.AreEqual( ",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator ); // check

      TestLLA( );

      T_GlobalTools.ResetCulture( );
    }

  }
}
