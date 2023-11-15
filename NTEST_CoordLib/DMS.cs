using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CoordLib;

namespace NTEST_CoordLib
{
  [TestClass]
  public class DMS
  {
    [TestMethod]
    public void DMS_ParseLat1( )
    {
      Assert.AreEqual( 0, Dms.ParseDMS( "N0° 00' 00\"" ) );
      Assert.AreEqual( 2.7777777777777777777777777777778e-4, Dms.ParseDMS( "N0° 00' 01\"" ) );
      Assert.AreEqual( -2.7777777777777777777777777777778e-4, Dms.ParseDMS( "S0° 00' 01\"" ) );


      Assert.AreEqual( 51.0, Dms.ParseDMS( "N51°" ) );
      Assert.AreEqual( 51.883333333333333333333333333333, Dms.ParseDMS( "N51° 53'" ) );
      Assert.AreEqual( 51.275833333333333333333333333333, Dms.ParseDMS( "N51° 16' 33\"" ) );

      Assert.AreEqual( -51.0, Dms.ParseDMS( "S51°" ) );
      Assert.AreEqual( -51.883333333333333333333333333333, Dms.ParseDMS( "S51° 53'" ) );
      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "S51° 16' 33\"" ) );

      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "S51 16 33" ) );

      Assert.AreEqual( 51.275833333333333333333333333333, Dms.ParseDMS( "N511633" ) );
      Assert.AreEqual( 51.275833333333333333333333333333, Dms.ParseDMS( "511633N" ) );
      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "S511633" ) );
      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "511633S" ) );
    }

    [TestMethod]
    public void DMS_ParseLon1( )
    {
      Assert.AreEqual( 51.0, Dms.ParseDMS( "E51°" ) );
      Assert.AreEqual( 51.883333333333333333333333333333, Dms.ParseDMS( "E51° 53'" ) );
      Assert.AreEqual( 51.275833333333333333333333333333, Dms.ParseDMS( "E51° 16' 33\"" ) );

      Assert.AreEqual( 151.0, Dms.ParseDMS( "E151°" ) );
      Assert.AreEqual( 151.883333333333333333333333333333, Dms.ParseDMS( "E151° 53'" ) );

      Assert.AreEqual( -51.0, Dms.ParseDMS( "W51°" ) );
      Assert.AreEqual( -51.883333333333333333333333333333, Dms.ParseDMS( "W51° 53'" ) );
      Assert.AreEqual( -51.275833333333333333333333333333, Dms.ParseDMS( "W51° 16' 33\"" ) );

      Assert.AreEqual( -151.0, Dms.ParseDMS( "W151°" ) );
      Assert.AreEqual( -151.883333333333333333333333333333, Dms.ParseDMS( "W151° 53'" ) );
      Assert.AreEqual( -151.275833333333333333333333333333, Dms.ParseDMS( "W151° 16' 33\"" ) );

      Assert.AreEqual( 151.275833333333333333333333333333, Dms.ParseDMS( "E1511633" ) );
      Assert.AreEqual( 151.275833333333333333333333333333, Dms.ParseDMS( "1511633E" ) );
      Assert.AreEqual( -151.275833333333333333333333333333, Dms.ParseDMS( "W1511633" ) );
      Assert.AreEqual( -151.275833333333333333333333333333, Dms.ParseDMS( "1511633W" ) );
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

  }
}
