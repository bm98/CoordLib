using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using static System.Math;
using CoordLib.MercatorTiles;

namespace CoordLib.Extensions
{
  /// <summary>
  /// CoordLib Extensions available via
  /// 
  ///   using CoordLib.Extensions;
  ///   
  /// 
  /// </summary>
  public static class Extensions
  {
    static readonly double R2D = 180.0 / PI;
    static readonly double D2R = PI / 180.0;

    /// <summary>
    /// True for valid Latitudes (+-90°)
    /// </summary>
    public static bool IsValidLat( this double lat ) => lat >= -90 && lat <= 90;

    /// <summary>
    /// True for valid Longitudes (+-180°)
    /// </summary>
    public static bool IsValidLon( this double lon ) => lon >= -180 && lon <= 180;

    /// <summary>
    /// Returns the angle in radians
    /// </summary>
    public static float ToRadians( this float angleInDegree )
    {
      return (float)(angleInDegree * D2R);
    }
    /// <summary>
    /// Returns the angle in Degrees
    /// </summary>
    public static float ToDegrees( this float angleInRadians )
    {
      return (float)(angleInRadians * R2D);
    }

    /// <summary>
    /// Returns the angle in radians
    /// </summary>
    public static double ToRadians( this double angleInDegree )
    {
      return angleInDegree * D2R;
    }
    /// <summary>
    /// Returns the angle in Degrees
    /// </summary>
    public static double ToDegrees( this double angleInRadians )
    {
      return angleInRadians * R2D;
    }

    /// <summary>
    /// Clips a number to the specified minimum and maximum values.
    /// Returns the clipped value
    /// </summary>
    /// <param name="_d"></param>
    /// <param name="minValue">Minimum allowable value.</param>
    /// <param name="maxValue">Maximum allowable value.</param>
    /// <returns>The clipped value.</returns>
    public static double Clip( this double _d, double minValue, double maxValue )
    {
      return Min( Max( _d, minValue ), maxValue );
    }

    /// <summary>
    /// Clips a number to the specified minimum and maximum values.
    /// Returns the clipped value
    /// </summary>
    /// <param name="_i"></param>
    /// <param name="minValue">Minimum allowable value.</param>
    /// <param name="maxValue">Maximum allowable value.</param>
    /// <returns>The clipped value.</returns>
    public static float Clip( this float _i, double minValue, double maxValue )
    {
      return (float)Min( Max( _i, minValue ), maxValue );
    }

    /// <summary>
    /// Clips a number to the specified minimum and maximum values.
    /// Returns the clipped value
    /// </summary>
    /// <param name="_i"></param>
    /// <param name="minValue">Minimum allowable value.</param>
    /// <param name="maxValue">Maximum allowable value.</param>
    /// <returns>The clipped value.</returns>
    public static double Clip( this int _i, double minValue, double maxValue )
    {
      return Min( Max( _i, minValue ), maxValue );
    }

    /// <summary>
    /// Clips a number to the specified minimum and maximum values.
    /// Returns the clipped value
    /// </summary>
    /// <param name="_i"></param>
    /// <param name="minValue">Minimum allowable value.</param>
    /// <param name="maxValue">Maximum allowable value.</param>
    /// <returns>The clipped value.</returns>
    public static int Clip( this int _i, int minValue, int maxValue )
    {
      return Min( Max( _i, minValue ), maxValue );
    }


    /// <summary>
    /// Returns the Point as MapPixel Type
    /// </summary>
    public static MapPixel AsMapPixel( this Point _p ) => new MapPixel( _p );

    /// <summary>
    /// Returns a Point as TileXY Type
    /// </summary>
    public static TileXY AsTileXY( this Point _p ) => new TileXY( _p );



    /// <summary>
    /// Returns a Quad @ Zoom Level
    /// </summary>
    public static Quad AsQuad( this LatLon _ll, ushort zoomLevel ) => new Quad( _ll, zoomLevel );

    /// <summary>
    /// Returns a Quad @ max Zoom Level
    /// </summary>
    public static Quad AsQuadMax( this LatLon _ll ) => new Quad( _ll, Projection.MaxZoom );



    /// <summary>
    /// Returns the UtmZone Designator NNC 
    /// </summary>
    public static string UtmZone( this LatLon _ll ) => UTMGrid.UtmOp.UtmZone( _ll );

    /// <summary>
    /// Returns the UTM Longitude Zone Number
    /// </summary>
    public static int UtmZoneNumber( this LatLon _ll ) => UTMGrid.UtmOp.UtmZoneNo( _ll );

    /// <summary>
    /// Returns the UTM Latitude Zone Letter
    /// </summary>
    public static string UtmZoneLetter( this LatLon _ll ) => UTMGrid.UtmOp.UtmLetterDesignator( _ll );


    /// <summary>
    /// Returns the Magnetic Variation (declinattion) at this location 
    /// using WMM2020 (valid 2020..2025)
    /// </summary>
    public static double MagVarCalc_deg( this LatLon _ll ) => WMM.MagVarEx.MagVar_deg( _ll, false );

    /// <summary>
    /// Returns the Magnetic Variation (declinattion) at this location 
    /// using the Lookup table/tree
    /// using WMM2020 (valid 2020..2025)
    /// </summary>
    public static double MagVarLookup_deg( this LatLon _ll ) => WMM.MagVarEx.MagVar_deg( _ll, true );

  }
}
