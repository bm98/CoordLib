using System;
using System.Collections.Generic;
using System.Text;

using CoordLib.Extensions;

namespace CoordLib
{
  /// <summary>
  /// Geodetic functions
  /// A pool of static base functions
  /// 
  /// Derived from 
  /// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
  /// Latitude/longitude spherical geodesy tools                         (c) Chris Veness 2002-2017  */
  ///                                                                                   MIT Licence  */
  /// www.movable-type.co.uk/scripts/latlong.html                                                    */
  /// www.movable-type.co.uk/scripts/geodesy/docs/module-latlon-spherical.html                       */
  /// https://github.com/chrisveness/geodesy
  /// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
  /// </summary>
  public class Geo
  {
    /// <summary>
    /// Returns the distance from ‘this’ point to destination point (using haversine formula).
    /// </summary>
    /// <param name="thisLat">Latitude of starting point</param>
    /// <param name="thisLon">Longitude of starting point</param>
    /// <param name="pointLat">Latitude of destination point</param>
    /// <param name="pointLon">Longitude of destination point</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>{number} Distance between this point and destination point, in same units as radius.</returns>
    public static double DistanceTo( double thisLat, double thisLon,
                              double pointLat, double pointLon,
                              double radius = ConvConsts.EarthRadiusM )
    {
      // a = sin²(Δφ/2) + cos(φ1)⋅cos(φ2)⋅sin²(Δλ/2)
      // tanδ = √(a) / √(1−a)
      // see mathforum.org/library/drmath/view/51879.html for derivation
      var R = radius;
      double φ1 = thisLat.ToRadians( );
      double λ1 = thisLon.ToRadians( );
      double φ2 = pointLat.ToRadians( );
      double λ2 = pointLon.ToRadians( );
      double Δφ = φ2 - φ1;
      double Δλ = λ2 - λ1;

      var a = Math.Sin( Δφ / 2 ) * Math.Sin( Δφ / 2 )
            + Math.Cos( φ1 ) * Math.Cos( φ2 )
            * Math.Sin( Δλ / 2 ) * Math.Sin( Δλ / 2 );
      var c = 2 * Math.Atan2( Math.Sqrt( a ), Math.Sqrt( 1 - a ) );
      var d = R * c;

      return d;
    }


    /// <summary>
    /// Returns the (initial) bearing from ‘this’ point to destination point.
    /// </summary>
    /// <param name="thisLat">Latitude of starting point</param>
    /// <param name="thisLon">Longitude of starting point</param>
    /// <param name="pointLat">Latitude of destination point</param>
    /// <param name="pointLon">Longitude of destination point</param>
    /// <returns>{number} Initial bearing in degrees from north.</returns>
    public static double BearingTo( double thisLat, double thisLon,
                             double pointLat, double pointLon )
    {
      // tanθ = sinΔλ⋅cosφ2 / cosφ1⋅sinφ2 − sinφ1⋅cosφ2⋅cosΔλ
      // see mathforum.org/library/drmath/view/55417.html for derivation
      double φ1 = thisLat.ToRadians( );
      double φ2 = pointLat.ToRadians( );
      double Δλ = (pointLon - thisLon).ToRadians( );
      var y = Math.Sin( Δλ ) * Math.Cos( φ2 );
      var x = Math.Cos( φ1 ) * Math.Sin( φ2 ) -
              Math.Sin( φ1 ) * Math.Cos( φ2 ) * Math.Cos( Δλ );
      var θ = Math.Atan2( y, x );

      return Wrap360( θ.ToDegrees( ) );
    }


    /// <summary>
    /// Returns final bearing arriving at destination destination point from ‘this’ point; the final bearing
    /// will differ from the initial bearing by varying degrees according to distance and latitude.
    /// </summary>
    /// <param name="thisLat">Latitude of starting point</param>
    /// <param name="thisLon">Longitude of starting point</param>
    /// <param name="pointLat">Latitude of destination point</param>
    /// <param name="pointLon">Longitude of destination point</param>
    /// <returns>{number} Final bearing in degrees from north.</returns>
    public static double FinalBearingTo( double thisLat, double thisLon,
                                  double pointLat, double pointLon )
    {
      // get initial bearing from destination point to this point & reverse it by adding 180°
      return Wrap360( BearingTo( pointLat, pointLon, thisLat, thisLon ) + 180 );
    }


    /// <summary>
    /// Returns the midpoint between ‘this’ point and the supplied point.
    /// </summary>
    /// <param name="thisLat">Latitude of starting point</param>
    /// <param name="thisLon">Longitude of starting point</param>
    /// <param name="pointLat">Latitude of destination point</param>
    /// <param name="pointLon">Longitude of destination point</param>
    /// <returns>Lat and Lon of the Mid point as array</returns>
    public static double[] MidpointTo( double thisLat, double thisLon,
                                  double pointLat, double pointLon )
    {
      // φm = atan2( sinφ1 + sinφ2, √( (cosφ1 + cosφ2⋅cosΔλ) ⋅ (cosφ1 + cosφ2⋅cosΔλ) ) + cos²φ2⋅sin²Δλ )
      // λm = λ1 + atan2(cosφ2⋅sinΔλ, cosφ1 + cosφ2⋅cosΔλ)
      // see mathforum.org/library/drmath/view/51822.html for derivation
      double φ1 = thisLat.ToRadians( );
      double λ1 = thisLon.ToRadians( );
      double φ2 = pointLat.ToRadians( );
      double Δλ = (double)(pointLon - thisLon).ToRadians( );

      var Bx = Math.Cos( φ2 ) * Math.Cos( Δλ );
      var By = Math.Cos( φ2 ) * Math.Sin( Δλ );

      var x = Math.Sqrt( (Math.Cos( φ1 ) + Bx) * (Math.Cos( φ1 ) + Bx) + By * By );
      var y = Math.Sin( φ1 ) + Math.Sin( φ2 );
      var φ3 = Math.Atan2( y, x );

      var λ3 = λ1 + Math.Atan2( By, Math.Cos( φ1 ) + Bx );
      return new double[] { Wrap90( φ3.ToDegrees( ) ), Wrap180( λ3.ToDegrees( ) ) }; // normalise to −180..+180°
    }


    /// <summary>
    /// Returns the point at given fraction between ‘this’ point and specified point.
    /// </summary>
    /// <param name="thisLat">Latitude of starting point</param>
    /// <param name="thisLon">Longitude of starting point</param>
    /// <param name="pointLat">Latitude of destination point</param>
    /// <param name="pointLon">Longitude of destination point</param>
    /// <param name="fraction">{number} fraction - Fraction between the two points (0 = this point, 1 = specified point).</param>
    /// <returns>Lat and Lon of the intermediate point as array</returns>
    public static double[] IntermediatePointTo( double thisLat, double thisLon,
                                  double pointLat, double pointLon,
                                  double fraction )
    {
      double φ1 = thisLat.ToRadians( );
      double λ1 = thisLon.ToRadians( );
      double φ2 = pointLat.ToRadians( );
      double λ2 = pointLon.ToRadians( );

      double sinφ1 = Math.Sin( φ1 ), cosφ1 = Math.Cos( φ1 ), sinλ1 = Math.Sin( λ1 ), cosλ1 = Math.Cos( λ1 );
      double sinφ2 = Math.Sin( φ2 ), cosφ2 = Math.Cos( φ2 ), sinλ2 = Math.Sin( λ2 ), cosλ2 = Math.Cos( λ2 );

      // distance between points
      var Δφ = φ2 - φ1;
      var Δλ = λ2 - λ1;
      var a = Math.Sin( Δφ / 2 ) * Math.Sin( Δφ / 2 )
          + Math.Cos( φ1 ) * Math.Cos( φ2 ) * Math.Sin( Δλ / 2 ) * Math.Sin( Δλ / 2 );
      var δ = 2 * Math.Atan2( Math.Sqrt( a ), Math.Sqrt( 1 - a ) );

      var A = Math.Sin( (1 - fraction) * δ ) / Math.Sin( δ );
      var B = Math.Sin( fraction * δ ) / Math.Sin( δ );

      var x = A * cosφ1 * cosλ1 + B * cosφ2 * cosλ2;
      var y = A * cosφ1 * sinλ1 + B * cosφ2 * sinλ2;
      var z = A * sinφ1 + B * sinφ2;

      var φ3 = Math.Atan2( z, Math.Sqrt( x * x + y * y ) );
      var λ3 = Math.Atan2( y, x );

      return new double[] { Wrap90( φ3.ToDegrees( ) ), Wrap180( λ3.ToDegrees( ) ) }; // normalise lon to −180..+180°
    }


    /// <summary>
    /// Returns the destination point from ‘this’ point having travelled the given distance on the
    /// given initial bearing( bearing normally varies around path followed ).
    /// </summary>
    /// <param name="thisLat">Latitude of starting point</param>
    /// <param name="thisLon">Longitude of starting point</param>
    /// <param name="distance">{number} distance - Distance travelled, in same units as earth radius (default: metres).</param>
    /// <param name="bearing">{number} bearing - Initial bearing in degrees from north.</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>Lat and Lon of the Destination point as array</returns>
    public static double[] DestinationPoint( double thisLat, double thisLon,
                                        double distance, double bearing,
                                        double radius = ConvConsts.EarthRadiusM )
    {
      // sinφ2 = sinφ1⋅cosδ + cosφ1⋅sinδ⋅cosθ
      // tanΔλ = sinθ⋅sinδ⋅cosφ1 / cosδ−sinφ1⋅sinφ2
      // see mathforum.org/library/drmath/view/52049.html for derivation
      double δ = distance / radius; // angular distance in radians
      double θ = bearing.ToRadians( );

      double φ1 = thisLat.ToRadians( );
      double λ1 = thisLon.ToRadians( );

      double sinφ1 = Math.Sin( φ1 ), cosφ1 = Math.Cos( φ1 );
      double sinδ = Math.Sin( δ ), cosδ = Math.Cos( δ );
      double sinθ = Math.Sin( θ ), cosθ = Math.Cos( θ );

      var sinφ2 = sinφ1 * cosδ + cosφ1 * sinδ * cosθ;
      var φ2 = Math.Asin( sinφ2 );
      var y = sinθ * sinδ * cosφ1;
      var x = cosδ - sinφ1 * sinφ2;
      var λ2 = λ1 + Math.Atan2( y, x );

      return new double[] { Wrap90( φ2.ToDegrees( ) ), Wrap180( λ2.ToDegrees( ) ) }; // normalise to −180..+180°
    }


    /// <summary>
    /// Returns the point of intersection of two paths defined by point and bearing.
    /// </summary>
    /// <param name="p1Lat">Latitude of First point</param>
    /// <param name="p1Lon">Longitude of First point</param>
    /// <param name="brng1">{number} brng1 - Initial bearing from first point.</param>
    /// <param name="p2Lat">Latitude of Second point</param>
    /// <param name="p2Lon">Longitude of Second point</param>
    /// <param name="brng2">{number} brng2 - Initial bearing from second point.</param>
    /// <returns>Lat and Lon of the Intersection point as array (null if no unique intersection defined)</returns>
    public static double[] Intersection( double p1Lat, double p1Lon, double brng1,
                                    double p2Lat, double p2Lon, double brng2 )
    {
      // see www.edwilliams.org/avform.htm#Intersection
      double φ1 = p1Lat.ToRadians( );
      double λ1 = p1Lon.ToRadians( );
      double φ2 = p2Lat.ToRadians( );
      double λ2 = p2Lon.ToRadians( );
      double θ13 = brng1.ToRadians( ), θ23 = brng2.ToRadians( );
      double Δφ = φ2 - φ1;
      double Δλ = λ2 - λ1;

      // angular distance p1-p2
      var δ12 = 2 * Math.Asin( Math.Sqrt( Math.Sin( Δφ / 2 ) * Math.Sin( Δφ / 2 )
          + Math.Cos( φ1 ) * Math.Cos( φ2 ) * Math.Sin( Δλ / 2 ) * Math.Sin( Δλ / 2 ) ) );
      if (δ12 == 0) return null;

      // initial/final bearings between points
      var θa = Math.Acos( (Math.Sin( φ2 ) - Math.Sin( φ1 ) * Math.Cos( δ12 )) / (Math.Sin( δ12 ) * Math.Cos( φ1 )) );
      if (double.IsNaN( θa )) θa = 0; // protect against rounding
      var θb = Math.Acos( (Math.Sin( φ1 ) - Math.Sin( φ2 ) * Math.Cos( δ12 )) / (Math.Sin( δ12 ) * Math.Cos( φ2 )) );

      var θ12 = Math.Sin( λ2 - λ1 ) > 0 ? θa : 2 * Math.PI - θa;
      var θ21 = Math.Sin( λ2 - λ1 ) > 0 ? 2 * Math.PI - θb : θb;

      var α1 = θ13 - θ12; // angle 2-1-3
      var α2 = θ21 - θ23; // angle 1-2-3

      if (Math.Sin( α1 ) == 0 && Math.Sin( α2 ) == 0) return null; // infinite intersections
      if (Math.Sin( α1 ) * Math.Sin( α2 ) < 0) return null;      // ambiguous intersection

      var α3 = Math.Acos( -Math.Cos( α1 ) * Math.Cos( α2 ) + Math.Sin( α1 ) * Math.Sin( α2 ) * Math.Cos( δ12 ) );
      var δ13 = Math.Atan2( Math.Sin( δ12 ) * Math.Sin( α1 ) * Math.Sin( α2 ), Math.Cos( α2 ) + Math.Cos( α1 ) * Math.Cos( α3 ) );
      var φ3 = Math.Asin( Math.Sin( φ1 ) * Math.Cos( δ13 ) + Math.Cos( φ1 ) * Math.Sin( δ13 ) * Math.Cos( θ13 ) );
      var Δλ13 = Math.Atan2( Math.Sin( θ13 ) * Math.Sin( δ13 ) * Math.Cos( φ1 ), Math.Cos( δ13 ) - Math.Sin( φ1 ) * Math.Sin( φ3 ) );
      var λ3 = λ1 + Δλ13;

      return new double[] { Wrap90( φ3.ToDegrees( ) ), Wrap180( λ3.ToDegrees( ) ) }; // normalise to −180..+180°
    }


    /// <summary>
    /// Returns (signed) distance from ‘this’ point to great circle defined by start-point and end-point.
    /// </summary>
    /// <param name="thisLat">Latitude of starting point</param>
    /// <param name="thisLon">Longitude of starting point</param>
    /// <param name="pStartLat">Latitude of Start point of great circle path</param>
    /// <param name="pStartLon">Longitude of Start point of great circle path</param>
    /// <param name="pEndLat">Latitude of End point of great circle path</param>
    /// <param name="pEndLon">Longitude of End point of great circle path</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>{number} Distance to great circle (-ve if to left, +ve if to right of path).</returns>
    public static double DoubleCrossTrackDistanceTo( double thisLat, double thisLon,
                                                  double pStartLat, double pStartLon,
                                                  double pEndLat, double pEndLon,
                                                  double radius = ConvConsts.EarthRadiusM )
    {
      var δ13 = DistanceTo( pStartLat, pStartLon, thisLat, thisLon, radius ) / radius;
      var θ13 = BearingTo( pStartLat, pStartLon, thisLat, thisLon ).ToRadians( );
      var θ12 = BearingTo( pStartLat, pStartLon, pEndLat, pEndLon ).ToRadians( );

      var δxt = Math.Asin( Math.Sin( δ13 ) * Math.Sin( θ13 - θ12 ) );

      return δxt * radius;
    }


    /// <summary>
    /// Returns how far ‘this’ point is along a path from from start-point, heading towards end-point.
    /// That is, if a perpendicular is drawn from ‘this’ point to the( great circle ) path, the along-track
    /// distance is the distance from the start point to where the perpendicular crosses the path.
    /// </summary>
    /// <param name="thisLat">Latitude of starting point</param>
    /// <param name="thisLon">Longitude of starting point</param>
    /// <param name="pStartLat">Latitude of Start point of great circle path</param>
    /// <param name="pStartLon">Longitude of Start point of great circle path</param>
    /// <param name="pEndLat">Latitude of End point of great circle path</param>
    /// <param name="pEndLon">Longitude of End point of great circle path</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>{number} Distance along great circle to point nearest ‘this’ point.</returns>
    public static double AlongTrackDistanceTo( double thisLat, double thisLon,
                                            double pStartLat, double pStartLon,
                                            double pEndLat, double pEndLon,
                                            double radius = ConvConsts.EarthRadiusM )
    {
      var δ13 = DistanceTo( pStartLat, pStartLon, thisLat, thisLon, radius ) / radius;
      var θ13 = BearingTo( pStartLat, pStartLon, thisLat, thisLon ).ToRadians( );
      var θ12 = BearingTo( pStartLat, pStartLon, pEndLat, pEndLon ).ToRadians( );

      var δxt = Math.Asin( Math.Sin( δ13 ) * Math.Sin( θ13 - θ12 ) );

      var δat = Math.Acos( Math.Cos( δ13 ) / Math.Abs( Math.Cos( δxt ) ) );

      return δat * Math.Sign( Math.Cos( θ12 - θ13 ) ) * radius;
    }


    /// <summary>
    /// Returns maximum latitude reached when travelling on a great circle on given bearing from this
    /// point('Clairaut's formula'). Negate the result for the minimum latitude (in the Southern
    /// hemisphere).
    /// 
    /// The maximum latitude is independent of longitude; it will be the same for all points on a given
    /// latitude.
    /// </summary>
    /// <param name="thisLat">Latitude of starting point</param>
    /// <param name="bearing">{number} bearing - Initial bearing.</param>
    /// <returns>maximum latitude reached</returns>
    public static double MaxLatitude( double thisLat, double bearing )
    {
      var θ = bearing.ToRadians( );
      var φ = thisLat.ToRadians( );
      var φMax = Math.Acos( Math.Abs( Math.Sin( θ ) * Math.Cos( φ ) ) );

      return Wrap90( φMax.ToDegrees( ) );
    }


    /// <summary>
    /// Returns the pair of meridians at which a great circle defined by two points crosses the given
    /// latitude.If the great circle doesn't reach the given latitude, null is returned.
    /// </summary>
    /// <param name="p1Lat">Latitude of First point defining great circle</param>
    /// <param name="p1Lon">Longitude of First point defining great circle</param>
    /// <param name="p2Lat">Latitude of Second point defining great circle</param>
    /// <param name="p2Lon">Longitude of Second point defining great circle</param>
    /// <param name="latitude">{number} latitude - Latitude crossings are to be determined for.</param>
    /// <returns>{Object|null} Object containing { lon1, lon2 } or null if given latitude not reached.</returns>
    public static double[] CrossingParallels( double p1Lat, double p1Lon,
                                            double p2Lat, double p2Lon,
                                            double latitude )
    {
      var φ = latitude.ToRadians( );

      double φ1 = p1Lat.ToRadians( );
      double λ1 = p1Lon.ToRadians( );
      double φ2 = p2Lat.ToRadians( );
      double λ2 = p2Lon.ToRadians( );

      var Δλ = λ2 - λ1;

      var x = Math.Sin( φ1 ) * Math.Cos( φ2 ) * Math.Cos( φ ) * Math.Sin( Δλ );
      var y = Math.Sin( φ1 ) * Math.Cos( φ2 ) * Math.Cos( φ ) * Math.Cos( Δλ ) - Math.Cos( φ1 ) * Math.Sin( φ2 ) * Math.Cos( φ );
      var z = Math.Cos( φ1 ) * Math.Cos( φ2 ) * Math.Sin( φ ) * Math.Sin( Δλ );

      if (z * z > x * x + y * y) return null; // great circle doesn't reach latitude

      var λm = Math.Atan2( -y, x );                  // longitude at max latitude
      var Δλi = Math.Acos( z / Math.Sqrt( x * x + y * y ) ); // Δλ from λm to intersection points

      var λi1 = λ1 + λm - Δλi;
      var λi2 = λ1 + λm + Δλi;

      return new double[] { Wrap180( λi1.ToDegrees( ) ), Wrap180( λi2.ToDegrees( ) ) }; // normalise to −180..+180°
    }

    #region Track functions

    /// <summary>
    /// Returns true if a track is towards a station at bearing
    /// </summary>
    /// <param name="bearing_deg">The bearing to the station</param>
    /// <param name="track_deg">The current track</param>
    /// <returns>True if towards, else false (going away from)</returns>
    public static bool Towards( double bearing_deg, double track_deg )
    {
      // deviation of the two must be <=90°
      return Math.Abs( Wrap180( bearing_deg - track_deg ) ) <= 90.0;
    }

    /// <summary>
    /// Returns the heads on direction of a station at bearing with regards of a track
    /// 0 is straight ahead, neg. is left, pos. is right (+-180)
    /// </summary>
    /// <returns></returns>
    public static double DirectionOf( double bearing_deg, double track_deg )
    {
      return Wrap180( bearing_deg - track_deg );
    }

    #endregion

    #region Constraint functions

    private const double a_90 = 90;
    private const double a_180 = 180;
    private const double p_180 = 180;
    private const double p_360 = 360;

    /// <summary>
    /// Constrain degrees to range -90..+90 (for latitude); e.g. -91 => -89, 91 => 89.
    /// </summary>
    /// <param name="degrees">Latitude degrees</param>
    /// <returns>degrees within range -90..+90.</returns>
    public static double Wrap90( double degrees )
    {
      if (double.IsNaN( degrees )) return degrees;

      if (degrees >= -90 && degrees <= 90) return degrees; // avoid rounding due to arithmetic ops if within range

      // latitude wrapping requires a triangle wave function; a general triangle wave is
      //     f(x) = 4a/p ⋅ | (x-p/4)%p - p/2 | - a   // a=90, p=360
      // where a = amplitude, p = period, % = modulo; however, JavaScript '%' is a remainder operator (same in C#)
      // not a modulo operator - for modulo, replace 'x%n' with '((x%n)+n)%n'
      return 4 * a_90 / p_360 * Math.Abs( (((degrees - p_360 / 4) % p_360) + p_360) % p_360 - p_360 / 2 ) - a_90;
    }

    /// <summary>
    /// Constrain degrees to range -180..+180 (for longitude); e.g. -181 => 179, 181 => -179.
    /// </summary>
    /// <param name="degrees">Longitude degrees</param>
    /// <returns>degrees within range -180..+180.</returns>
    public static double Wrap180( double degrees )
    {
      if (double.IsNaN( degrees )) return degrees;

      if (degrees >= -180 && degrees <= 180) return degrees; // avoid rounding due to arithmetic ops if within range

      // longitude wrapping requires a sawtooth wave function; a general sawtooth wave is
      //     f(x) = (2ax/p - p/2) % p - a   // a=180, p=360
      // where a = amplitude, p = period, % = modulo; however, JavaScript '%' is a remainder operator (same in C#)
      // not a modulo operator - for modulo, replace 'x%n' with '((x%n)+n)%n'
      return (((2 * a_180 * degrees / p_360 - p_180) % p_360) + p_360) % p_360 - a_180;
    }

    /// <summary>
    /// Constrain degrees to range 0..360 (for bearings); e.g. -1 => 359, 361 => 1.
    /// </summary>
    /// <param name="degrees">Compass degrees (360°)</param>
    /// <returns>degrees within range 0..360.</returns>
    public static double Wrap360( double degrees )
    {
      if (double.IsNaN( degrees )) return degrees;

      if (degrees >= 0 && degrees < 360) return degrees; // avoid rounding due to arithmetic ops if within range

      // bearing wrapping requires a sawtooth wave function with a vertical offset equal to the
      // amplitude and a corresponding phase shift; this changes the general sawtooth wave function from
      //     f(x) = (2ax/p - p/2) % p - a
      // to
      //     f(x) = (2ax/p) % p   // a=360, p=180
      // where a = amplitude, p = period, % = modulo; however, JavaScript '%' is a remainder operator (same in C#)
      //  modulo and remainder are the same with positive argument, else see below
      // not a modulo operator - for modulo, replace 'x%n' with '((x%n)+n)%n' in case of negative degrees
      return (((2 * a_180 * degrees / p_360) % p_360) + p_360) % p_360;
    }

    /// <summary>
    /// Constrain degrees to range 0..360 (for bearings); e.g. -1 => 359, 361 => 1.
    /// </summary>
    /// <param name="degrees">Compass degrees (360°)</param>
    /// <returns>degrees within range 0..360.</returns>
    public static int Wrap360( int degrees )
    {
      if (degrees >= 0 && degrees < 360) return degrees; // avoid rounding due to arithmetic ops if within range

      return (int)Math.Round( Wrap360( (double)degrees ) );
    }

    /// <summary>
    /// Constrain degrees to range >0..360 for Aviation use (for bearings)
    /// e.g. -1 => 359, 361 => 1 and 000 is returned as 360
    /// </summary>
    /// <param name="degrees">Compass degrees (360°)</param>
    /// <returns>degrees within range >0..360.</returns>
    public static double Wrap360avi( double degrees )
    {
      if (double.IsNaN( degrees )) return degrees;
      if (degrees > 0 && degrees <= 360) return degrees; // in range

      double ret = Wrap360( degrees );
      return (ret == 0.0) ? 360.0 : ret;
    }

    /// <summary>
    /// Constrain degrees to range >0..360 for Aviation use (for bearings)
    /// e.g. -1 => 359, 361 => 1 and 000 is returned as 360
    /// </summary>
    /// <param name="degrees">Compass degrees</param>
    /// <returns>degrees within range 1..360.</returns>
    public static int Wrap360avi( int degrees )
    {
      if (degrees > 0 && degrees <= 360) return degrees; // in range

      int ret = Wrap360( degrees );
      return (ret == 0) ? 360 : ret;
    }

    #endregion

  }
}
