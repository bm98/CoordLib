using System;
using System.Globalization;
using System.Xml;

namespace CoordLib
{
  /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
  /* Latitude/longitude spherical geodesy tools                         (c) Chris Veness 2002-2017  */
  /*                                                                                   MIT Licence  */
  /* www.movable-type.co.uk/scripts/latlong.html                                                    */
  /* www.movable-type.co.uk/scripts/geodesy/docs/module-latlon-spherical.html                       */
  /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
  /// <summary>
  /// Implements a Lat, Lon point
  /// 1:1 C# translation of functions from above
  /// Redesign to provide Static Functions with Lat/Lon doubles in Geo.XY
  /// and using them here for the LatLon Class
  /// 
  /// Added Altitude for convenience
  /// Added UTM Zone resolution
  /// 
  /// Changed to Struct (Aug 2033/BM)
  /// </summary>
  public struct LatLon : IEquatable<LatLon>, IComparable<LatLon>
  {
    /// <summary>
    /// Returns an Empty LatLon (Lat,Lon,Alt=NaN)
    /// </summary>
    public static LatLon Empty => new LatLon( false );


    /// <summary>
    /// Latitude
    /// </summary>
    private double _lat;

    /// <summary>
    /// Longitude
    /// </summary>
    private double _lon;

    /// <summary>
    /// Altitude above ellipsoid (meter preferred)
    /// Is set to 0 if not provided
    /// </summary>
    private double _altitude;

    /// <summary>
    /// Latitude part
    /// </summary>
    public double Lat { get => _lat; set => _lat = Geo.Wrap90( value ); }
    /// <summary>
    /// Longitude part
    /// </summary>
    public double Lon { get => _lon; set => _lon = Geo.Wrap180( value ); }

    /// <summary>
    /// Altitude part metres above ellipsoid.
    /// </summary>
    public double Altitude { get => _altitude; set => _altitude = value; }

    /// <summary>
    /// True if Lat or Lon are not assigned
    /// </summary>
    public bool IsEmpty => (double.IsNaN( _lat ) || double.IsNaN( _lon ));

    /// <summary>
    /// True if Alt is set (not NaN)
    /// </summary>
    public bool HasAltitude => !double.IsNaN( Altitude );

    /// <summary>
    /// Creates a LatLon point on the earth's surface at the specified latitude / longitude.
    ///  initialized to 0/0/0
    /// </summary>
    /// <param name="initZero">Will init an 0/0/0 item if true (default) else it's Empty</param>
    public LatLon( bool initZero = true )
    {
      if (initZero) {
        _lat = 0; _lon = 0; _altitude = 0;
      }
      else {
        _lat = double.NaN;
        _lon = double.NaN;
        _altitude = double.NaN;
      }
    }

    /// <summary>
    /// Creates a LatLon point on the earth's surface at the specified latitude / longitude.
    /// </summary>
    /// <param name="lat"></param>
    /// <param name="lon"></param>
    public LatLon( double lat, double lon )
    {
      _lat = Geo.Wrap90( lat );
      _lon = Geo.Wrap180( lon );
      _altitude = 0;
    }

    /// <summary>
    /// Creates a LatLon point on the earth's surface at the specified latitude / longitude / altitude.
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="alt">Altitude above the ellipsoid</param>
    public LatLon( double lat, double lon, double alt )
    {
      _lat = Geo.Wrap90( lat );
      _lon = Geo.Wrap180( lon );
      _altitude = alt;
    }

    /// <summary>
    /// Creates a LatLon point on the earth's surface at the specified latitude / longitude.
    /// </summary>
    /// <param name="latLon"> Array of double where [0] = Lat, [1] = Lon, [2]= Alt (if provided)</param>
    public LatLon( double[] latLon )
    {
      // init with 0
      _lat = 0;
      _lon = 0;
      _altitude = 0;
      // lat / lon given?
      if (latLon?.Length > 1) {
        _lat = Geo.Wrap90( latLon[0] );
        _lon = Geo.Wrap180( latLon[1] );
        _altitude = 0;
      }
      // akt given?
      if (latLon?.Length > 2) {
        _altitude = latLon[2];
      }
    }

    /// <summary>
    /// Creates a LatLon point on the earth's surface at the specified latitude / longitude.
    ///   as copy of the given LatLon
    /// </summary>
    /// <param name="other">LatLon to copy from</param>
    public LatLon( LatLon other )
    {
      _lat = Geo.Wrap90( other.Lat );
      _lon = Geo.Wrap180( other.Lon );
      _altitude = other.Altitude;
    }

    /// <summary>
    /// To set the Altitude on created objects
    /// </summary>
    /// <param name="alt">An altitude value</param>
    public void SetAltitude( double alt ) => _altitude = alt;

    /// <summary>
    /// Equality of a LatLon (lat, lon, alt)
    /// </summary>
    public static bool operator ==( LatLon latLon, LatLon other )
    {
      return latLon.Equals( other ); // full check is done in Equals<LatLon>()
    }
    /// <summary>
    /// Inequality of a LatLon (lat, lon, alt)
    /// </summary>
    public static bool operator !=( LatLon latLon, LatLon other )
    {
      return !latLon.Equals( other );
    }

    /// <summary>
    /// Checks if another point is equal to ‘this’ point.
    /// 
    ///      * @example
    ///      *   var p1 = new LatLon( 52.205, 0.119 );
    ///      *   var p2 = new LatLon( 52.205, 0.119 );
    ///      *   var equal = p1.equals( p2 ); // true
    /// </summary>
    /// <param name="other">{LatLon} point - Point to be compared against this point.</param>
    /// <returns>{bool}   True if points are identical.</returns>
    public bool Equals( LatLon other )
    {
      // NaN never equals with == !!

      // Lat check
      if (double.IsNaN( this.Lat )) {
        if (!double.IsNaN( other.Lat )) return false;
      }
      else {
        if (double.IsNaN( other.Lat )) return false;
        if (this.Lat != other.Lat) return false;
      }

      // Lon check
      if (double.IsNaN( this.Lon )) {
        if (!double.IsNaN( other.Lon )) return false;
      }
      else {
        if (double.IsNaN( other.Lon )) return false;
        if (this.Lon != other.Lon) return false;
      }

      // Alt check
      if (double.IsNaN( this.Altitude )) {
        if (!double.IsNaN( other.Altitude )) return false;
      }
      else {
        if (double.IsNaN( other.Altitude )) return false;
        if (this.Altitude != other.Altitude) return false;
      }
      // must be the same...
      return true;
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="obj">An object</param>
    public override bool Equals( object obj )
    {
      if (obj is LatLon latlon) {
        return this.Equals( latlon );
      }
      return false;
    }

    /// <summary>
    /// Returns the hash code for this instance
    /// </summary>
    public override int GetHashCode( )
    {
      return (Lat.GetHashCode( ) ^ Lon.GetHashCode( )) ^ Altitude.GetHashCode( );
    }

    /// <summary>
    /// Compare this with another LatLon
    /// Larger, Smaller decided on Latitude, then Longitude, then Altitude if set
    /// If this or the other is empty it returns 0
    /// </summary>
    /// <param name="other">The other LatLon</param>
    /// <returns>+1 if this is greater than the other,-1 if smaller and 0 if the same</returns>
    public int CompareTo( LatLon other )
    {
      if (this.IsEmpty || other.IsEmpty) return 0;
      if (this.Equals( other )) return 0;
      // Latitude first
      if (this.Lat > other.Lat) return 1;
      if (this.Lat < other.Lat) return -1;
      // then longitude
      if (this.Lon > other.Lon) return 1;
      if (this.Lon < other.Lon) return -1;
      // then Altitude
      if (double.IsNaN( other.Altitude )) {
        return double.IsNaN( this.Altitude ) ? 0 : 1;
      }
      else if (double.IsNaN( this.Altitude )) {
        return double.IsNaN( other.Altitude ) ? 0 : -1;
      }
      // neither Alt is NaN, compare them
      if (this.Altitude > other.Altitude) return 1;
      if (this.Altitude < other.Altitude) return -1;
      return 0; // same - should be catched above anyway
    }

    /// <summary>
    /// Returns the distance from ‘this’ point to destination point (using haversine formula).
    ///      * @example
    ///      *     var p1 = new LatLon( 52.205, 0.119 );
    ///      *     var p2 = new LatLon( 48.857, 2.351 );
    ///      *     var d = p1.distanceTo( p2 ); // 404.3 km
    /// </summary>
    /// <param name="point">{LatLon} point - Latitude/longitude of destination point</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>{number} Distance between this point and destination point, in same units as radius.</returns>
    public double DistanceTo( LatLon point, double radius = ConvConsts.EarthRadiusM )
    {
      return Geo.DistanceTo( _lat, _lon, point.Lat, point.Lon, radius );
    }


    /// <summary>
    /// Returns the (initial) bearing from ‘this’ point to destination point.
    /// 
    ///      * @example
    ///      *     var p1 = new LatLon( 52.205, 0.119 );
    ///      *     var p2 = new LatLon( 48.857, 2.351 );
    ///      *     var b1 = p1.bearingTo( p2 ); // 156.2°
    /// </summary>
    /// <param name="point">{LatLon} point - Latitude/longitude of destination point.</param>
    /// <returns>{number} Initial bearing in degrees from north.</returns>
    public double BearingTo( LatLon point )
    {
      return Geo.BearingTo( _lat, _lon, point.Lat, point.Lon );
    }

    /// <summary>
    /// Returns final bearing arriving at destination destination point from ‘this’ point; the final bearing
    /// will differ from the initial bearing by varying degrees according to distance and latitude.
    /// 
    ///      * @example
    ///      *     var p1 = new LatLon( 52.205, 0.119 );
    ///      *     var p2 = new LatLon( 48.857, 2.351 );
    ///      *     var b2 = p1.finalBearingTo( p2 ); // 157.9°
    /// </summary>
    /// <param name="point">{LatLon} point - Latitude/longitude of destination point.</param>
    /// <returns>{number} Final bearing in degrees from north.</returns>
    public double FinalBearingTo( LatLon point )
    {
      return Geo.FinalBearingTo( _lat, _lon, point.Lat, point.Lon );
    }


    /// <summary>
    /// Returns the midpoint between ‘this’ point and the supplied point.
    /// 
    ///  * @example
    ///  *     var p1 = new LatLon( 52.205, 0.119 );
    ///  *     var p2 = new LatLon( 48.857, 2.351 );
    ///  *     var pMid = p1.midpointTo( p2 ); // 50.5363°N, 001.2746°E
    /// </summary>
    /// <param name="point">{LatLon} point - Latitude/longitude of destination point.</param>
    /// <returns>{LatLon} Midpoint between this point and the supplied point.</returns>
    public LatLon MidpointTo( LatLon point )
    {
      var midLL = new LatLon( Geo.MidpointTo( _lat, _lon, point.Lat, point.Lon ) );
      if (this.HasAltitude && point.HasAltitude) {
        midLL.Altitude = (this.Altitude + point.Altitude) / 2.0;
      }
      else {
        midLL.Altitude = double.NaN; // set to NaN if either of the two does not have an Altitude
      }
      return midLL;
    }


    /// <summary>
    /// Returns the point at given fraction between ‘this’ point and specified point.
    /// 
    ///      * @example
    ///      *   let p1 = new LatLon( 52.205, 0.119 );
    ///      *   let p2 = new LatLon( 48.857, 2.351 );
    ///      *   let pMid = p1.intermediatePointTo( p2, 0.25 ); // 51.3721°N, 000.7073°E
    /// </summary>
    /// <param name="point">{LatLon} point - Latitude/longitude of destination point.</param>
    /// <param name="fraction">{number} fraction - Fraction between the two points (0 = this point, 1 = specified point).</param>
    /// <returns>{LatLon} Intermediate point between this point and destination point.</returns>
    public LatLon IntermediatePointTo( LatLon point, double fraction )
    {
      var fracLL = new LatLon( Geo.IntermediatePointTo( _lat, _lon, point.Lat, point.Lon, fraction ) );
      if (this.HasAltitude && point.HasAltitude) {
        fracLL.Altitude = this.Altitude + (point.Altitude - this.Altitude) * fraction;
      }
      else {
        fracLL.Altitude = double.NaN; // set to NaN if either of the two don't have an Altitude
      }
      return fracLL;
    }


    /// <summary>
    /// Returns the destination point from ‘this’ point having travelled the given distance on the
    /// given initial bearing( bearing normally varies around path followed ).
    /// 
    ///      * @example
    ///      *     var p1 = new LatLon( 51.4778, -0.0015 );
    ///      *     var p2 = p1.destinationPoint( 7794, 300.7 ); // 51.5135°N, 000.0983°W
    /// </summary>
    /// <param name="distance">{number} distance - Distance travelled, in same units as earth radius (default: metres).</param>
    /// <param name="bearing">{number} bearing - Initial bearing in degrees from north.</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>{LatLon} Destination point.</returns>
    public LatLon DestinationPoint( double distance, double bearing, double radius = ConvConsts.EarthRadiusM )
    {
      var destLL = new LatLon( Geo.DestinationPoint( _lat, _lon, distance, bearing, radius ) ) {
        Altitude = this.Altitude
      };
      return destLL;
    }


    /// <summary>
    /// Returns the point of intersection of two paths defined by point and bearing.
    /// 
    ///      * @example
    ///      *     var p1 = LatLon( 51.8853, 0.2545 ), brng1 = 108.547;
    ///      *     var p2 = LatLon( 49.0034, 2.5735 ), brng2 = 32.435;
    ///      *     var pInt = LatLon.intersection( p1, brng1, p2, brng2 ); // 50.9078°N, 004.5084°E
    /// </summary>
    /// <param name="p1">{LatLon} p1 - First point.</param>
    /// <param name="brng1">{number} brng1 - Initial bearing from first point.</param>
    /// <param name="p2">{LatLon} p2 - Second point.</param>
    /// <param name="brng2">{number} brng2 - Initial bearing from second point.</param>
    /// <returns>{LatLon|null} Destination point (null if no unique intersection defined).</returns>
    public static LatLon Intersection( LatLon p1, double brng1, LatLon p2, double brng2 )
    {
      if (p1 == p2) return p1; // shortcut
      if (p1.Lat == p2.Lat && p1.Lon == p2.Lon) {
        var ret = p1;
        if (p1.HasAltitude && p2.HasAltitude) {
          // shortcut with identical coordinates
          ret.Altitude = (p1.Altitude + p2.Altitude) / 2.0;
        }
        return p1;
      }
      // calculate
      var isecLL = new LatLon( Geo.Intersection( p1.Lat, p1.Lon, brng1, p2.Lat, p2.Lon, brng2 ) );
      if (p1.HasAltitude && p2.HasAltitude) {
        // must calculate the fraction of the two pathlength to get an Altitude
        var l1 = p1.DistanceTo( isecLL );
        var l2 = p2.DistanceTo( isecLL );
        isecLL.Altitude = p1.Altitude + (p2.Altitude - p1.Altitude) * (l1 / (l1 + l2));// potential Div0 if both length are 0 due to numeric issues
      }
      return isecLL;
    }


    /// <summary>
    /// Returns (signed) distance from ‘this’ point to great circle defined by start-point and end-point.
    /// 
    ///      * @example
    ///      *   var pCurrent = new LatLon( 53.2611, -0.7972 );
    ///      *   var p1 = new LatLon( 53.3206, -1.7297 );
    ///      *   var p2 = new LatLon( 53.1887, 0.1334 );
    ///      *   var d = pCurrent.crossTrackDistanceTo( p1, p2 );  // -307.5 m
    /// </summary>
    /// <param name="pathStart">{LatLon} pathStart - Start point of great circle path.</param>
    /// <param name="pathEnd">{LatLon} pathEnd - End point of great circle path.</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>{number} Distance to great circle (-ve if to left, +ve if to right of path).</returns>
    public double DoubleCrossTrackDistanceTo( LatLon pathStart, LatLon pathEnd, double radius = ConvConsts.EarthRadiusM )
    {
      return Geo.DoubleCrossTrackDistanceTo( _lat, _lon, pathStart.Lat, pathStart.Lon, pathEnd.Lat, pathEnd.Lon, radius );
    }


    /// <summary>
    /// Returns how far ‘this’ point is along a path from from start-point, heading towards end-point.
    /// That is, if a perpendicular is drawn from ‘this’ point to the( great circle ) path, the along-track
    /// distance is the distance from the start point to where the perpendicular crosses the path.
    /// 
    ///      * @example
    ///      *   var pCurrent = new LatLon( 53.2611, -0.7972 );
    ///      *   var p1 = new LatLon( 53.3206, -1.7297 );
    ///      *   var p2 = new LatLon( 53.1887, 0.1334 );
    ///      *   var d = pCurrent.alongTrackDistanceTo( p1, p2 );  // 62.331 km
    /// </summary>
    /// <param name="pathStart">{LatLon} pathStart - Start point of great circle path.</param>
    /// <param name="pathEnd">{LatLon} pathEnd - End point of great circle path.</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>{number} Distance along great circle to point nearest ‘this’ point.</returns>
    public double AlongTrackDistanceTo( LatLon pathStart, LatLon pathEnd, double radius = ConvConsts.EarthRadiusM )
    {
      return Geo.AlongTrackDistanceTo( _lat, _lon, pathStart.Lat, pathStart.Lon, pathEnd.Lat, pathEnd.Lon, radius );
    }


    /// <summary>
    /// Returns maximum latitude reached when travelling on a great circle on given bearing from this
    /// point('Clairaut's formula'). Negate the result for the minimum latitude (in the Southern
    /// hemisphere).
    /// 
    /// The maximum latitude is independent of longitude; it will be the same for all points on a given
    /// latitude.
    /// </summary>
    /// <param name="bearing">{number} bearing - Initial bearing.</param>
    /// <returns>maximum latitude reached</returns>
    public double MaxLatitude( double bearing )
    {
      return Geo.MaxLatitude( _lat, bearing );
    }


    /// <summary>
    /// Returns the pair of meridians at which a great circle defined by two points crosses the given
    /// latitude.If the great circle doesn't reach the given latitude, null is returned.
    /// </summary>
    /// <param name="point1">{LatLon} point1 - First point defining great circle.</param>
    /// <param name="point2">{LatLon} point2 - Second point defining great circle.</param>
    /// <param name="latitude">{number} latitude - Latitude crossings are to be determined for.</param>
    /// <returns>{Object|null} Object containing { lon1, lon2 } or null if given latitude not reached.</returns>
    public static double[] CrossingParallels( LatLon point1, LatLon point2, double latitude )
    {
      return Geo.CrossingParallels( point1.Lat, point1.Lon, point2.Lat, point2.Lon, Geo.Wrap90( latitude ) );
    }


    /// <summary>
    /// Returns a string representation of ‘this’ point, formatted as degrees, degrees+minutes, or
    /// degrees+minutes+seconds.
    /// </summary>
    /// <param name="format">{string} [format=dms] - Format point as 'd', 'dm', 'dms'.</param>
    /// <param name="separator">Item separator string - default a space</param>
    /// <param name="dp">{number} [dp=0|2|4] - Number of decimal places to use - default 0 for dms, 2 for dm, 4 for d.</param>
    /// <returns>{string} Comma-separated latitude/longitude.</returns>
    public string ToString( string format = "dms", string separator = " ", int dp = 0 )
    {
      return Dms.ToLat( _lat, format, separator, dp ) + ", " + Dms.ToLon( _lon, format, separator, dp );
    }

    /// <summary>
    /// Returns the default string representation 
    /// </summary>
    /// <returns>A string</returns>
    public override string ToString( )
    {
      if (double.IsNaN( _altitude ))
        return ToString( "dms" );
      else
        return ToString( "dms" ) + $", {_altitude:####0}";
    }

    // Crude Serializing support

    /// <summary>
    /// Create a LatLon from a Serializer String
    /// </summary>
    /// <param name="serString">Serialized string</param>
    public LatLon( string serString = "" )
    {
      _lat = double.NaN; _lon = double.NaN; _altitude = double.NaN;
      if (string.IsNullOrEmpty( serString )) return;

      // "lat;lon;alt" as decimals from Invariant Culture
      string[] e = serString.Split( new char[] { ';' } );
      if (e.Length > 0) double.TryParse( e[0], NumberStyles.Float, CultureInfo.InvariantCulture, out _lat );
      if (e.Length > 1) double.TryParse( e[1], NumberStyles.Float, CultureInfo.InvariantCulture, out _lon );
      if (e.Length > 2) double.TryParse( e[2], NumberStyles.Float, CultureInfo.InvariantCulture, out _altitude );
    }

    /// <summary>
    /// Provide A serializer string
    /// </summary>
    public string ToSerString( )
    {
      return $"{_lat.ToString( "#0.000000", CultureInfo.InvariantCulture )};{_lon.ToString( "##0.000000", CultureInfo.InvariantCulture )};{_altitude.ToString( "####0.0", CultureInfo.InvariantCulture )}";
    }

  }

}


