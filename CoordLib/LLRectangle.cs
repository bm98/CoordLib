using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CoordLib
{
  /// <summary>
  /// Rectangle made from LatLon Coordinate Items
  /// Note: Math operations are performed on the numbers and not in any particular projection
  /// </summary>
  public struct LLRectangle
  {
    /// <summary>
    /// static cTor: init empty field
    /// </summary>
    static LLRectangle( )
    {
      Empty = new LLRectangle( );
    }

    /// <summary>
    /// Represents a Rectangle structure with its properties left uninitialized.
    /// </summary>
    public static readonly LLRectangle Empty;

    /// <summary>
    /// Longitude (X) Part
    /// </summary>
    public double Lon { get; set; }
    /// <summary>
    /// Latitude (Y) Part
    /// </summary>
    public double Lat { get; set; }
    /// <summary>
    /// Radial Distance as angle (deg)
    /// </summary>
    public double WidthLon { get; set; }
    /// <summary>
    /// Radial Distance as angle (deg)
    /// </summary>
    public double HeightLat { get; set; }


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
      Lat = 0; Lon = 0; WidthLon = 0; HeightLat = 0;
    }

    /// <summary>
    /// cTor: from valued
    /// </summary>
    public LLRectangle( double lat, double lon, double widthLng, double heightLat )
    {
      Lon = lon;
      Lat = lat;
      WidthLon = widthLng;
      HeightLat = heightLat;
      _notEmpty = true;
    }
    /// <summary>
    /// cTor: from point and size
    /// </summary>
    /// <param name="location"></param>
    /// <param name="size"></param>
    public LLRectangle( LLPoint location, LLSize size )
    {
      Lon = location.Lon;
      Lat = location.Lat;
      WidthLon = size.WidthLon;
      HeightLat = size.HeightLat;
      _notEmpty = true;
    }
    /// <summary>
    /// Returns an LLRectangle from Left,Top,Right,Bottom points
    /// </summary>
    public static LLRectangle FromLTRB( double leftLng, double topLat, double rightLng, double bottomLat )
      => new LLRectangle( topLat, leftLng, rightLng - leftLng, topLat - bottomLat );

    /// <summary>
    /// TopLeft Point
    /// </summary>
    public LLPoint LocationTopLeft {
      get => new LLPoint( Lat, Lon ); set { Lon = value.Lon; Lat = value.Lat; }
    }
    /// <summary>
    /// Right Bottom Point
    /// </summary>
    public LLPoint LocationRightBottom {
      get {
        var ret = new LLPoint( Lat, Lon );
        ret.Offset( HeightLat, WidthLon );
        return ret;
      }
    }
    /// <summary>
    /// Center Point
    /// </summary>
    public LLPoint LocationMiddle {
      get {
        var ret = new LLPoint( Lat, Lon );
        ret.Offset( HeightLat / 2, WidthLon / 2 );
        return ret;
      }
    }
    /// <summary>
    /// Gets or sets the size of this Rectangle.
    /// </summary>
    public LLSize Size {
      get => new LLSize( HeightLat, WidthLon );
      set { WidthLon = value.WidthLon; HeightLat = value.HeightLat; }
    }

    /// <summary>
    /// Left (Longitude)
    /// </summary>
    public double Left {
      get {
        return Lon;
      }
    }

    /// <summary>
    /// Top (Latitude)
    /// </summary>
    public double Top {
      get {
        return Lat;
      }
    }
    /// <summary>
    /// Right (Lon + WidthDeg)
    /// </summary>
    public double Right {
      get {
        return Lon + WidthLon;
      }
    }
    /// <summary>
    /// Bottom (Lat-Height)
    /// </summary>
    public double Bottom {
      get {
        return Lat - HeightLat;
      }
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
    /// Determines if the specified point is contained within this Rectangle structure.
    /// </summary>
    public bool Contains( double lat, double lon )
    {
      return (Lon <= lon) && (lon < Lon + WidthLon) && (Lat >= lat) && (lat > Lat - HeightLat);
    }
    /// <summary>
    /// Determines if the specified point is contained within this Rectangle structure.
    /// </summary>
    public bool Contains( LLPoint pt ) => Contains( pt.Lat, pt.Lon );

    /// <summary>
    /// Determines if the rectangular region represented by rect is entirely contained within this Rectangle structure.
    /// </summary>
    public bool Contains( LLRectangle rect )
    {
      return (Lon <= rect.Lon)
              && ((rect.Lon + rect.WidthLon) <= (Lon + WidthLon))
              && (Lat >= rect.Lat)
              && ((rect.Lat - rect.HeightLat) >= (Lat - HeightLat));
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
    /// Adjusts the location of this rectangle by the specified amount.
    /// </summary>
    public void Offset( LLPoint pos ) => Offset( pos.Lat, pos.Lon );
    /// <summary>
    /// Adjusts the location of this rectangle by the specified amount.
    /// </summary>
    public void Offset( double lat, double lon )
    {
      Lon += lon;
      Lat -= lat;
    }
    /// <summary>
    /// Converts the attributes of this Rectangle to a human-readable string.
    /// </summary>
    public override string ToString( )
    {
      return "{Lat=" + Lat.ToString( CultureInfo.InvariantCulture ) + ",Lng=" +
             Lon.ToString( CultureInfo.InvariantCulture ) + ",WidthLng=" +
             WidthLon.ToString( CultureInfo.InvariantCulture ) + ",HeightLat=" +
             HeightLat.ToString( CultureInfo.InvariantCulture ) + "}";
    }


    // from here down need to test each function to be sure they work good
    // |
    // .

    #region -- unsure --

    private void Inflate( double lat, double lon )
    {
      Lon -= lon;
      Lat += lat;
      WidthLon += 2d * lon;
      HeightLat += 2d * lat;
    }

    private void Inflate( LLSize size )
    {
      Inflate( size.HeightLat, size.WidthLon );
    }

    private static LLRectangle Inflate( LLRectangle rect, double lat, double lon )
    {
      LLRectangle ef = rect;
      ef.Inflate( lat, lon );
      return ef;
    }

    private void Intersect( LLRectangle rect )
    {
      LLRectangle ef = Intersect( rect, this );
      Lon = ef.Lon;
      Lat = ef.Lat;
      WidthLon = ef.WidthLon;
      HeightLat = ef.HeightLat;
    }

    // ok ???
    private static LLRectangle Intersect( LLRectangle a, LLRectangle b )
    {
      double lon = Math.Max( a.Lon, b.Lon );
      double num2 = Math.Min( a.Lon + a.WidthLon, b.Lon + b.WidthLon );

      double lat = Math.Max( a.Lat, b.Lat );
      double num4 = Math.Min( a.Lat + a.HeightLat, b.Lat + b.HeightLat );

      if (num2 >= lon && num4 >= lat) {
        return new LLRectangle( lat, lon, num2 - lon, num4 - lat );
      }

      return Empty;
    }

    // ok ???
    // http://greatmaps.codeplex.com/workitem/15981
    private bool IntersectsWith( LLRectangle a )
    {
      return Left < a.Right && Top > a.Bottom && Right > a.Left && Bottom < a.Top;
    }

    // ok ???
    // http://greatmaps.codeplex.com/workitem/15981
    private static LLRectangle Union( LLRectangle a, LLRectangle b )
    {
      return FromLTRB(
          Math.Min( a.Left, b.Left ),
          Math.Max( a.Top, b.Top ),
          Math.Max( a.Right, b.Right ),
          Math.Min( a.Bottom, b.Bottom ) );
    }

    #endregion
    // .
    // |
    // unsure ends here
  }
}
