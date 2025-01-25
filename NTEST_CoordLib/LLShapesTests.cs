using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using CoordLib;
using CoordLib.LLShapes;
using CoordLib.Extensions;
using static CoordLib.ConvConsts;

namespace NTEST_CoordLib
{
  [TestClass]
  public class LLShapesTests
  {
    [TestMethod]
    public void TestLLPoint( )
    {
      // simple Point and Ops

      LLPoint point = LLPoint.Empty;
      LLPoint point2 = new LLPoint( point );

      Assert.AreEqual( true, point.IsEmpty );
      Assert.AreEqual( true, point.AsLatLon( ).IsEmpty );

      point = new LLPoint( 0, 0 );
      Assert.AreEqual( 0.0, point.Lon );
      Assert.AreEqual( 0.0, point.Lat );

      point2 = new LLPoint( point );
      bool outcome = (point == point2);
      Assert.AreEqual( true, outcome );

      point = new LLPoint( 180, 90 );
      Assert.AreEqual( 180.0, point.Lon );
      Assert.AreEqual( 90.0, point.Lat );

      // must wrap around the globe
      point = new LLPoint( 181, 91 );
      Assert.AreEqual( -179.0, point.Lon );
      Assert.AreEqual( 89.0, point.Lat );

      point = new LLPoint( -180, -90 );
      Assert.AreEqual( -180.0, point.Lon );
      Assert.AreEqual( -90.0, point.Lat );

      point = new LLPoint( -181, -91 );
      Assert.AreEqual( 179.0, point.Lon );
      Assert.AreEqual( -89.0, point.Lat );

      // copy constructor
      point = new LLPoint( 16, 77 );
      point2 = new LLPoint( point );
      Assert.AreEqual( point.Lon, point2.Lon );
      Assert.AreEqual( point.Lat, point2.Lat );

      // LatLon constructor
      point2 = new LLPoint( (LatLon)point );
      Assert.AreEqual( point.Lon, point2.Lon );
      Assert.AreEqual( point.Lat, point2.Lat );

      // move with LLSize
      point = new LLPoint( 0, 0 );
      LLSize size = new LLSize( 20, 30 );
      point2 = point + size;
      Assert.AreEqual( 20, point2.Lon );
      Assert.AreEqual( 30, point2.Lat );

      // move with Offset
      point = new LLPoint( 0, 0 );
      point.Offset( 20, 30 );
      Assert.AreEqual( 20, point.Lon );
      Assert.AreEqual( 30, point.Lat );

      point = new LLPoint( 0, 0 );
      point2 = point + size;
      point.Offset( point2 );
      Assert.AreEqual( 20, point.Lon );
      Assert.AreEqual( 30, point.Lat );

      // 
    }


    [TestMethod]
    public void TestLLSize( )
    {
      // simple Size and Ops

      LLPoint point = new LLPoint( );
      LLSize size = new LLSize( );
      LLSize size2 = new LLSize( );

      size = new LLSize( 20, 30 );
      Assert.AreEqual( 20, size.WidthLon );
      Assert.AreEqual( 30, size.HeightLat );

      size2 = new LLSize( size );
      bool outcome = size == size2;
      Assert.AreEqual( true, outcome );

      size = new LLSize( 20, 30 );
      size2 = size + new LLSize( 20, 10 );
      Assert.AreEqual( 20 + 20, size2.WidthLon );
      Assert.AreEqual( 30 + 10, size2.HeightLat );

      size = new LLSize( 20, 30 );
      size2 = size - new LLSize( 20, 10 );
      Assert.AreEqual( 20 - 20, size2.WidthLon );
      Assert.AreEqual( 30 - 10, size2.HeightLat );

      point = size.ToLLPoint( );
      Assert.AreEqual( size.WidthLon, point.Lon );
      Assert.AreEqual( size.HeightLat, point.Lat );


    }


    [TestMethod]
    public void TestLLRectagle( )
    {
      // simple Rectangle and Ops
      LLPoint center = new LLPoint( 0, 0 );
      LLPoint center2 = new LLPoint( 0, 0 );
      LLSize size = new LLSize( 10, 20 );
      LLRectangle rect = new LLRectangle( center, size );
      LLRectangle rect2 = new LLRectangle( center, size );

      Assert.AreEqual( 0, rect.LeftLon );
      Assert.AreEqual( 0, rect.TopLat );
      Assert.AreEqual( 10, rect.WidthLon );
      Assert.AreEqual( 20, rect.HeightLat );

      Assert.AreEqual( new LLPoint( 0, 0 ), rect.LocationTopLeft );
      Assert.AreEqual( new LLPoint( 10, -20 ), rect.LocationRightBottom );

      rect = new LLRectangle( center, size );
      rect2 = new LLRectangle( center, new LLSize( size.WidthLon / 2, size.HeightLat / 2 ) );
      Assert.AreEqual( true, rect.Contains( rect2 ) );

      // move rect 2 but still inside rect 
      center2 = center; center2.Offset( 0.00001, -0.00001 ); // top is below center.Top i.e. inside
      rect2 = new LLRectangle( center2, new LLSize( size.WidthLon / 2, size.HeightLat / 2 ) );
      Assert.AreEqual( true, rect.Contains( rect2 ) );

      // move rect 2 outside rect 
      center2 = center; center2.Offset( -0.00001, 0 ); // left is left of center.Left i.e. outside
      rect2 = new LLRectangle( center2, new LLSize( size.WidthLon / 2, size.HeightLat / 2 ) );
      Assert.AreEqual( false, rect.Contains( rect2 ) );

      center2 = center; center2.Offset( 0, +0.00001 ); // top is above center.Top i.e. outside
      rect2 = new LLRectangle( center2, new LLSize( size.WidthLon / 2, size.HeightLat / 2 ) );
      Assert.AreEqual( false, rect.Contains( rect2 ) );

      // rect2 is larger now
      rect = new LLRectangle( center, size );
      rect2 = new LLRectangle( center, new LLSize( size.WidthLon + 0.0001, size.HeightLat / 2 ) );
      Assert.AreEqual( false, rect.Contains( rect2 ) );

      rect = new LLRectangle( center, size );
      rect2 = new LLRectangle( center, new LLSize( size.WidthLon / 2, size.HeightLat + 0.0001 ) );
      Assert.AreEqual( false, rect.Contains( rect2 ) );

    }

    [TestMethod]
    public void TestLLRectagle_Intersect( )
    {
      // Distance/Degree Rectangle Intersection
      LLPoint center = new LLPoint( 0, 0 );
      LLRectangle rect1 = new LLRectangle( );
      LLRectangle rect2 = new LLRectangle( );
      LLRectangle rect3 = new LLRectangle( );

      // equal ones
      rect1 = new LLRectangle( 1, 3, 3, 2 );
      rect2 = new LLRectangle( 1, 3, 3, 2 );
      Assert.AreEqual( true, rect1.IntersectsWith( rect2 ) );
      Assert.AreEqual( true, rect2.IntersectsWith( rect1 ) );
      rect3 = LLRectangle.Intersect( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 1, 3, 3, 2 ), rect3 );
      rect3 = LLRectangle.Intersect( rect2, rect1 );
      Assert.AreEqual( new LLRectangle( 1, 3, 3, 2 ), rect3 );

      // non intersecting ones
      rect1 = new LLRectangle( 10, 30, 3, 2 );
      rect2 = new LLRectangle( 1, 3, 3, 2 );
      Assert.AreEqual( false, rect1.IntersectsWith( rect2 ) );
      Assert.AreEqual( false, rect2.IntersectsWith( rect1 ) );
      rect3 = LLRectangle.Intersect( rect1, rect2 );
      Assert.AreEqual( LLRectangle.Empty, rect3 );
      rect3 = LLRectangle.Intersect( rect2, rect1 );
      Assert.AreEqual( LLRectangle.Empty, rect3 );


      // base 3x2 at various positions resulting in a 1x1 outcome at various positions
      rect1 = new LLRectangle( 1, 3, 3, 2 );
      rect2 = new LLRectangle( 3, 4, 3, 2 );
      Assert.AreEqual( true, rect1.IntersectsWith( rect2 ) );
      Assert.AreEqual( true, rect2.IntersectsWith( rect1 ) );
      rect3 = LLRectangle.Intersect( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 3, 3, 1, 1 ), rect3 );
      rect3 = LLRectangle.Intersect( rect2, rect1 );
      Assert.AreEqual( new LLRectangle( 3, 3, 1, 1 ), rect3 );

      // ** translate around

      // shift left by 2
      rect1 = new LLRectangle( 1 - 2, 3, 3, 2 );
      rect2 = new LLRectangle( 3 - 2, 4, 3, 2 );
      Assert.AreEqual( true, rect1.IntersectsWith( rect2 ) );
      rect3 = LLRectangle.Intersect( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 3 - 2, 3, 1, 1 ), rect3 );

      // shift right by 2
      rect1 = new LLRectangle( 1 + 2, 3, 3, 2 );
      rect2 = new LLRectangle( 3 + 2, 4, 3, 2 );
      Assert.AreEqual( true, rect1.IntersectsWith( rect2 ) );
      rect3 = LLRectangle.Intersect( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 3 + 2, 3, 1, 1 ), rect3 );

      // shift up by 2
      rect1 = new LLRectangle( 1, 3 + 2, 3, 2 );
      rect2 = new LLRectangle( 3, 4 + 2, 3, 2 );
      Assert.AreEqual( true, rect1.IntersectsWith( rect2 ) );
      rect3 = LLRectangle.Intersect( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 3, 3 + 2, 1, 1 ), rect3 );

      // shift down by 2
      rect1 = new LLRectangle( 1, 3 - 2, 3, 2 );
      rect2 = new LLRectangle( 3, 4 - 2, 3, 2 );
      Assert.AreEqual( true, rect1.IntersectsWith( rect2 ) );
      rect3 = LLRectangle.Intersect( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 3, 3 - 2, 1, 1 ), rect3 );

      // shift left,down by 2
      rect1 = new LLRectangle( 1 - 2, 3 - 2, 3, 2 );
      rect2 = new LLRectangle( 3 - 2, 4 - 2, 3, 2 );
      Assert.AreEqual( true, rect1.IntersectsWith( rect2 ) );
      rect3 = LLRectangle.Intersect( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 3 - 2, 3 - 2, 1, 1 ), rect3 );

      //** Other side of the globe at Lon 180
      rect1 = new LLRectangle( 1 + 180, 3, 3, 2 );
      rect2 = new LLRectangle( 3 + 180, 4, 3, 2 );
      Assert.AreEqual( true, rect1.IntersectsWith( rect2 ) );
      rect3 = LLRectangle.Intersect( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( Geo.Wrap180( 3 + 180 ), 3, 1, 1 ), rect3 );
      // shift left by 2
      rect1 = new LLRectangle( 1 + 180 - 2, 3, 3, 2 );
      rect2 = new LLRectangle( 3 + 180 - 2, 4, 3, 2 );
      Assert.AreEqual( true, rect1.IntersectsWith( rect2 ) );
      rect3 = LLRectangle.Intersect( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( Geo.Wrap180( 3 + 180 - 2 ), 3, 1, 1 ), rect3 );

      // ** apply intersection
      rect1 = new LLRectangle( 1, 3, 3, 2 );
      rect2 = new LLRectangle( 3, 4, 3, 2 );
      Assert.AreEqual( true, rect1.IntersectsWith( rect2 ) );
      rect1.Intersect( rect2 );
      Assert.AreEqual( new LLRectangle( 3, 3, 1, 1 ), rect1 );

    }

    [TestMethod]
    public void TestLLRectagle_Union( )
    {
      // Distance/Degree Rectangle Union
      LLPoint center = new LLPoint( 0, 0 );
      LLRectangle rect1 = new LLRectangle( );
      LLRectangle rect2 = new LLRectangle( );
      LLRectangle rect3 = new LLRectangle( );

      // equal ones
      rect1 = new LLRectangle( 1, 3, 3, 2 );
      rect2 = new LLRectangle( 1, 3, 3, 2 );
      rect3 = LLRectangle.Union( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 1, 3, 3, 2 ), rect3 );
      rect3 = LLRectangle.Union( rect2, rect1 );
      Assert.AreEqual( new LLRectangle( 1, 3, 3, 2 ), rect3 );

      // base 3x2 at various positions resulting in a 1x1 outcome at various positions
      rect1 = new LLRectangle( 1, 3, 3, 2 );
      rect2 = new LLRectangle( 3, 4, 3, 2 );
      rect3 = LLRectangle.Union( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 1, 4, 5, 3 ), rect3 );
      rect3 = LLRectangle.Union( rect2, rect1 );
      Assert.AreEqual( new LLRectangle( 1, 4, 5, 3 ), rect3 );

      // ** translate around

      // shift left by 2
      rect1 = new LLRectangle( 1 - 2, 3, 3, 2 );
      rect2 = new LLRectangle( 3 - 2, 4, 3, 2 );
      rect3 = LLRectangle.Union( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 1 - 2, 4, 5, 3 ), rect3 );

      // shift right by 2
      rect1 = new LLRectangle( 1 + 2, 3, 3, 2 );
      rect2 = new LLRectangle( 3 + 2, 4, 3, 2 );
      rect3 = LLRectangle.Union( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 1 + 2, 4, 5, 3 ), rect3 );

      // shift up by 2
      rect1 = new LLRectangle( 1, 3 + 2, 3, 2 );
      rect2 = new LLRectangle( 3, 4 + 2, 3, 2 );
      rect3 = LLRectangle.Union( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 1, 4 + 2, 5, 3 ), rect3 );

      // shift down by 2
      rect1 = new LLRectangle( 1, 3 - 2, 3, 2 );
      rect2 = new LLRectangle( 3, 4 - 2, 3, 2 );
      rect3 = LLRectangle.Union( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( 1, 4 - 2, 5, 3 ), rect3 );

      //** Other side of the globe at Lon 180
      rect1 = new LLRectangle( 1 + 180, 3, 3, 2 );
      rect2 = new LLRectangle( 3 + 180, 4, 3, 2 );
      rect3 = LLRectangle.Union( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( Geo.Wrap180( 1 + 180 ), 4, 5, 3 ), rect3 );
      // shift left by 2
      rect1 = new LLRectangle( 1 + 180 - 2, 3, 3, 2 );
      rect2 = new LLRectangle( 3 + 180 - 2, 4, 3, 2 );
      rect3 = LLRectangle.Union( rect1, rect2 );
      Assert.AreEqual( new LLRectangle( Geo.Wrap180( 1 + 180 - 2 ), 4, 5, 3 ), rect3 );

      // ** apply Union
      rect1 = new LLRectangle( 1, 3, 3, 2 );
      rect2 = new LLRectangle( 3, 4, 3, 2 );
      rect1.Union( rect2 );
      Assert.AreEqual( new LLRectangle( 1, 4, 5, 3 ), rect1 );


    }

    [TestMethod]
    public void TestLLRectagle_DistDeg( )
    {
      // Distance/Degree Rectangle and Ops
      LLPoint center = new LLPoint( 0, 0 );
      LLRectangle rect = new LLRectangle( );

      double degLon = ConvConsts.LonDegPerDist( 100, center.Lat ); // degree per 100 m
      double degLat = ConvConsts.LatDegPerDist( 100 ); // degree per 100 m

      rect = LLRectangle.GetSized( center, 200, 200 ); // using meter
      Assert.AreEqual( center.Lon - degLon, rect.LeftLon, 0.000000001 ); // left of center 
      Assert.AreEqual( center.Lat + degLat, rect.TopLat, 0.000000001 );   // above center
      Assert.AreEqual( degLon * 2, rect.WidthLon, 0.000000001 );
      Assert.AreEqual( degLat * 2, rect.HeightLat, 0.000000001 );

      // move center to the Npole at 180 deg Lon (which does not really exist...)
      center = new LLPoint( 180, 89 );
      degLon = ConvConsts.LonDegPerDist( 100, center.Lat ); // degree per 100 m
      degLat = ConvConsts.LatDegPerDist( 100 ); // degree per 100 m

      rect = LLRectangle.GetSized( center, 200, 200 ); // using meter
      Assert.AreEqual( center.Lon - degLon, rect.LeftLon, 0.000000001 ); // left of center 
      Assert.AreEqual( center.Lat + degLat, rect.TopLat, 0.000000001 );   // above center
      Assert.AreEqual( degLon * 2, rect.WidthLon, 0.000000001 );
      Assert.AreEqual( degLat * 2, rect.HeightLat, 0.000000001 );

      // use NM as base unit
      center = new LLPoint( 0, 0 );
      degLon = ConvConsts.LonDegPerDist( 100, center.Lat, ConvConsts.EarthRadiusNm ); // degree per 100 nm
      degLat = ConvConsts.LatDegPerDist( 100, ConvConsts.EarthRadiusNm ); // degree per 100 nm

      rect = LLRectangle.GetSized( center, 200, 200, ConvConsts.EarthRadiusNm ); // using nm
      Assert.AreEqual( center.Lon - degLon, rect.LeftLon, 0.000000001 ); // left of center 
      Assert.AreEqual( center.Lat + degLat, rect.TopLat, 0.000000001 );   // above center
      Assert.AreEqual( degLon * 2, rect.WidthLon, 0.000000001 );
      Assert.AreEqual( degLat * 2, rect.HeightLat, 0.000000001 );


    }

  }


}

