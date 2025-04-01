using System;

namespace CoordLib.LLShapes
{
  /// <summary>
  /// Point Struct with LatLon items as Points
  /// Lon and Lat are coordinates [deg]
  /// Points are wrapped around to remain (Lon: -180..180, Lat: -90..90)
  /// 
  /// Note: Math operations are performed on the numbers and not in any particular projection
  /// 
  /// Note: all methods go with X,Y (Lon,Lat) parameters
  /// 
  /// </summary>
  [Serializable]
  public struct LLPoint
  {
    /// <summary>
    /// Represents a Point that has X and Y values set to NaN
    /// </summary>
    public static readonly LLPoint Empty = new LLPoint( true );

    // *** STRUCT

    // underlying LatLon
    private LatLon _latLon;

    /// <summary>
    /// Initializes a new instance of an Empty Point /arg does not matter
    /// </summary>
    public LLPoint( bool arg = true ) => _latLon = LatLon.Empty;

    /// <summary>
    /// Initializes a new instance with the given LLPoint
    /// </summary>
    /// <param name="other">Point to copy from</param>
    public LLPoint( LLPoint other ) => _latLon = other.AsLatLon( );

    /// <summary>
    /// Initializes a new instance of the Point struct with the specified coordinates.
    /// </summary>
    /// <param name="lonX">Longitude</param>
    /// <param name="latY">Latitude</param>
    public LLPoint( double lonX, double latY ) => _latLon = new LatLon( latY, lonX );

    /// <summary>
    /// Initializes a new instance of the Point struct with the specified LatLon obj
    /// </summary>
    /// <param name="latLon">A LatLon Item</param>
    public LLPoint( LatLon latLon ) => _latLon = latLon;

    /// <summary>
    /// Returns the Point as LatLon Type
    /// </summary>
    public LatLon AsLatLon( ) => _latLon;

    /// <summary>
    /// returns true if coordinates wasn't assigned
    /// </summary>
    public bool IsEmpty => _latLon.IsEmpty;

    /// <summary>
    /// Longitude of this Point
    /// </summary>
    public double Lon { get => _latLon.Lon; set => _latLon.Lon = value; }
    /// <summary>
    /// Latitude of this Point
    /// </summary>
    public double Lat { get => _latLon.Lat; set => _latLon.Lat = value; }

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
    /// Compares two Point objects. The result specifies whether the values of the X and Y properties of the two Point objects are equal.
    /// </summary>
    public static bool operator ==( LLPoint left, LLPoint right ) => left.Lon == right.Lon && left.Lat == right.Lat;

    /// <summary>
    /// Compares two Point objects. The result specifies whether the values of the X or Y properties of the two Point objects are unequal.
    /// </summary>
    public static bool operator !=( LLPoint left, LLPoint right ) => !(left == right);

    /// <summary>
    /// Adds the specified width and height to the specified Point.
    /// Returns a new LLPoint as result
    /// </summary>
    public static LLPoint Add( LLPoint pt, double widthLon, double heightLat ) => new LLPoint( pt.Lon + widthLon, pt.Lat + heightLat );

    /// <summary>
    /// Adds the specified Size to the specified Point.
    /// Returns a new LLPoint as result
    /// </summary>
    public static LLPoint Add( LLPoint pt, LLSize sz ) => Add( pt, sz.WidthLon, sz.HeightLat );
    /// <summary>
    /// Subtracts the specified width and height from the specified Point.
    /// Returns a new LLPoint as result
    /// </summary>
    public static LLPoint Subtract( LLPoint pt, double widthLon, double heightLat ) => Add( pt, -widthLon, -heightLat );
    /// <summary>
    /// Returns a new LLPoint as result
    /// Returns the result of subtracting specified Size from the specified Point.
    /// </summary>
    public static LLPoint Subtract( LLPoint pt, LLSize sz ) => Add( pt, -sz.WidthLon, -sz.HeightLat );

    /// <summary>
    /// Translates this Point by the specified amount 
    /// </summary>
    public void Offset( double distLlonX, double distLatY ) { Lon = Lon + distLlonX; Lat = Lat + distLatY; }
    /// <summary>
    /// Translates this Point by the specified distance
    /// </summary>
    public void Offset( LLSize distance ) => Offset( distance.WidthLon, distance.HeightLat );
    /// <summary>
    /// Translates this Point by the specified Point (as offsets).
    /// </summary>
    public void Offset( LLPoint pos ) => Offset( pos.Lon, pos.Lat );

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
    /// Returns a hash code for this Point.
    /// </summary>
    public override int GetHashCode( ) => Lon.GetHashCode( ) ^ Lat.GetHashCode( );

    /// <summary>
    /// Converts this Point to a human-readable string.
    /// </summary>
    public override string ToString( )
    {
      return string.Format( System.Globalization.CultureInfo.InvariantCulture, "{{Lon={0}, Lat={1}}}", Lon, Lat );
    }
  }
}
