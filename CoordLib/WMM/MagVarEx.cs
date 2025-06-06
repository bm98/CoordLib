﻿using System;
using System.Collections.Generic;

using CoordLib.Extensions;
using CoordLib.MercatorTiles;
using CoordLib.UTMGrid;

namespace CoordLib.WMM
{
  /// <summary>
  /// Tooling to use the WMM library part
  /// All calculations are done at MagVar at 3km height
  ///   
  /// Extra MagVar Lookup Tree using a Quad Grid at level 9 which is about 4 times faster than calculation
  /// but calculation is also rather fast...
  /// 
  ///  Lookup 100_000x => 225 ms
  ///  Calc   100_000x => 1055 ms
  /// 
  /// Lookup values are only calculated once per cell if first time requested
  /// 
  /// </summary>
  public static class MagVarEx
  {
    // height where mag var is calculated
    private const double c_height_km = 3; //km

    // the model
    private readonly static MagVar _wmm = new MagVar( );

    // magVar lookup table Quad
    private static QuadLookup<double?> _quadLookup;
    private const ushort c_qtZoom = 9; // L9 -> squares of 78x78 km at lat==0

    // magVar lookup table UTM - NOT IN USE - too coarse
    private static Dictionary<string, List<double>> _mvLookup_rad = new Dictionary<string, List<double>>( );
    // build the UTM lookup table
    private static void BuildUTMTable( )
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
      // UTM based lookup
      // BuildUTMTable( );

      // Quad based lookup
      _quadLookup = new QuadLookup<double?>( 3, c_qtZoom - 1, 64 );
    }

    /// <summary>
    /// Returns the MagVar for the center of the UTM Cell at 3km Height
    /// </summary>
    /// <param name="utmZone">An UTM Zone Number</param>
    /// <param name="utmBand">A Band Letter</param>
    /// <param name="date">A date to calculate with</param>
    /// <returns>The MagVar</returns>
    private static double MagVar_rad( int utmZone, string utmBand, DateTime date )
    {
      var ll = UtmOp.UTMCellCenterCoord( utmZone, utmBand );
      if (ll.IsEmpty) return 0;

      return _wmm.SGMagVar( ll.Lat, ll.Lon, c_height_km, date, -1, new double[0] { } );
    }

    /// <summary>
    /// Returns the MagVar for the center of the UTM Cell at 3km Height
    /// </summary>
    /// <param name="utmZone">An UTM Zone Number</param>
    /// <param name="utmBand">A Band Letter</param>
    /// <returns>The MagVar</returns>
    private static double MagVar_rad( int utmZone, string utmBand )
    {
      return MagVar_rad( utmZone, utmBand, DateTime.Now );
    }

    /// <summary>
    /// Returns the Magnetic Declination at a location at 3km height [rad]
    /// </summary>
    /// <param name="latLon">A LatLon coordinate</param>
    /// <param name="date">A date to calculate with</param>
    /// <returns>The MagVar</returns>
    private static double MagVar_rad_Lookup( LatLon latLon, DateTime date )
    {
      // sanity
      if (latLon.IsEmpty) return 0;

      double magVar_rad;

      // UTM based lookup
      // Returns the MagVar for the center of the UTM Cell of a LatLon at 3km height
      //magVar_rad = _mvLookup_rad[latLon.UtmZoneLetter( )][latLon.UtmZoneNumber( )];

      // Quad based lookup
      // Returns the MagVar for the center LatLon of a Quad containing the input coord at 3km height
      var quad = latLon.AsQuad( c_qtZoom );

      // support threaded access
      lock (_quadLookup) {
        var mvl = _quadLookup.GetItemWhichIncludes( quad );
        if (mvl.HasValue) {
          // found
          magVar_rad = mvl.Value;
        }
        else {
          // not found, calculate and add
          var center = quad.Center( );
          lock (_wmm) {
            magVar_rad = _wmm.SGMagVar( center.Lat, center.Lon, c_height_km, date, -1, new double[0] );
          }
          _quadLookup.Add( quad, magVar_rad );
        }
      }
      // finally
      return magVar_rad;
    }


    #region API

    /// <summary>
    /// Returns the Magnetic Declination at a location at 3km height [rad]
    /// </summary>
    /// <param name="latLon">The location</param>
    /// <param name="date">A date to calculate with</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic declination [rad]</returns>
    public static double MagVar_rad( LatLon latLon, DateTime date, bool useLookup )
    {
      // sanity
      if (latLon.IsEmpty) return 0;

      double magVar_rad;
      if (useLookup) {
        magVar_rad = MagVar_rad_Lookup( latLon, date );
      }
      else {
        lock (_wmm) {
          magVar_rad = _wmm.SGMagVar( latLon.Lat, latLon.Lon, c_height_km, date, -1, new double[0] );
        }
      }
      return magVar_rad;
    }
    /// <summary>
    /// Returns the Magnetic Declination at a location at 3km height [rad]
    /// using Now as date
    /// </summary>
    /// <param name="latLon">The location</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic declination [rad]</returns>
    public static double MagVar_rad( LatLon latLon, bool useLookup )
    {
      return MagVar_rad( latLon, DateTime.Now, useLookup );
    }

    /// <summary>
    /// Returns the Magnetic Declination at a location at 3km height [deg]
    /// </summary>
    /// <param name="latLon">The location</param>
    /// <param name="date">A date to calculate with</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic declination [deg]</returns>
    public static double MagVar_deg( LatLon latLon, DateTime date, bool useLookup )
    {
      return MagVar_rad( latLon, date, useLookup ).ToDegrees( );
    }

    /// <summary>
    /// Returns the Magnetic Declination at a location at 3km height [deg]
    /// using Now as date
    /// </summary>
    /// <param name="latLon">The location</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic declination [deg]</returns>
    public static double MagVar_deg( LatLon latLon, bool useLookup )
    {
      return MagVar_deg( latLon, DateTime.Now, useLookup );
    }


    /// <summary>
    /// Returns the Magnetic Bearing from a True Bearing at a location at 3km height [deg]
    /// </summary>
    /// <param name="trueBearing">The true bearing [deg]</param>
    /// <param name="latLon">The location</param>
    /// <param name="date">A date to calculate with</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic bearing [deg]</returns>
    public static double MagFromTrueBearing( double trueBearing, LatLon latLon, DateTime date, bool useLookup = false )
    {
      // sanity
      if (double.IsNaN( trueBearing )) return double.NaN;
      if (latLon.IsEmpty) return trueBearing;

      double magVar = MagVar_deg( latLon, date, useLookup );

      // correction is: add if West(neg magVar) else sub
      return Geo.Wrap360( trueBearing - magVar );
    }

    /// <summary>
    /// Returns the Magnetic Bearing from a True Bearing at a location at 3km height [deg]
    /// using Now as date
    /// </summary>
    /// <param name="trueBearing">The true bearing [deg]</param>
    /// <param name="latLon">The location</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic bearing [deg]</returns>
    public static double MagFromTrueBearing( double trueBearing, LatLon latLon, bool useLookup = false )
    {
      return MagFromTrueBearing( trueBearing, latLon, DateTime.Now, useLookup );
    }

    /// <summary>
    /// Returns the Magnetic Bearing from a True Bearing using the MagVar at the intermediate point of loc1 and loc2 at 3km height [deg]
    /// </summary>
    /// <param name="trueBearing">The true bearing [deg]</param>
    /// <param name="latLon1">The location (from)</param>
    /// <param name="latLon2">The location (to)</param>
    /// <param name="date">A date to calculate with</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic bearing [deg]</returns>
    public static double MagFromTrueBearing( double trueBearing, LatLon latLon1, LatLon latLon2, DateTime date, bool useLookup = false )
    {
      // sanity
      if (double.IsNaN( trueBearing )) return double.NaN;
      if (latLon1.IsEmpty) return trueBearing;
      if (latLon2.IsEmpty) return trueBearing;

      var llIntermediate = latLon1.IntermediatePointTo( latLon2, 0.5 );
      double magVar = MagVar_deg( llIntermediate, useLookup );
      // take ( start + mid + end ) / 3 as magVar
      double mgCalc = (magVar + MagVar_deg( latLon1, date, useLookup ) + MagVar_deg( latLon2, date, useLookup )) / 3.0;

      // correction is: add if West(neg magVar) else sub
      return Geo.Wrap360( trueBearing - mgCalc );
    }

    /// <summary>
    /// Returns the Magnetic Bearing from a True Bearing using the MagVar at the intermediate point of loc1 and loc2 at 3km height [deg]
    /// using Now as date
    /// </summary>
    /// <param name="trueBearing">The true bearing [deg]</param>
    /// <param name="latLon1">The location (from)</param>
    /// <param name="latLon2">The location (to)</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic bearing [deg]</returns>
    public static double MagFromTrueBearing( double trueBearing, LatLon latLon1, LatLon latLon2, bool useLookup = false )
    {
      return MagFromTrueBearing( trueBearing, latLon1, latLon2, DateTime.Now, useLookup );
    }


    /// <summary>
    /// Returns the True Bearing from a Magnetic Bearing at a location at 3km height [deg]
    /// </summary>
    /// <param name="magBearing">The magnetic bearing [deg]</param>
    /// <param name="latLon">The location</param>
    /// <param name="date">A date to calculate with</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic bearing [deg]</returns>
    public static double TrueFromMagBearing( double magBearing, LatLon latLon, DateTime date, bool useLookup = false )
    {
      // sanity
      if (double.IsNaN( magBearing )) return double.NaN;
      if (latLon.IsEmpty) return magBearing;

      double magVar = MagVar_deg( latLon, date, useLookup );

      // correction is: add if East(pos magVar) else sub
      // correction is: add if East else sub
      return Geo.Wrap360( magBearing + magVar );
    }

    /// <summary>
    /// Returns the True Bearing from a Magnetic Bearing at a location at 3km height [deg]
    /// using Now as date
    /// </summary>
    /// <param name="magBearing">The magnetic bearing [deg]</param>
    /// <param name="latLon">The location</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic bearing [deg]</returns>
    public static double TrueFromMagBearing( double magBearing, LatLon latLon, bool useLookup = false )
    {
      return TrueFromMagBearing( magBearing, latLon, DateTime.Now, useLookup );
    }

    /// <summary>
    /// Returns the True Bearing from a Magnetic Bearing using the MagVar at the intermediate point of loc1 and loc2 at 3km height [deg]
    /// </summary>
    /// <param name="magBearing">The magnetic bearing [deg]</param>
    /// <param name="latLon1">The location (from)</param>
    /// <param name="latLon2">The location (to)</param>
    /// <param name="date">A date to calculate with</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic bearing [deg]</returns>
    public static double TrueFromMagBearing( double magBearing, LatLon latLon1, LatLon latLon2, DateTime date, bool useLookup = false )
    {
      // sanity
      if (double.IsNaN( magBearing )) return double.NaN;
      if (latLon1.IsEmpty) return magBearing;
      if (latLon2.IsEmpty) return magBearing;

      var llIntermediate = latLon1.IntermediatePointTo( latLon2, 0.5 );
      double magVar = MagVar_deg( llIntermediate, useLookup );
      // take ( start + mid + end ) / 3 as magVar
      double mgCalc = (magVar + MagVar_deg( latLon1, date, useLookup ) + MagVar_deg( latLon2, date, useLookup )) / 3.0;

      // correction is: add if East(pos magVar) else sub
      // correction is: add if East else sub
      return Geo.Wrap360( magBearing + mgCalc );
    }

    /// <summary>
    /// Returns the True Bearing from a Magnetic Bearing using the MagVar at the intermediate point of loc1 and loc2 at 3km height [deg]
    /// using Now as date
    /// </summary>
    /// <param name="magBearing">The magnetic bearing [deg]</param>
    /// <param name="latLon1">The location (from)</param>
    /// <param name="latLon2">The location (to)</param>
    /// <param name="useLookup">When true using the UTM lookup table</param>
    /// <returns>The magnetic bearing [deg]</returns>
    public static double TrueFromMagBearing( double magBearing, LatLon latLon1, LatLon latLon2, bool useLookup = false )
    {
      return TrueFromMagBearing( magBearing, latLon1, latLon2, DateTime.Now, useLookup );
    }

    #endregion

  }
}
