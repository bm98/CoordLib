using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using static System.Math;
using CoordLib.MercatorTiles;

namespace CoordLib
{
  /// <summary>
  /// Helper Extensions 
  /// </summary>
  internal static class Extensions
  {
    static readonly double R2D = 180 / PI;
    static readonly double D2R = PI / 180;

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
      return (int)Min( Max( _i, minValue ), maxValue );
    }

    public static MapPixel AsMapPixel( this Point _p ) => new MapPixel( _p );
    public static TileXY AsTileXY( this Point _p ) => new TileXY( _p );
  }
}
