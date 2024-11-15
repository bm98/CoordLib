using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CoordLib;

namespace NTEST_CoordLib
{
  [TestClass]
  public class DMS
  {
    [TestMethod]
    public void DMS_ParseGen( )
    {
      Assert.AreEqual( 0.12345, Dms.ParseDMS( "0.12345" ), 0.000001 );
      Assert.AreEqual( -0.12345, Dms.ParseDMS( "-0.12345" ), 0.000001 );
      Assert.AreEqual( +0.12345, Dms.ParseDMS( "+0.12345" ), 0.000001 );

      Assert.AreEqual( 2.12345, Dms.ParseDMS( "2.12345" ), 0.000001 );
      Assert.AreEqual( -2.12345, Dms.ParseDMS( "-2.12345" ), 0.000001 );
      Assert.AreEqual( +2.12345, Dms.ParseDMS( "+2.12345" ), 0.000001 );

      Assert.AreEqual( 124.12345, Dms.ParseDMS( "124.12345" ), 0.000001 );
      Assert.AreEqual( -124.12345, Dms.ParseDMS( "-124.12345" ), 0.000001 );
      Assert.AreEqual( +124.12345, Dms.ParseDMS( "+124.12345" ), 0.000001 );

      Assert.AreEqual( 124, Dms.ParseDMS( "124" ), 0.000001 );
      Assert.AreEqual( -124, Dms.ParseDMS( "-124" ), 0.000001 );
      Assert.AreEqual( +124, Dms.ParseDMS( "+124" ), 0.000001 );
    }


    [TestMethod]
    public void DMS_ParseLat1( )
    {
      Assert.AreEqual( 2.12345, Dms.ParseDMS( "2.12345" ), 0.000001 );
      Assert.AreEqual( -2.12345, Dms.ParseDMS( "-2.12345" ), 0.000001 );
      Assert.AreEqual( +2.12345, Dms.ParseDMS( "+2.12345" ), 0.000001 );

      Assert.AreEqual( 0, Dms.ParseDMS( "N0° 00' 00\"" ) );
      Assert.AreEqual( 0, Dms.ParseDMS( "0° 00' 00\"N" ) );
      Assert.AreEqual( 0, Dms.ParseDMS( "n0° 00' 00\"" ) );
      Assert.AreEqual( 2.7777777777777777777777777777778e-4, Dms.ParseDMS( "N0° 00' 01\"" ) );
      Assert.AreEqual( 2.7777777777777777777777777777778e-4, Dms.ParseDMS( "N0° 0' 01\"" ) );
      Assert.AreEqual( 2.7777777777777777777777777777778e-4, Dms.ParseDMS( "N0° 0' 1\"" ) );

      Assert.AreEqual( -2.7777777777777777777777777777778e-4, Dms.ParseDMS( "S0° 00' 01\"" ) );

      Assert.AreEqual( 0, Dms.ParseDMS( "N0° 00' 00.0\"" ) );
      Assert.AreEqual( 0, Dms.ParseDMS( "0° 00' 00.00\"N" ) );
      Assert.AreEqual( 0, Dms.ParseDMS( "n0° 00' 00.000\"" ) );
      Assert.AreEqual( 2.7777777777777777777777777777778e-4, Dms.ParseDMS( "N0° 00' 01.00\"" ) );
      Assert.AreEqual( 2.7777777777777777777777777777778e-4, Dms.ParseDMS( "N0° 0' 01.000\"" ) );
      Assert.AreEqual( 2.7777777777777777777777777777778e-4, Dms.ParseDMS( "N0° 0' 1.00000\"" ) );

      Assert.AreEqual( -2.7777777777777777777777777777778e-4, Dms.ParseDMS( "S0° 00' 01.0000\"" ) );

      Assert.AreEqual( 1.0, Dms.ParseDMS( "N1" ) );
      Assert.AreEqual( 1.0, Dms.ParseDMS( "N1°" ) );
      Assert.AreEqual( 51.0, Dms.ParseDMS( "N51°" ) );
      Assert.AreEqual( 51.883333333333333333333333333333, Dms.ParseDMS( "N51° 53'" ) );
      Assert.AreEqual( 51.275833333333333333333333333333, Dms.ParseDMS( "N51° 16' 33\"" ) );

      Assert.AreEqual( 51.27583675, Dms.ParseDMS( "N51° 16' 33.0123\"" ), 0.000000001 );  // dec on Sec
      Assert.AreEqual( 51.275, Dms.ParseDMS( "N51° 16.5'" ), 0.000000001 );  // dec on Min
      Assert.AreEqual( 51.275, Dms.ParseDMS( "N51 16.5" ), 0.000000001 );  // dec on Min

      Assert.AreEqual( -1.0, Dms.ParseDMS( "S1" ) );
      Assert.AreEqual( -1.0, Dms.ParseDMS( "S1°" ) );
      Assert.AreEqual( -51.0, Dms.ParseDMS( "S51°" ) );
      Assert.AreEqual( -51.883333333333333333333333333333, Dms.ParseDMS( "S51° 53'" ) );
      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "S51° 16' 33\"" ) );

      Assert.AreEqual( -51.27583675, Dms.ParseDMS( "S51° 16' 33.0123\"" ), 0.000000001 );

      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "S51 16 33" ) );

      Assert.AreEqual( -51.27583675, Dms.ParseDMS( "S51 16 33.0123" ), 0.000000001 );  // dec on Sec
      Assert.AreEqual( -51.275, Dms.ParseDMS( "S51° 16.5'" ), 0.000000001 );  // dec on Min
      Assert.AreEqual( -51.275, Dms.ParseDMS( "S51 16.5" ), 0.000000001 );  // dec on Min


      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "S51_16_33_", separator: '_' ) );
      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "S51°_16'_33\"_", separator: '_' ) );

      Assert.AreEqual( 51.275833333333333333333333333333, Dms.ParseDMS( "N511633" ) );
      Assert.AreEqual( 51.275833333333333333333333333333, Dms.ParseDMS( "511633N" ) );
      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "S511633" ) );
      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "511633S" ) );

      Assert.AreEqual( 51.26666666666666666666666666667, Dms.ParseDMS( "N5116" ) );
      Assert.AreEqual( 51.26666666666666666666666666667, Dms.ParseDMS( "5116N" ) );
      Assert.AreEqual( -51.26666666666666666666666666667, Dms.ParseDMS( "S5116" ) );
      Assert.AreEqual( -51.26666666666666666666666666667, Dms.ParseDMS( "5116S" ) );
    }

    [TestMethod]
    public void DMS_ParseLon1( )
    {
      Assert.AreEqual( 0, Dms.ParseDMS( "E0° 00' 00\"" ) );
      Assert.AreEqual( 0, Dms.ParseDMS( "0° 00' 00\"W" ) );
      Assert.AreEqual( 0, Dms.ParseDMS( "w0° 00' 00\"" ) );

      Assert.AreEqual( 1.0, Dms.ParseDMS( "E1" ) );

      Assert.AreEqual( 1.0, Dms.ParseDMS( "E1°" ) );
      Assert.AreEqual( 51.0, Dms.ParseDMS( "E51°" ) );
      Assert.AreEqual( 151.0, Dms.ParseDMS( "E151°" ) );

      Assert.AreEqual( 51.883333333333333333333333333333, Dms.ParseDMS( "E51° 53'" ) );
      Assert.AreEqual( 51.275833333333333333333333333333, Dms.ParseDMS( "E51° 16' 33\"" ) );

      Assert.AreEqual( 51.27583675, Dms.ParseDMS( "E51° 16' 33.0123\"" ), 0.000000001 );
      Assert.AreEqual( 51.275, Dms.ParseDMS( "E51° 16.5'" ), 0.000000001 );  // dec on Min
      Assert.AreEqual( 51.275, Dms.ParseDMS( "E051 16.5" ), 0.000000001 );  // dec on Min

      Assert.AreEqual( 151.883333333333333333333333333333, Dms.ParseDMS( "E151° 53'" ) );

      Assert.AreEqual( -1.0, Dms.ParseDMS( "W1" ) );
      Assert.AreEqual( -1.0, Dms.ParseDMS( "W1°" ) );
      Assert.AreEqual( -51.0, Dms.ParseDMS( "W51°" ) );
      Assert.AreEqual( -151.0, Dms.ParseDMS( "W151°" ) );

      Assert.AreEqual( -51.883333333333333333333333333333, Dms.ParseDMS( "W51° 53'" ) );
      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "W51° 16' 33\"" ) );

      Assert.AreEqual( -51.27583675, Dms.ParseDMS( "W51° 16' 33.0123\"" ), 0.000000001 );
      Assert.AreEqual( -51.275, Dms.ParseDMS( "W51° 16.5'" ), 0.000000001 );  // dec on Min
      Assert.AreEqual( -51.275, Dms.ParseDMS( "W51 16.5" ), 0.000000001 );  // dec on Min

      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "W51 16 33" ) );

      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "W51_16_33", separator: '_' ) );
      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "W51°_16'_33\"_", separator: '_' ) );

      Assert.AreEqual( -151.883333333333333333333333333333, Dms.ParseDMS( "W151° 53'" ) );
      Assert.AreEqual( -151.275833333333333333333333333333, Dms.ParseDMS( "W151° 16' 33\"" ) );
      Assert.AreEqual( -151.275, Dms.ParseDMS( "W151° 16.5'" ), 0.000000001 );  // dec on Min
      Assert.AreEqual( -151.275, Dms.ParseDMS( "W151 16.5" ), 0.000000001 );  // dec on Min

      Assert.AreEqual( 151.275833333333333333333333333333, Dms.ParseDMS( "E1511633" ) );
      Assert.AreEqual( 151.275833333333333333333333333333, Dms.ParseDMS( "1511633E" ) );
      Assert.AreEqual( -151.275833333333333333333333333333, Dms.ParseDMS( "W1511633" ) );
      Assert.AreEqual( -151.275833333333333333333333333333, Dms.ParseDMS( "1511633W" ) );

      Assert.AreEqual( 151.26666666666666666666666666667, Dms.ParseDMS( "E15116" ) );
      Assert.AreEqual( 151.26666666666666666666666666667, Dms.ParseDMS( "15116E" ) );
      Assert.AreEqual( -151.26666666666666666666666666667, Dms.ParseDMS( "W15116" ) );
      Assert.AreEqual( -151.26666666666666666666666666667, Dms.ParseDMS( "15116W" ) );
    }

    [TestMethod]
    public void DMS_ParseFail( )
    {
      // invalid ones: returns double.NaN

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "1254° 01' 01\"" ) ); // missing Lat/Lon designator
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "N14° 01' 01\"N" ) ); // both Lat/Lon designators

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "Q54° 01' 01\"" ) ); // invalid Lat/Lon designator
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "54° 01' 01\"Q" ) ); // invalid Lat/Lon designator

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "E54° 01' 01.\"" ) ); // invalid solo dec point

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "Q54 01 01" ) ); // invalid Lat/Lon designator
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "54 01 01Q" ) ); // invalid Lat/Lon designator

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "Q1511633" ) ); // invalid Lat/Lon designator

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "W16' 33\"" ) ); // missing deg
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "16' 33\"N" ) ); // missing deg

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "W16° 33\"" ) ); // missing min
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "16° 33\"N" ) ); // missing min

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "W33\"" ) ); // missing deg, min
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "33\"N" ) ); // missing deg, min

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "W51_16 33" ) ); // unknown separator
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "51_16 33S" ) ); // unknown separator

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "N1254° 01' 01\"" ) ); // too many deg digits
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "1254° 01' 01\"W" ) ); // too many deg digits

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "N° 0' 0\"" ) ); // missing deg digit
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "N0° ' 0\"" ) ); // missing min digit
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "N0° 0' \"" ) ); // missing sec digit

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "° 0' 0\"W" ) ); // missing deg digit
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "0° ' 0\"W" ) ); // missing min digit
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "0° 0' \"W" ) ); // missing sec digit

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "N 1511633" ) ); // invalid space
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "1511633 W" ) ); // invalid space

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "N1511633" ) ); // Lat with 3 digit deg
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "W511633" ) ); // Lon with 2 digit deg

      Assert.AreEqual( double.NaN, Dms.ParseDMS( "N15116" ) ); // Lat with 3 digit deg, too many digits
      Assert.AreEqual( double.NaN, Dms.ParseDMS( "W5116" ) ); // Lon with 2 digit deg, too many digits

      Assert.AreEqual( LatLon.Empty, Dms.ParseRouteCoord( "123018N1123018 E" ) ); // invalid space

      Assert.AreEqual( "", Dms.ToRouteCoord( new LatLon( 12.5050, 112.5050 ), "dmx" ) ); // invalid format
    }

    [TestMethod]
    public void DMS_ParseRouteCoord( )
    {
      Assert.AreEqual( "123018N1123018E", Dms.ToRouteCoord( new LatLon( 12.5050, 112.5050 ), "dms" ) );
      Assert.AreEqual( "1230N11230E", Dms.ToRouteCoord( new LatLon( 12.50, 112.50 ), "dm" ) );
      Assert.AreEqual( "123018S1123018W", Dms.ToRouteCoord( new LatLon( -12.5050, -112.5050 ), "dms" ) );
      Assert.AreEqual( "1230S11230W", Dms.ToRouteCoord( new LatLon( -12.50, -112.50 ), "dm" ) );

      // take care of rounding issues from Dec to DMS and back when using AreEqual...
      Assert.AreEqual( new LatLon( 12.5050, 112.5050 ), Dms.ParseRouteCoord( "123018N1123018E" ) );
      Assert.AreEqual( new LatLon( -12.5050, -112.5050 ), Dms.ParseRouteCoord( "123018S1123018W" ) );
    }

    [TestMethod]
    public void DMS_ParseRouteCoord2( )
    {
      double lat = 47.533333333333333333333333333333; // N47 32 00
      double lon = 9.8333333333333333333333333333333; // E009 50 00
      // make sure we have a working target..
      Assert.AreEqual( lat, Dms.ParseDMS( "N47 32' 00\"" ), 0.000000001 );
      Assert.AreEqual( lon, Dms.ParseDMS( "E009 50' 00\"" ), 0.000000001 );
      Assert.AreEqual( new LatLon( lat, lon ), Dms.ParseRouteCoord( "4732N00950E" ) );

      Assert.AreEqual( lat, Dms.ParseDMS( "N47 32'" ), 0.000000001 );
      Assert.AreEqual( lon, Dms.ParseDMS( "E009 50'" ), 0.000000001 );
      Assert.AreEqual( new LatLon( lat, lon ), Dms.ParseRouteCoord( "4732N00950E" ) );

      lat = -47.533333333333333333333333333333; // S47 32 00
      lon = -9.8333333333333333333333333333333; // W009 50 00
      // make sure we have a working target..
      Assert.AreEqual( lat, Dms.ParseDMS( "S47 32' 00\"" ), 0.000000001 );
      Assert.AreEqual( lon, Dms.ParseDMS( "W009 50' 00\"" ), 0.000000001 );
      Assert.AreEqual( new LatLon( lat, lon ), Dms.ParseRouteCoord( "4732S00950W" ) );

    }

    [TestMethod]
    public void DMS_ToDMSarray( )
    {

      var x = Dms.ToDMSarray( 0, false );

    }


  }
}
