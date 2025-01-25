using System;

using CoordLib.Extensions;

namespace CoordLib
{
  /// <summary>
  /// Useful conversions and constants
  /// </summary>
  public static class ConvConsts
  {
    /// <summary>
    /// Const Km per Sm (Statute Mile)
    /// </summary>
    public const double KmPerSm = 1.609344;
    /// <summary>
    /// Const Km per Nm
    /// </summary>
    public const double KmPerNm = 1.852;
    /// <summary>
    /// Const M per Ft
    /// </summary>
    public const double MPerFt = 0.3048;

    /// <summary>
    /// Const Earth radius in M
    /// </summary>
    public const double EarthRadiusM = 6378137.0; //20220726: set exact number
    /// <summary>
    /// Const Earth Flattening for the WGS 84 spheroid
    /// </summary>
    public const double EarthFlattening_WGS84 = 1.0 / 298.257223563;
    /// <summary>
    /// Const Earth radius in km
    /// </summary>
    public const double EarthRadiusKm = EarthRadiusM / 1000.0;
    /// <summary>
    /// Const Earth radius in Nm
    /// </summary>
    public const double EarthRadiusNm = EarthRadiusKm / KmPerNm;
    /// <summary>
    /// Const Earth radius in SM
    /// </summary>
    public const double EarthRadiusSM = EarthRadiusKm / KmPerSm;

    /// <summary>
    /// Latitude Distance per Degree [m]
    /// </summary>
    public const double LatDistPerDeg_m = 111_319.5;
    /// <summary>
    /// Latitude Distance per Degree, default [m]
    ///  use earthRadius to change the unit
    /// </summary>
    /// <param name="earthRadius">Earth radius, default [m]</param>
    public static double LatDistPerDeg( double earthRadius = EarthRadiusM )
    {
      return (LatDistPerDeg_m / EarthRadiusM * earthRadius);
    }
    /// <summary>
    /// Longitude Distance per Degree at a Latitude, default [m]
    ///  use earthRadius to change the unit
    /// </summary>
    /// <param name="atLatitude">Latitude [deg]</param>
    /// <param name="earthRadius">Earth radius, default [m]</param>
    public static double LonDistPerDeg( double atLatitude, double earthRadius = EarthRadiusM )
    {
      var lat = Geo.Wrap90( atLatitude );
      // at 90 the distance gets 0 i.e. no longer defined
      var absLat = Math.Abs( lat );
      if ( absLat >= 89.9) { 
        absLat= double.NegativeInfinity; }
      return Math.Cos( absLat.ToRadians( ) ) * LatDistPerDeg( earthRadius );
    }

    /// <summary>
    /// Latitude Degrees for a distance, default [m]
    /// </summary>
    /// <param name="distance">The distance in earthRadius base units</param>
    /// <param name="earthRadius">Earth radius, default [m]</param>
    public static double LatDegPerDist( double distance, double earthRadius = EarthRadiusM )
    {
      return distance / LatDistPerDeg( earthRadius );
    }
    /// <summary>
    /// Longitude Degrees for a distance at a Latitude, default [m]
    /// </summary>
    /// <param name="distance">The distance in earthRadius base units</param>
    /// <param name="atLatitude"></param>
    /// <param name="earthRadius">Earth radius, default [m]</param>
    public static double LonDegPerDist( double distance, double atLatitude, double earthRadius = EarthRadiusM )
    {
      return distance / LonDistPerDeg( atLatitude, earthRadius );
    }

    /// <summary>
    /// Convert m to nm
    /// </summary>
    /// <param name="m">m to convert</param>
    /// <returns>nm</returns>
    public static double MToNm( double m )
    {
      if (m == double.NaN) return double.NaN;  // sanity
      return ((m / 1000.0) / KmPerNm);
    }

    /// <summary>
    /// Convert m to Sm
    /// </summary>
    /// <param name="m">m to convert</param>
    /// <returns>Sm</returns>
    public static double MToSm( double m )
    {
      if (m == double.NaN) return double.NaN;  // sanity
      return ((m / 1000.0) / KmPerSm);
    }

    /// <summary>
    /// Convert km to nm
    /// </summary>
    /// <param name="km">km to convert</param>
    /// <returns>nm</returns>
    public static double KmToNm( double km )
    {
      if (km == double.NaN) return double.NaN;  // sanity
      return (km / KmPerNm);
    }

    /// <summary>
    /// Convert km to Sm
    /// </summary>
    /// <param name="km">km to convert</param>
    /// <returns>Sm</returns>
    public static double KmToSm( double km )
    {
      if (km == double.NaN) return double.NaN;  // sanity
      return (km / KmPerSm);
    }

    /// <summary>
    /// Convert nm to m
    /// </summary>
    /// <param name="nm">nm to convert</param>
    /// <returns>m</returns>
    public static double NmToM( double nm )
    {
      if (nm == double.NaN) return double.NaN;  // sanity
      return (nm * KmPerNm * 1000.0);
    }

    /// <summary>
    /// Convert Sm to m
    /// </summary>
    /// <param name="sm">Sm to convert</param>
    /// <returns>m</returns>
    public static double SmToM( double sm )
    {
      if (sm == double.NaN) return double.NaN;  // sanity
      return (sm * KmPerSm * 1000.0);
    }

    /// <summary>
    /// Convert nm to km
    /// </summary>
    /// <param name="nm">nm to convert</param>
    /// <returns>km</returns>
    public static double NmToKm( double nm )
    {
      if (nm == double.NaN) return double.NaN;  // sanity
      return (nm * KmPerNm);
    }

    /// <summary>
    /// Convert Sm to km
    /// </summary>
    /// <param name="sm">Sm to convert</param>
    /// <returns>km</returns>
    public static double SmToKm( double sm )
    {
      if (sm == double.NaN) return double.NaN;  // sanity
      return (sm * KmPerSm);
    }

    /// <summary>
    /// Convert m to ft
    /// </summary>
    /// <param name="m">m to convert</param>
    /// <returns>ft</returns>
    public static double MToFt( double m )
    {
      if (m == double.NaN) return double.NaN;  // sanity
      return (m / MPerFt);
    }
    /// <summary>
    /// Convert ft to m
    /// </summary>
    /// <param name="ft">ft to convert</param>
    /// <returns>m</returns>
    public static double FtToM( double ft )
    {
      if (ft == double.NaN) return double.NaN;  // sanity
      return (ft * MPerFt);
    }

    /// <summary>
    /// Convert Radians to Degrees
    /// </summary>
    /// <param name="angleInDegree">An angle in degrees</param>
    /// <returns>The angle in radians</returns>
    public static double ToRadians( double angleInDegree )
    {
      return angleInDegree.ToRadians( );
    }

    /// <summary>
    /// Convert Degrees to Radians
    /// </summary>
    /// <param name="angleInRadians">An angle in radians</param>
    /// <returns>The angle in degrees</returns>
    public static double ToDegrees( double angleInRadians )
    {
      return angleInRadians.ToDegrees( );
    }


  }
}
