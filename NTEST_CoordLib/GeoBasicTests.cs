using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using CoordLib;

namespace NTEST_CoordLib
{
  [TestClass]
  public class GeoBasicTests
  {
    private const double eps = 0.00000001; // to not check undefined skip points of the sawtooth function (numeric issue)
    private const double epsTest = eps * 10;
    [TestMethod]
    public void ConstraintsTests( )
    {
      // *** Wrap90
      Assert.AreEqual( 0.0, Geo.Wrap90( 0.0 ) ); // pass through argument
      Assert.AreEqual( 90.0, Geo.Wrap90( 90.0 ) ); // pass through argument
      Assert.AreEqual( 0.0, Geo.Wrap90( 2.0 * 90.0 - eps ), epsTest ); // defined outcome
      Assert.AreEqual( -90.0, Geo.Wrap90( 3.0 * 90.0 - eps ), epsTest ); // defined outcome

      Assert.AreEqual( -90.0, Geo.Wrap90( -90.0 ) ); // pass through argument
      Assert.AreEqual( 0.0, Geo.Wrap90( 2.0 * -90.0 + eps ), epsTest ); // defined outcome
      Assert.AreEqual( 90.0, Geo.Wrap90( 3.0 * -90.0 + eps ), epsTest ); // defined outcome

      Assert.AreEqual( 89.0, Geo.Wrap90( 90.0 + 1.0 ) ); // defined outcome
      Assert.AreEqual( -1.0, Geo.Wrap90( 2.0 * 90.0 + 1.0 ) ); // defined outcome
      Assert.AreEqual( -89.0, Geo.Wrap90( 3.0 * 90.0 + 1.0 ) ); // defined outcome

      Assert.AreEqual( -89.0, Geo.Wrap90( -90.0 - 1.0 ) ); // defined outcome
      Assert.AreEqual( 1.0, Geo.Wrap90( 2.0 * -90.0 - 1.0 ) ); // defined outcome
      Assert.AreEqual( 89.0, Geo.Wrap90( 3.0 * -90.0 - 1.0 ) ); // defined outcome

      // *** Wrap180 
      // multiples of +-180 are skip points (undefined if + or -180)
      Assert.AreEqual( 0.0, Geo.Wrap180( 0.0 ) ); // pass through argument

      Assert.AreEqual( 180.0, Geo.Wrap180( 180.0 ) ); // pass through argument
      Assert.AreEqual( 0.0, Geo.Wrap180( 2.0 * 180.0 - eps ), epsTest ); // defined outcome
      Assert.AreEqual( 180.0, Geo.Wrap180( 3.0 * 180.0 - eps ), epsTest ); // defined outcome

      Assert.AreEqual( -180.0, Geo.Wrap180( -180.0 ) ); // pass through argument
      Assert.AreEqual( 0.0, Geo.Wrap180( 2.0 * -180.0 + eps ), epsTest ); // defined outcome
      Assert.AreEqual( -180.0, Geo.Wrap180( 3.0 * -180.0 + eps ), epsTest ); // defined outcome

      Assert.AreEqual( -179.0, Geo.Wrap180( 181.0 ) ); // defined outcome
      Assert.AreEqual( 1.0, Geo.Wrap180( 2.0 * 180.0 + 1.0 ) ); // defined outcome
      Assert.AreEqual( -179.0, Geo.Wrap180( 3.0 * 180.0 + 1.0 ) ); // defined outcome

      Assert.AreEqual( 179.0, Geo.Wrap180( -180.0 - 1.0 ) ); // defined outcome
      Assert.AreEqual( -1.0, Geo.Wrap180( 2.0 * -180.0 - 1.0 ) ); // defined outcome
      Assert.AreEqual( 179.0, Geo.Wrap180( 3.0 * -180.0 - 1.0 ) ); // defined outcome

      // *** Wrap360
      // multiples of +-360 are skip points (undefined if 0 or 360)
      // Double version
      Assert.AreEqual( 0.0, Geo.Wrap360( 0.0 ) );  // pass through argument
      Assert.AreEqual( 180.7554847, Geo.Wrap360( 180.7554847 ) ); // pass through argument
      Assert.AreEqual( 359.0, Geo.Wrap360( -1.0 ) ); // defined outcome
      Assert.AreEqual( 180.015, Geo.Wrap360( -180.0 + 0.015 ), 0.0001 );

      Assert.AreEqual( 0.0, Geo.Wrap360( 1.0 * 360.0 + eps ), epsTest ); // defined outcome
      Assert.AreEqual( 0.0, Geo.Wrap360( 2.0 * 360.0 + eps ), epsTest ); // defined outcome
      Assert.AreEqual( 0.0, Geo.Wrap360( 3.0 * 360.0 + eps ), epsTest ); // defined outcome

      Assert.AreEqual( 0.0, Geo.Wrap360( 1.0 * -360.0 + eps ), epsTest ); // defined outcome
      Assert.AreEqual( 0.0, Geo.Wrap360( 2.0 * -360.0 + eps ), epsTest ); // defined outcome
      Assert.AreEqual( 0.0, Geo.Wrap360( 3.0 * -360.0 + eps ), epsTest ); // defined outcome

      Assert.AreEqual( 35.0, Geo.Wrap360( 3.0 * 360.0 + 35.0 ) );
      Assert.AreEqual( 180.7554847, Geo.Wrap360( 3.0 * 360.0 + 180.7554847 ), 0.00000001 ); // match precision

      // Int version
      Assert.AreEqual( 0, Geo.Wrap360( 0 ) ); // pass through argument
      Assert.AreEqual( 0, Geo.Wrap360( 360 ) );
      Assert.AreEqual( 359, Geo.Wrap360( -1 ) );
      Assert.AreEqual( 0, Geo.Wrap360( 3 * 360 ) );
      Assert.AreEqual( 35, Geo.Wrap360( 3 * 360 + 35 ) );

      // Avi Double version
      Assert.AreEqual( 360.0, Geo.Wrap360avi( 0.0 ) );
      Assert.AreEqual( 360.0, Geo.Wrap360avi( 360.0 ) ); // pass through
      Assert.AreEqual( 0.1, Geo.Wrap360avi( 0.1 ) ); // pass through
      Assert.AreEqual( 359.9, Geo.Wrap360avi( -0.1 ), epsTest );
      Assert.AreEqual( 360.0, Geo.Wrap360avi( 2.0 * 360.0 ), epsTest );
      Assert.AreEqual( 360.0, Geo.Wrap360avi( 3.0 * 360.0 ), epsTest );
      Assert.AreEqual( 180.0, Geo.Wrap360avi( 180.0 ) ); // pass through
      Assert.AreEqual( 360.0, Geo.Wrap360avi( 2 * 180.0 ), epsTest );
      Assert.AreEqual( 180.0, Geo.Wrap360avi( 3 * 180.0 ), epsTest );

      // Avi Int version
      Assert.AreEqual( 360, Geo.Wrap360avi( 0 ) );
      Assert.AreEqual( 1, Geo.Wrap360avi( 1 ) );
      Assert.AreEqual( 359, Geo.Wrap360avi( -1 ) );
      Assert.AreEqual( 360, Geo.Wrap360avi( -360 ) );
      Assert.AreEqual( 360, Geo.Wrap360avi( 360 ) ); // pass through
      Assert.AreEqual( 360, Geo.Wrap360avi( 2 * 360 ) );
      Assert.AreEqual( 360, Geo.Wrap360avi( 3 * 360 ) );
      Assert.AreEqual( 180, Geo.Wrap360avi( 180 ) ); // pass through
      Assert.AreEqual( 360, Geo.Wrap360avi( 2 * 180 ) );
      Assert.AreEqual( 180, Geo.Wrap360avi( 3 * 180 ) );
    }

    [TestMethod]
    public void TrackTests( )
    {
      // Towards flag
      Assert.AreEqual( true, Geo.Towards( 0, 0 ) );
      Assert.AreEqual( false, Geo.Towards( 180, 0 ) );
      Assert.AreEqual( true, Geo.Towards( 0, 90 ) );
      Assert.AreEqual( false, Geo.Towards( 0, 90 + 0.00001 ) );
      Assert.AreEqual( true, Geo.Towards( 0, -90 ) );
      Assert.AreEqual( false, Geo.Towards( 0, -90 - 0.00001 ) );

      Assert.AreEqual( true, Geo.Towards( 180, 180 ) );
      Assert.AreEqual( false, Geo.Towards( 0, 180 ) );

      Assert.AreEqual( true, Geo.Towards( 45, 45 ) );
      Assert.AreEqual( true, Geo.Towards( 45, 45 + 90 ) );
      Assert.AreEqual( false, Geo.Towards( 45, 45 + 90 + 0.00001 ) );
      Assert.AreEqual( true, Geo.Towards( 45, 45 - 90 ) );
      Assert.AreEqual( false, Geo.Towards( 45, 45 - 90 - 0.00001 ) );

      Assert.AreEqual( true, Geo.Towards( -145, -145 ) );
      Assert.AreEqual( true, Geo.Towards( -145, -145 + 90 ) );
      Assert.AreEqual( false, Geo.Towards( -145, -145 + 90 + 0.00001 ) );
      Assert.AreEqual( true, Geo.Towards( -145, -145 - 90 ) );
      Assert.AreEqual( false, Geo.Towards( -145, -145 - 90 - 0.00001 ) );

      // Direction
      Assert.AreEqual( 0.0, Geo.DirectionOf( 0, 0 ) );
      Assert.AreEqual( -45.0, Geo.DirectionOf( 0, 45.0 ) );
      Assert.AreEqual( 45.0, Geo.DirectionOf( 0, -45.0 ) );
      Assert.AreEqual( -95.0, Geo.DirectionOf( 0, 95.0 ) );
      Assert.AreEqual( 95.0, Geo.DirectionOf( 0, -95.0 ) );
      Assert.AreEqual( -179.0, Geo.DirectionOf( 0, 179.0 ) );
      Assert.AreEqual( 179.0, Geo.DirectionOf( 0, -179.0 ) );

      Assert.AreEqual( -45.0, Geo.DirectionOf( 3 * 360 + 17, 2 * 360 + 17 + 45.0 ) );
      Assert.AreEqual( 45.0, Geo.DirectionOf( 3 * 360 + 17, 2 * 360 + 17 - 45.0 ) );

      Assert.AreEqual( 180.0, Geo.DirectionOf( 0, 180.0 + eps ), epsTest );
      Assert.AreEqual( -180.0, Geo.DirectionOf( 0, -180.0 - eps ), epsTest );

      Assert.AreEqual( 0.0, Geo.DirectionOf( 180.0, 180.0 ) );
      Assert.AreEqual( -45.0, Geo.DirectionOf( 180.0, 180.0 + 45.0 ) );
      Assert.AreEqual( 45.0, Geo.DirectionOf( 180.0, 180.0 - 45.0 ) );

      Assert.AreEqual( 180.0, Geo.DirectionOf( 180.0, 180.0 + 180.0 + eps ), epsTest );
      Assert.AreEqual( -180.0, Geo.DirectionOf( 180.0, 180.0 - 180.0 - eps ), epsTest );

    }

    [TestMethod]
    public void XTrackTests( )
    {
      // ********************************
      // ***** Cross Track Distance *****
      // ********************************

      var pCurrent = new LatLon( 53.2611, -0.7972 );
      var p1 = new LatLon( 53.3206, -1.7297 );
      var p2 = new LatLon( 53.1887, 0.1334 );
      var d = pCurrent.CrossTrackDistanceTo( p1, p2 );  // -307.5 m
      Assert.AreEqual( -307.894, d, 0.001 );

      LatLon Start = new LatLon( Dms.ParseDMS( "N 21° 18' 24\"" ), Dms.ParseDMS( "W 157° 56' 44\"" ), 7 );
      LatLon Target = new LatLon( Dms.ParseDMS( "N 21° 18' 25\"" ), Dms.ParseDMS( "W 157° 56' 23\"" ) );
      var xtk = Target.CrossTrackDistanceTo(
        Start.DestinationPoint( -1000, 89 ),
        Start.DestinationPoint( 5000, 89 ) );


      Start = new LatLon( 21.306800879538, -157.945554703474, 7 );
      Target = new LatLon( 21.306847018665916, -157.93977428471166 );
      xtk = Target.CrossTrackDistanceTo(
        Start.DestinationPoint( -1000, 89.9940032958984 ),
        Start.DestinationPoint( 5000, 89.9940032958984 ) );

    }


    [TestMethod]
    public void AlongTrackTests( )
    {
      // ********************************
      // ***** Along Track Distance *****
      // ********************************

      // Setup
      double trackAngle = 0.0;
      var Start = new LatLon( 21.306800879538, -157.945554703474, 7 );
      var Target = Start.DestinationPoint( 20, trackAngle, ConvConsts.EarthRadiusNm ); // Target is North out 20nm

      // on Start/End tests
      var p1 = Start;
      double xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 30nm from Start
      Assert.AreEqual( 0, xtk, 0.00001 );

      p1 = Target;
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 30nm from Start
      Assert.AreEqual( 20, xtk, 0.00001 );

      // on track tests
      p1 = Start.DestinationPoint( 10, trackAngle, ConvConsts.EarthRadiusNm ); // Current is North out 10nm
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 10nm from Start
      Assert.AreEqual( 10, xtk, 0.00001 );

      p1 = Start.DestinationPoint( 30, trackAngle, ConvConsts.EarthRadiusNm ); // Current is North out 30nm
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 30nm from Start
      Assert.AreEqual( 30, xtk, 0.00001 );

      p1 = Start.DestinationPoint( 10, trackAngle + 180, ConvConsts.EarthRadiusNm ); // Current is South out 10nm
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // -10nm from Start
      Assert.AreEqual( -10, xtk, 0.00001 );

      p1 = Start.DestinationPoint( 30, trackAngle + 180, ConvConsts.EarthRadiusNm ); // Current is South out 30nm
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 30nm from Start
      Assert.AreEqual( -30, xtk, 0.00001 );


      // no longer on track tests
      p1 = Start.DestinationPoint( 20.0 / 2.0 * Math.Sqrt( 2 ), trackAngle - 45.0, ConvConsts.EarthRadiusNm ); // Current 45° West 0.707 * Dist
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 10nm from Start
      Assert.AreEqual( 10, xtk, 0.0001 ); // calc error is about 0.00003

      p1 = Start.DestinationPoint( 20.0 / 2.0 * Math.Sqrt( 2 ), trackAngle + 45.0, ConvConsts.EarthRadiusNm ); // Current 45° East 0.707 * Dist
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 10nm from Start
      Assert.AreEqual( 10, xtk, 0.0001 ); // calc error is about 0.00003

      p1 = Start.DestinationPoint( 20.0 / 2.0 * Math.Sqrt( 2 ), trackAngle - 90.0, ConvConsts.EarthRadiusNm ); // Current 90° West 0.707 * Dist
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 0nm from Start
      Assert.AreEqual( 0, xtk, 0.0001 );

      p1 = Start.DestinationPoint( 20.0 / 2.0 * Math.Sqrt( 2 ), trackAngle + 90.0, ConvConsts.EarthRadiusNm ); // Current 90° East 0.707 * Dist
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 0nm from Start
      Assert.AreEqual( 0, xtk, 0.0001 );


      // ** 2nd Setup with initial track of 33° and inversed sign on LatLon
      trackAngle = 33.0;
      Start = new LatLon( -21.306800879538, 157.945554703474, 7 );
      Target = Start.DestinationPoint( 20, trackAngle, ConvConsts.EarthRadiusNm ); // Target is 33° out 20nm

      // on Start/End tests
      p1 = Start;
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 30nm from Start
      Assert.AreEqual( 0, xtk, 0.00001 );

      p1 = Target;
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 30nm from Start
      Assert.AreEqual( 20, xtk, 0.00001 );

      // on track tests
      p1 = Start.DestinationPoint( 10, trackAngle, ConvConsts.EarthRadiusNm ); // Current is North out 10nm
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 10nm from Start
      Assert.AreEqual( 10, xtk, 0.00001 );

      p1 = Start.DestinationPoint( 30, trackAngle, ConvConsts.EarthRadiusNm ); // Current is North out 30nm
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 30nm from Start
      Assert.AreEqual( 30, xtk, 0.00001 );

      p1 = Start.DestinationPoint( 10, trackAngle + 180, ConvConsts.EarthRadiusNm ); // Current is South out 10nm
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // -10nm from Start
      Assert.AreEqual( -10, xtk, 0.00001 );

      p1 = Start.DestinationPoint( 30, trackAngle + 180, ConvConsts.EarthRadiusNm ); // Current is South out 30nm
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 30nm from Start
      Assert.AreEqual( -30, xtk, 0.00001 );


      // no longer on track tests
      p1 = Start.DestinationPoint( 20.0 / 2.0 * Math.Sqrt( 2 ), trackAngle - 45.0, ConvConsts.EarthRadiusNm ); // Current 45° West 0.707 * Dist
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 10nm from Start
      Assert.AreEqual( 10, xtk, 0.0001 ); // calc error is about 0.00003

      p1 = Start.DestinationPoint( 20.0 / 2.0 * Math.Sqrt( 2 ), trackAngle + 45.0, ConvConsts.EarthRadiusNm ); // Current 45° East 0.707 * Dist
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 10nm from Start
      Assert.AreEqual( 10, xtk, 0.0001 ); // calc error is about 0.00003

      p1 = Start.DestinationPoint( 20.0 / 2.0 * Math.Sqrt( 2 ), trackAngle - 90.0, ConvConsts.EarthRadiusNm ); // Current 90° West 0.707 * Dist
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 0nm from Start
      Assert.AreEqual( 0, xtk, 0.0001 );

      p1 = Start.DestinationPoint( 20.0 / 2.0 * Math.Sqrt( 2 ), trackAngle + 90.0, ConvConsts.EarthRadiusNm ); // Current 90° East 0.707 * Dist
      xtk = p1.AlongTrackDistanceTo( Start, Target, ConvConsts.EarthRadiusNm ); // 0nm from Start
      Assert.AreEqual( 0, xtk, 0.0001 );

      ;

    }



  }
}
