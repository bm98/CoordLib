using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using CoordLib;
using static CoordLib.WMM.MagVarEx;


namespace NTEST_CoordLib
{
  [TestClass]
  public class WMMTest
  {
    [TestMethod]
    public void BasicTests( )
    {
      var mdec_Calc = 0.0;
      var mdec_Look = 0.0;
      var pt = new LatLon( );

      // KSFO MV ~ 12.9
      pt = new LatLon( Dms.ParseDMS( "N37° 37.13'" ), Dms.ParseDMS( "W122° 22.53'" ) );
      mdec_Calc = MagVar_deg( pt, false );
      mdec_Look = MagVar_deg( pt, true );
      Assert.AreEqual( mdec_Calc, mdec_Look, 0.1 ); // lookup squares of 78x78 km at lat==0
      Assert.AreEqual( 12.9, mdec_Calc, 0.5 );

      // OMDB MV ~ 2.35±0.31 (20241115)
      pt = new LatLon( Dms.ParseDMS( "N25°15.17'" ), Dms.ParseDMS( "E055°21.87'" ) );
      mdec_Calc = MagVar_deg( pt, false );
      mdec_Look = MagVar_deg( pt, true );
      Assert.AreEqual( mdec_Calc, mdec_Look, 0.1 ); // lookup squares of 78x78 km at lat==0
      Assert.AreEqual( 2.35, mdec_Calc, 0.5 );

      // EGLL MV ~ 0.87±0.39 (20241115)
      pt = new LatLon( Dms.ParseDMS( "N51°28.65'" ), Dms.ParseDMS( "W000°27.68'" ) );
      mdec_Calc = MagVar_deg( pt, false );
      mdec_Look = MagVar_deg( pt, true );
      Assert.AreEqual( mdec_Calc, mdec_Look, 0.1 ); // lookup squares of 78x78 km at lat==0
      Assert.AreEqual( 0.87, mdec_Calc, 0.5 );

      // FAOB MV ~ -27.46±0.58 (20241115)
      pt = new LatLon( Dms.ParseDMS( "S34°33.8'" ), Dms.ParseDMS( "E020°15.1'" ) );
      mdec_Calc = MagVar_deg( pt, false );
      mdec_Look = MagVar_deg( pt, true );
      Assert.AreEqual( mdec_Calc, mdec_Look, 0.1 ); // lookup squares of 78x78 km at lat==0
      Assert.AreEqual( -27.46, mdec_Calc, 0.5 );

    }


    [TestMethod]
    public void BearingTests( )
    {
      var mdec_Calc = 0.0;
      var ptFrom = new LatLon( );
      var ptTo = new LatLon( );

      // setup and calc manual
      ptFrom = new LatLon( Dms.ParseDMS( "N37° 37.13'" ), Dms.ParseDMS( "W122° 22.53'" ) ); // KSFO
      ptTo = new LatLon( Dms.ParseDMS( "N37°36.36'" ), Dms.ParseDMS( "W122°02.92'" ) ); // DECOT
      var brg = ptFrom.BearingTo( ptTo ); // True
      mdec_Calc = MagVar_deg( ptTo, false );
      var brgM = brg - mdec_Calc; // neg MDec will be added (Abs(MDec)), pos will be subtracted
      // -> MAG
      Assert.AreEqual( brgM, MagFromTrueBearing( brg, ptTo, false ), 0.001 ); // calc at loc
      Assert.AreEqual( brgM, MagFromTrueBearing( brg, ptTo, true ), 0.5 );    // lookup
      // -> TRUE
      Assert.AreEqual( brg, TrueFromMagBearing( brgM, ptTo, false ), 0.001 ); // calc at loc
      Assert.AreEqual( brg, TrueFromMagBearing( brgM, ptTo, true ), 0.5 );    // lookup

      // true - mag - true
      Assert.AreEqual( brg, TrueFromMagBearing( MagFromTrueBearing( brg, ptTo, false ), ptTo, false ), 0.000001 ); // calc at loc
      Assert.AreEqual( brg, TrueFromMagBearing( MagFromTrueBearing( brg, ptTo, true ), ptTo, true ), 0.000001 ); // lookup


      // setup and calc manual
      ptFrom = new LatLon( Dms.ParseDMS( "N 49°06.52'" ), Dms.ParseDMS( "E 008°39.95'" ) ); // BANUX  TrueH=114°(114.8)
      ptTo = new LatLon( Dms.ParseDMS( "N 48°43.42'" ), Dms.ParseDMS( "E 009°54.42'" ) ); // VEKIR 
      brg = ptFrom.BearingTo( ptTo ); // True
      mdec_Calc = MagVar_deg( ptTo, false );
      brgM = brg - mdec_Calc; // neg MDec will be added (Abs(MDec)), pos will be subtracted
      // -> MAG
      Assert.AreEqual( brgM, MagFromTrueBearing( brg, ptTo, false ), 0.001 ); // calc at loc
      Assert.AreEqual( brgM, MagFromTrueBearing( brg, ptTo, true ), 0.5 );    // lookup
      // -> TRUE
      Assert.AreEqual( brg, TrueFromMagBearing( brgM, ptTo, false ), 0.001 ); // calc at loc
      Assert.AreEqual( brg, TrueFromMagBearing( brgM, ptTo, true ), 0.5 );    // lookup

      // true - mag - true
      Assert.AreEqual( brg, TrueFromMagBearing( MagFromTrueBearing( brg, ptTo, false ), ptTo, false ), 0.000001 ); // calc at loc
      Assert.AreEqual( brg, TrueFromMagBearing( MagFromTrueBearing( brg, ptTo, true ), ptTo, true ), 0.000001 ); // lookup


      // setup and calc manual
      ptFrom = new LatLon( Dms.ParseDMS( "N 25°36.75'" ), Dms.ParseDMS( "E 054°51.82'" ) ); // VUTEB  TrueH=128°(128.4)
      ptTo = new LatLon( Dms.ParseDMS( "N 25°15.17'" ), Dms.ParseDMS( "E 055°21.87'" ) ); // OMDB 
      brg = ptFrom.BearingTo( ptTo ); // True
      mdec_Calc = MagVar_deg( ptTo, false );
      brgM = brg - mdec_Calc; // neg MDec will be added (Abs(MDec)), pos will be subtracted
      // -> MAG
      Assert.AreEqual( brgM, MagFromTrueBearing( brg, ptTo, false ), 0.001 ); // calc at loc
      Assert.AreEqual( brgM, MagFromTrueBearing( brg, ptTo, true ), 0.5 );    // lookup
      // -> TRUE
      Assert.AreEqual( brg, TrueFromMagBearing( brgM, ptTo, false ), 0.001 ); // calc at loc
      Assert.AreEqual( brg, TrueFromMagBearing( brgM, ptTo, true ), 0.5 );    // lookup

      // true - mag - true
      Assert.AreEqual( brg, TrueFromMagBearing( MagFromTrueBearing( brg, ptTo, false ), ptTo, false ), 0.000001 ); // calc at loc
      Assert.AreEqual( brg, TrueFromMagBearing( MagFromTrueBearing( brg, ptTo, true ), ptTo, true ), 0.000001 ); // lookup


      // setup and calc manual
      ptFrom = new LatLon( Dms.ParseDMS( "N 51°07.85'" ), Dms.ParseDMS( "E 002°00.00'" ) ); // KONAN  TrueH=95°(94.8)
      ptTo = new LatLon( Dms.ParseDMS( "N 51°05.68'" ), Dms.ParseDMS( "E 002°39.10'" ) ); // KOK  (VOR)
      brg = ptFrom.BearingTo( ptTo ); // True
      mdec_Calc = MagVar_deg( ptTo, false );
      brgM = brg - mdec_Calc; // neg MDec will be added (Abs(MDec)), pos will be subtracted
      // -> MAG
      Assert.AreEqual( brgM, MagFromTrueBearing( brg, ptTo, false ), 0.001 ); // calc at loc
      Assert.AreEqual( brgM, MagFromTrueBearing( brg, ptTo, true ), 0.5 );    // lookup
      // -> TRUE
      Assert.AreEqual( brg, TrueFromMagBearing( brgM, ptTo, false ), 0.001 ); // calc at loc
      Assert.AreEqual( brg, TrueFromMagBearing( brgM, ptTo, true ), 0.5 );    // lookup

      // true - mag - true
      Assert.AreEqual( brg, TrueFromMagBearing( MagFromTrueBearing( brg, ptTo, false ), ptTo, false ), 0.000001 ); // calc at loc
      Assert.AreEqual( brg, TrueFromMagBearing( MagFromTrueBearing( brg, ptTo, true ), ptTo, true ), 0.000001 ); // lookup


    }
  }
}
