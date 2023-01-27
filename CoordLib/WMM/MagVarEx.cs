using CoordLib.UTMGrid;
using System;
using System.Collections.Generic;
using System.Text;

using CoordLib.Extensions;

namespace CoordLib.WMM
{
  /// <summary>
  /// Tooling to use the WMM library part
  /// All calculations are done at MagVar at 3km height
  ///   
  /// Extra MagVar Lookup Tables using the UTM Grid which might be faster than calculation
  /// but calculation is rather fast...
  /// </summary>
  public static class MagVarEx
  {
    // height where mag var is calculated
    private const double c_height_km = 3; //km

    // the model
    private static MagVar _wmm = new MagVar( );
    // magVar lookup table
    private static Dictionary<string, List<double>> _mvLookup_rad = new Dictionary<string, List<double>>( );

    // build the lookup table
    private static void BuildTable( )
    {
      foreach (var band in UtmOp.UTM_BandList) {
        _mvLookup_rad.Add( band, new List<double>( ) { 0 } ); // add a list with a zone 0 index)
        var list = _mvLookup_rad[band];
        foreach (var zone in UtmOp.UTM_ZoneList) {
          list.Add( MagVar_rad( zone, band ) ); // add zone 1.. magVar in Radians
        }
      }
    }

    /// <summary>
    /// static cTor:
    /// </summary>
    static MagVarEx( )
    {
      BuildTable( );
    }

    /// <summary>
    /// Returns the MagVar for the center of the UTM Cell at 3km Height
    /// </summary>
    /// <param name="utmZone">An UTM Zone Number</param>
    /// <param name="utmBand">A Band Letter</param>
    /// <returns>The MagVar</returns>
    private static double MagVar_rad( int utmZone, string utmBand )
    {
      var ll = UtmOp.UTMCellCenterCoord( utmZone, utmBand );
      if (ll.IsEmpty) return 0;

      return _wmm.SGMagVar( ll.Lat, ll.Lon, c_height_km, DateTime.Now, -1, new double[] { } );
    }

    /// <summary>
    /// Returns the MagVar for the center of the UTM Cell of a LatLon at 3km height
    /// </summary>
    /// <param name="latLon">A LatLon coordinate</param>
    /// <returns>The MagVar</returns>
    private static double MagVar_rad_UtmLookup( LatLon latLon )
    {
      // sanity
      if (latLon.IsEmpty) return 0;

      return _mvLookup_rad[latLon.UtmZoneLetter( )][latLon.UtmZoneNumber( )];
    }


    #region API

    /// <summary>
    /// Returns the Magnetic Declination at a location at 3km height [rad]
    /// </summary>
    /// <param name="latLon">The location</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic declination [rad]</returns>
    public static double MagVar_rad( LatLon latLon, bool useLookup )
    {
      // sanity
      if (latLon.IsEmpty) return 0;

      double magVar_rad;
      if (useLookup) {
        magVar_rad = MagVar_rad_UtmLookup( latLon );
      }
      else {
        magVar_rad = _wmm.SGMagVar( latLon.Lat, latLon.Lon, c_height_km, DateTime.Now, -1, new double[0] );
      }
      return magVar_rad;
    }

    /// <summary>
    /// Returns the Magnetic Declination at a location at 3km height [deg]
    /// </summary>
    /// <param name="latLon">The location</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic declination [deg]</returns>
    public static double MagVar_deg( LatLon latLon, bool useLookup )
    {
      return MagVar_rad( latLon, useLookup ).ToDegrees( );
    }


    /// <summary>
    /// Returns the Magnetic Bearing from a True Bearing at a location at 3km height [deg]
    /// </summary>
    /// <param name="trueBearing">The true bearing [deg]</param>
    /// <param name="latLon">The location</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic bearing [deg]</returns>
    public static double MagFromTrueBearing( double trueBearing, LatLon latLon, bool useLookup = false )
    {
      // sanity
      if (latLon.IsEmpty) return trueBearing;

      double magVar = MagVar_deg( latLon, useLookup );

      // correction is: add if West else sub
      if (latLon.Lon < 0) {
        return Geo.Wrap360( trueBearing + magVar );
      }
      else {
        return Geo.Wrap360( trueBearing - magVar );
      }
    }

    /// <summary>
    /// Returns the True Bearing from a Magnetic Bearing at a location at 3km height [deg]
    /// </summary>
    /// <param name="magBearing">The magnetic bearing [deg]</param>
    /// <param name="latLon">The location</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic bearing [deg]</returns>
    public static double TrueFromNagBearing( double magBearing, LatLon latLon, bool useLookup = false )
    {
      // sanity
      if (latLon.IsEmpty) return magBearing;

      double magVar = MagVar_deg( latLon, useLookup );

      // correction is: add if East else sub
      if (latLon.Lon > 0) {
        return Geo.Wrap360( magBearing + magVar );
      }
      else {
        return Geo.Wrap360( magBearing - magVar );
      }
    }

    #endregion

  }
}
