using System;
using System.Globalization;
using System.Security.Cryptography;

namespace CoordLib.LLShapes
{
  /// <summary>
  /// Rectangle made from LatLon Coordinate Items
  ///  THE RECTANGLE LOCATION IS LEFT/TOP  (same as Graphic Rectangles)
  ///    WIDTH  to the right (pos Longitude)
  ///    HEIGHT to downwards (neg Latitude)
  ///    
  /// Note: Math operations are performed on the numbers and not in any particular projection
  /// 
  /// Note: all methods go with X,Y (Lon,Lat) parameters
  /// 
  /// </summary>
  public struct LLRectangle
  {
    /// <summary>
    /// static cTor: init empty field
    /// </summary>
    static LLRectangle( ) => Empty = new LLRectangle( true );

    /// <summary>
    /// Represents a Rectangle structure with its properties left uninitialized.
    /// </summary>
    public static readonly LLRectangle Empty;

    // specific

    /// <summary>
    /// Get a sized LLRectangle around a center point with the dimensions given, default [m]
    ///   use earthRadius for other dimensions
    ///   this will scale along the Latitude of the center point (flattened)
    /// </summary>
    /// <param name="center">Center point</param>
    /// <param name="width">Widht in earthRadius base dimension</param>
    /// <param name="height">Height in earthRadius base dimension</param>
    /// <param name="earthRadius">EarthRadius, default [m]</param>
    public static LLRectangle GetSized( LLPoint center, double width, double height, double earthRadius = ConvConsts.EarthRadiusM )
    {
      var size = new LLSize( ConvConsts.LonDegPerDist( width, center.Lat, earthRadius ), ConvConsts.LatDegPerDist( height, earthRadius ) );
      var leftTop = center;
      // shift leftTop to make the center the Center
      double leftLon = Geo.Wrap180( center.Lon - size.WidthLon / 2.0 );
      double topLat = Geo.Wrap90( center.Lat + size.HeightLat / 2 ); // add to go upwards
      return new LLRectangle( new LLPoint( leftLon, topLat ), size );
    }


    // *** STRUCT

    private LLPoint _location;
    private LLSize _size;

    /// <summary>
    /// Get; Set: Longitude (X, horizontal part) [deg]
    /// </summary>
    public double Lon { get => _location.Lon; set => _location.Lon = value; }
    /// <summary>
    /// Get; Set: Latitude (Y, vertical part) [deg]
    /// </summary>
    public double Lat { get => _location.Lat; set => _location.Lat = value; }

    /// <summary>
    /// Get; Set: Longitudinal Distance as angle [deg]
    /// </summary>
    public double WidthLon { get => _size.WidthLon; set => _size.WidthLon = value; }
    /// <summary>
    /// Get; Set: Lateral Distance as angle [deg]
    /// </summary>
    public double HeightLat { get => _size.HeightLat; set => _size.HeightLat = value; }

    // Property Set tracker
    private bool _notEmpty; // 

    /// <summary>
    /// Returns true if coordinates are not assigned
    /// </summary>
    public bool IsEmpty => !_notEmpty;

    /// <summary>
    /// cTor: Empty (init as Empty)
    /// </summary>
    public LLRectangle( bool _ = true )
    {
      _notEmpty = false;
      _location = new LLPoint( 0, 0 );
      _size = new LLSize( 0, 0 );
    }

    /// <summary>
    /// cTor: from values
    /// </summary>
    /// <param name="lon">Longitude center</param>
    /// <param name="lat">Latitude center</param>
    /// <param name="widthLng">Horizontal size</param>
    /// <param name="heightLat">vertical size</param>
    public LLRectangle( double lon, double lat, double widthLng, double heightLat )
    {
      _location = new LLPoint( lon, lat );
      _size = new LLSize( widthLng, heightLat );
      _notEmpty = true;
    }
    /// <summary>
    /// cTor: from LLPoint and LLSize
    /// </summary>
    /// <param name="location">Center point</param>
    /// <param name="size">Area</param>
    public LLRectangle( LLPoint location, LLSize size )
    {
      _location = location;
      _size = size;
      _notEmpty = true;
    }
    /// <summary>
    /// Returns an LLRectangle from Left,Top,Right,Bottom points
    /// </summary>
    /// <param name="leftLon">Longitude of the left side</param>
    /// <param name="topLat">Latitude of the top side</param>
    /// <param name="rightLon">Longitude of the right side</param>
    /// <param name="bottomLat">Latitude of the bottom side</param>
    public static LLRectangle FromLTRB(
      double leftLon, double topLat, double rightLon, double bottomLat )
      => new LLRectangle( leftLon, topLat, rightLon - leftLon, topLat - bottomLat );

    /// <summary>
    /// Get: Left,Top Point
    /// </summary>
    public LLPoint Location => new LLPoint( _location );
    /// <summary>
    /// Get: Left,Top Point
    /// </summary>
    public LLPoint LocationTopLeft => Location;
    /// <summary>
    /// Get: Right,Bottom Point
    /// </summary>
    public LLPoint LocationRightBottom => new LLPoint( RightLon, BottomLat );
    /// <summary>
    /// Get: Center Point of this LLRectangle
    /// </summary>
    public LLPoint LocationMiddle => new LLPoint( Lon + WidthLon / 2.0, Lat - HeightLat / 2.0 );

    /// <summary>
    /// Get; Set: The size of this LLRectangle
    /// </summary>
    public LLSize Size { get => new LLSize( _size ); set { _size = value; } }

    /// <summary>
    /// Get: Left (Longitude)
    /// </summary>
    public double LeftLon => Lon;
    /// <summary>
    /// Get: Top (Latitude)
    /// </summary>
    public double TopLat => Lat;
    /// <summary>
    /// Get: Right (Lon + WidthDeg)
    /// </summary>
    public double RightLon => Lon + WidthLon;
    /// <summary>
    /// Get: Bottom (Lat - Height)
    /// </summary>
    public double BottomLat => Lat - HeightLat;

    /// <summary>
    /// Equality: Tests whether two Rectangle structures have equal location and size.
    /// </summary>
    public static bool operator ==( LLRectangle left, LLRectangle right )
    {
      return left.Lon == right.Lon && left.Lat == right.Lat && left.WidthLon == right.WidthLon &&
             left.HeightLat == right.HeightLat;
    }
    /// <summary>
    /// Inequality: Tests whether two Rectangle structures differ in location or size.
    /// </summary>
    public static bool operator !=( LLRectangle left, LLRectangle right ) => !(left == right);

    /// <summary>
    /// Adjusts the location of this rectangle by the specified amount.
    /// </summary>
    public void Offset( double lonX, double latY ) => _location.Offset( lonX, latY );
    /// <summary>
    /// Adjusts the location of this rectangle by the specified amount.
    /// </summary>
    public void Offset( LLSize distance ) => _location.Offset( distance );
    /// <summary>
    /// Adjusts the location of this rectangle by the specified amount.
    /// </summary>
    public void Offset( LLPoint pos ) => Offset( pos.Lon, pos.Lat );

    /// <summary>
    /// Determines if the specified point is contained within this Rectangle structure.
    /// </summary>
    /// <param name="lon">Lon or X of point</param>
    /// <param name="lat">Lat or Y of point</param>
    public bool Contains( double lon, double lat )
    {
      // calculate at our 0/0 coordinate to avoid +- cases
      var dx = -Lon; var dy = this.HeightLat - Lat;
      // now x is 0 and y is ourHeight going down to 0

      // shift inputs and wrap if needed
      var lx = Geo.Wrap180( lon + dx ); var ly = Geo.Wrap90( lat + dy );

      // check LonX, must be within 0..ourWidth inclusive
      if (lx < 0) return false;
      if (lx > _size.WidthLon) return false;
      // check LatY, must be within 0..ourHeight inclusive
      if (ly < 0) return false;
      if (ly > _size.HeightLat) return false;

      return true;
    }
    /// <summary>
    /// Determines if the specified point is contained within this Rectangle structure.
    /// </summary>
    public bool Contains( LLPoint pt ) => Contains( pt.Lon, pt.Lat );

    /// <summary>
    /// Determines if the rectangular region represented by rect is entirely contained within this Rectangle structure.
    /// </summary>
    public bool Contains( LLRectangle rect )
    {
      // size exceeds our size, cannot be contained
      if (rect.WidthLon > this.WidthLon) return false;
      if (rect.HeightLat > this.HeightLat) return false;

      if (!this.Contains( rect.LocationTopLeft )) return false; // Top and/or Left is outside, cannot be contained
      if (!this.Contains( rect.LocationRightBottom )) return false; // Bottom and/or Right is outside, cannot be contained
      return true;
    }

    /// <summary>
    /// Inflate our size by the amount given in both directions, maintaining the original Center
    /// i.e. a size of 50 increased by 20 will result in a size of 20+50+20 = 90
    /// </summary>
    /// <param name="lonX">Width Increase</param>
    /// <param name="latY">Height Increase</param>
    public void Inflate( double lonX, double latY )
    {
      Lon = Lon - lonX;
      Lat = Lat - latY;
      WidthLon = WidthLon + 2.0 * lonX;
      HeightLat = HeightLat + 2.0 * latY;
    }
    /// <summary>
    /// Inflate our size by the amount given in both directions, maintaining the original Center
    /// i.e. a size of 50 increased by 20 will result in a size of 20+50+20 = 90
    /// </summary>
    /// <param name="size">Increase Size</param>
    public void Inflate( LLSize size ) => Inflate( size.HeightLat, size.WidthLon );
    /// <summary>
    /// Return an inflated LLRectangle based on the given rectangle and the amount given in both directions, maintaining the original Center
    /// i.e. a size of 50 increased by 20 will result in a size of 20+50+20 = 90
    /// </summary>
    ///<param name="rect">The source Rectangle</param>
    /// <param name="lonX">Width Increase</param>
    /// <param name="latY">Height Increase</param>
    public static LLRectangle Inflate( LLRectangle rect, double lonX, double latY )
    {
      LLRectangle ret = rect;
      ret.Inflate( lonX, latY );
      return ret;
    }

    /// <summary>
    /// Returns a third LLRectangle that represents the intersection
    ///     of two other LLRectangles. 
    ///     If there is no intersection, an empty LLRectangle is returned.
    /// </summary>
    /// <param name="rect1">Rectangle 1</param>
    /// <param name="rect2">Rectangle 2</param>
    /// <returns>An LLRectangle</returns>
    public static LLRectangle Intersect( LLRectangle rect1, LLRectangle rect2 )
    {
      // calc defaults
      double dx = 0;
      double leftLon = Math.Max( rect1.Lon, rect2.Lon );
      double rightLon = Math.Min( rect1.Lon + rect1.WidthLon, rect2.Lon + rect2.WidthLon );

      double topLlat = Math.Min( rect1.Lat, rect2.Lat );
      double bottomLat = Math.Max( rect1.Lat - rect1.HeightLat, rect2.Lat - rect2.HeightLat );
      // check for crossing 0 or 180 - then shift all using the positive Lon 
      if ((rect1.Lon >= 0) && (rect2.Lon < 0)) {
        dx = -rect1.Lon;
        leftLon = Math.Max( Geo.Wrap180( rect1.Lon + dx ), Geo.Wrap180( rect2.Lon + dx ) );
        rightLon = Math.Min( Geo.Wrap180( rect1.Lon + dx ) + rect1.WidthLon, Geo.Wrap180( rect2.Lon + dx ) + rect2.WidthLon );
      }
      else if ((rect2.Lon >= 0) && (rect1.Lon < 0)) {
        dx = -rect2.Lon;
        leftLon = Math.Max( Geo.Wrap180( rect1.Lon + dx ), Geo.Wrap180( rect2.Lon + dx ) );
        rightLon = Math.Min( Geo.Wrap180( rect1.Lon + dx ) + rect1.WidthLon, Geo.Wrap180( rect2.Lon + dx ) + rect2.WidthLon );
      }
      // translate back to rect space
      leftLon -= dx; rightLon -= dx;

      if (rightLon >= leftLon && topLlat >= bottomLat) {
        return new LLRectangle( leftLon, topLlat, rightLon - leftLon, topLlat - bottomLat );
      }

      return Empty;
    }
    /// <summary>
    /// Intersect this LLRectangle with another LLRectangle
    /// </summary>
    /// <param name="otherRect">LLRectangle to intersect with</param>
    public void Intersect( LLRectangle otherRect )
    {
      LLRectangle ef = Intersect( this, otherRect );
      _location = ef.Location;
      _size = ef.Size;
    }

    /// <summary>
    /// True if this Rectangle Intersects with another LLRectangle
    /// i.e. has a common area
    /// </summary>§
    /// <param name="otherRect"></param>
    /// <returns>True on intersection</returns>
    public bool IntersectsWith( LLRectangle otherRect )
    {
      LLRectangle ef = Intersect( this, otherRect );
      return !ef.IsEmpty;
    }

    /// <summary>
    /// An LLRectangle that bounds the union of the two LLRectangles
    /// </summary>
    /// <param name="rect1">Rectangle 1</param>
    /// <param name="rect2">Rectangle 2</param>
    /// <returns>An LLRectangle</returns>
    public static LLRectangle Union( LLRectangle rect1, LLRectangle rect2 )
    {
      // calc defaults
      double dx = 0;
      double leftLon = Math.Min( rect1.Lon, rect2.Lon );
      double rightLon = Math.Max( rect1.Lon + rect1.WidthLon, rect2.Lon + rect2.WidthLon );

      double topLlat = Math.Max( rect1.Lat, rect2.Lat );
      double bottomLat = Math.Min( rect1.Lat - rect1.HeightLat, rect2.Lat - rect2.HeightLat );
      // check for crossing 0 or 180 - then shift all using the positive Lon 
      if ((rect1.Lon >= 0) && (rect2.Lon < 0)) {
        dx = -rect1.Lon;
        leftLon = Math.Min( Geo.Wrap180( rect1.Lon + dx ), Geo.Wrap180( rect2.Lon + dx ) );
        rightLon = Math.Max( Geo.Wrap180( rect1.Lon + dx ) + rect1.WidthLon, Geo.Wrap180( rect2.Lon + dx ) + rect2.WidthLon );
      }
      else if ((rect2.Lon >= 0) && (rect1.Lon < 0)) {
        dx = -rect2.Lon;
        leftLon = Math.Min( Geo.Wrap180( rect1.Lon + dx ), Geo.Wrap180( rect2.Lon + dx ) );
        rightLon = Math.Max( Geo.Wrap180( rect1.Lon + dx ) + rect1.WidthLon, Geo.Wrap180( rect2.Lon + dx ) + rect2.WidthLon );
      }
      // translate back to rect space
      leftLon -= dx; rightLon -= dx;

      return new LLRectangle( leftLon, topLlat, rightLon - leftLon, topLlat - bottomLat );
    }
    /// <summary>
    /// Union this LLRectangle with another LLRectangle
    /// </summary>
    /// <param name="otherRect">LLRectangle to union with</param>
    public void Union( LLRectangle otherRect )
    {
      LLRectangle ef = Union( this, otherRect );
      _location = ef.Location;
      _size = ef.Size;
    }


    /// <summary>
    /// Tests whether obj is a Rectangle structure with the same location and size of this Rectangle structure.
    /// </summary>
    public override bool Equals( object obj )
    {
      if (!(obj is LLRectangle)) return false;

      var ef = (LLRectangle)obj;
      return ef.Lon == Lon && ef.Lat == Lat && ef.WidthLon == WidthLon &&
             ef.HeightLat == HeightLat;
    }

    /// <summary>
    /// Returns the hash code for this Rectangle structure. For information about the use of hash codes, see GetHashCode() .
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode( )
    {
      if (IsEmpty) return 0;

      return Lon.GetHashCode( ) ^ Lat.GetHashCode( ) ^ WidthLon.GetHashCode( ) ^ HeightLat.GetHashCode( );
    }

    /// <summary>
    /// Converts the attributes of this Rectangle to a human-readable string.
    /// </summary>
    public override string ToString( )
    {
      return "{Lon=" + Lon.ToString( CultureInfo.InvariantCulture ) +
        ",Lat=" + Lat.ToString( CultureInfo.InvariantCulture )
      + ",WidthLon=" + WidthLon.ToString( CultureInfo.InvariantCulture )
      + ",HeightLat=" + HeightLat.ToString( CultureInfo.InvariantCulture )
      + "}";
    }

    // from here down need to test each function to be sure they work good
    // |
    // .

    #region -- unsure --


    // ok ???

    #endregion
    // .
    // |
    // unsure ends here
  }
}
