﻿using System;
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
    static readonly double R2D = 180 / PI;
    static readonly double D2R = PI / 180;

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


  }
}