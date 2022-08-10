using System;
using System.Collections.Generic;
using System.Text;

namespace CoordLib
{
  /// <summary>
  /// Point Struct with LatLon items as Points
  /// Note: Math operations are performed on the numbers and not in any particular projection
  /// </summary>
  public struct LLPoint
  {
    /// <summary>
    /// Represents a Point that has X and Y values set to zero.
    /// </summary>
    public static readonly LLPoint Empty = new LLPoint( );

    // underlying LatLon
    private LatLon _latLon;

    /// <summary>
    /// Initializes a new instance of the Point struct with the specified coordinates.
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    public LLPoint( double lat, double lon )
    {
      _latLon = new LatLon( lat, lon );
    }

    /// <summary>
    /// Initializes a new instance of the Point struct with the specified LatLon obj
    /// </summary>
    /// <param name="latLon">A LatLon Item</param>
    public LLPoint( LatLon latLon )
    {
      _latLon = latLon;
    }

    /// <summary>
    /// Returns the Point as LatLon Type
    /// </summary>
    public LatLon AsLatLon( ) => _latLon;

    /// <summary>
    /// returns true if coordinates wasn't assigned
    /// </summary>
    public bool IsEmpty {
      get {
        return _latLon.IsEmpty;
      }
    }
    /// <summary>
    /// Latitude of this Point
    /// </summary>
    public double Lat { get => _latLon.Lat; set => _latLon.Lat = value; }
    /// <summary>
    /// Longitude of this Point
    /// </summary>
    public double Lon { get => _latLon.Lon; set => _latLon.Lon = value; }

    /// <summary>
    /// Returns the Point as LatLon Type
    /// </summary>
    public static explicit operator LatLon( LLPoint pt ) => pt._latLon;

    /// <summary>
    /// Translates a Point by a given Size.
    /// </summary>
    public static LLPoint operator +( LLPoint pt, LLSize sz ) => Add( pt, sz );

    /// <summary>
    /// Translates a Point by the negative of a given Size.
    /// </summary>
    public static LLPoint operator -( LLPoint pt, LLSize sz ) => Subtract( pt, sz );
    /// <summary>
    /// Translates a Point by the negative of a given Point
    /// </summary>
    public static LLSize operator -( LLPoint pt1, LLPoint pt2 ) => new LLSize( pt1.Lat - pt2.Lat, pt2.Lon - pt1.Lon );

    /// <summary>
    /// Compares two Point objects. The result specifies whether the values of the X and Y properties of the two Point objects are equal.
    /// </summary>
    public static bool operator ==( LLPoint left, LLPoint right ) => left.Lon == right.Lon && left.Lat == right.Lat;

    /// <summary>
    /// Compares two Point objects. The result specifies whether the values of the X or Y properties of the two Point objects are unequal.
    /// </summary>
    public static bool operator !=( LLPoint left, LLPoint right ) => !(left == right);

    /// <summary>
    /// Adds the specified Size to the specified Point.
    /// </summary>
    public static LLPoint Add( LLPoint pt, LLSize sz ) => new LLPoint( pt.Lat - sz.HeightLat, pt.Lon + sz.WidthLon );
    /// <summary>
    /// Returns the result of subtracting specified Size from the specified Point.
    /// </summary>
    public static LLPoint Subtract( LLPoint pt, LLSize sz ) => new LLPoint( pt.Lat + sz.HeightLat, pt.Lon - sz.WidthLon );
    /// <summary>
    /// Specifies whether this point instance contains the same coordinates as the specified object.
    /// </summary>
    public override bool Equals( object obj )
    {
      if (!(obj is LLPoint)) return false;

      LLPoint tf = (LLPoint)obj;
      return tf.Lon == Lon && tf.Lat == Lat && tf.GetType( ).Equals( GetType( ) );
    }
    /// <summary>
    /// Translates this Point by the specified Point.
    /// </summary>
    public void Offset( LLPoint pos ) => Offset( pos.Lat, pos.Lon );
    /// <summary>
    /// Translates this Point by the specified amount.
    /// </summary>
    public void Offset( double lat, double lon ) { Lon += lon; Lat -= lat; }
    /// <summary>
    /// Returns a hash code for this Point.
    /// </summary>
    public override int GetHashCode( )=> Lon.GetHashCode( ) ^ Lat.GetHashCode( );

    /// <summary>
    /// Converts this Point to a human-readable string.
    /// </summary>
    public override string ToString( )
    {
      return string.Format( System.Globalization.CultureInfo.InvariantCulture, "{{Lat={0}, Lon={1}}}", Lat, Lon );
    }
  }
}
