using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

using static CoordLib.MercatorTiles.TileOp;
using static CoordLib.MercatorTiles.Projection;

namespace CoordLib.MercatorTiles
{
  /// <summary>
  /// Quad Tree Helper
  /// Based on the Mercator Projection Grid with a Quad Square Tile of 256x256
  /// see also: https://docs.microsoft.com/en-us/bingmaps/articles/bing-maps-tile-system
  ///  
  /// </summary>
  internal static class QuadOp
  {
    // 1..22 digits out of 0,1,2,3 nothing else
    static Regex _qs = new Regex( @"^[0123]{1,22}$", RegexOptions.Compiled | RegexOptions.Singleline );

    /// <summary>
    /// Checks a Quad for validity
    /// </summary>
    /// <param name="quad">A Quad string</param>
    /// <returns>True if valid</returns>
    public static bool CheckQuad( string quad )
    {
      if (string.IsNullOrEmpty( quad )) return true;  // an empty one counts as valid input (but is not a Quad..)

      Match match = _qs.Match( quad );
      return match.Success;
    }

    /// <summary>
    /// returns the ZoomLevel of the argument
    /// </summary>
    public static ushort ZoomLevel( string quad ) => (ushort)quad.Length; // it is the number of chars in the Quad ..

    /// <summary>
    /// Returns a reduced Quad at the desired zoom
    /// If reduction not possible it returns the argument
    /// </summary>
    /// <param name="quad">A quad</param>
    /// <param name="zoom">Zoomlevel 1..23 </param>
    /// <returns>A Quad Key (empty if zoom=0)</returns>
    public static string QuadAtZoom( string quad, ushort zoom )
    {
      // sanity
      if (quad.Length <= zoom) return quad; // cannot reduce
      if (zoom < 1) return "";

      return quad.Substring( 0, (int)zoom );
    }

    /// <summary>
    /// Returns a Quad as String
    /// </summary>
    /// <param name="latLon">Coordinate</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>A Quadkey as String</returns>
    public static string LatLonToQuad( LatLon latLon, ushort zoom )
    {
      // sanity
      if (zoom < MinZoom || zoom > MaxZoom) return ""; // an empty string

      return TileXYToQuad( LatLonToTileXY( latLon, zoom ), zoom );
    }

    /// <summary>
    /// Returns a Quad as String
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>A Quadkey as String</returns>
    public static string LatLonToQuad( double lat, double lon, ushort zoom )
    {
      return LatLonToQuad( new LatLon( lat, lon ), zoom );
    }

    /// <summary>
    /// Converts TileXY coordinates into a QuadKey at a specified level of detail.
    /// see also: https://docs.microsoft.com/en-us/bingmaps/articles/bing-maps-tile-system
    /// </summary>
    /// <param name="tileXY">Tile XY coordinate.</param>
    /// <param name="zoom">
    ///     Level of detail, from 1 (lowest detail)
    ///     to 22 (highest detail).
    /// </param>
    /// <returns>A string containing the QuadKey.</returns>
    public static string TileXYToQuad( Point tileXY, ushort zoom )
    {
      // sanity.. 
      if (tileXY.X < 0 || tileXY.Y < 0) return "";
      if (zoom < MinZoom || zoom > MaxZoom) return "";
      var quadKey = new StringBuilder( );
      for (int i = (int)zoom; i > 0; i--) {
        char digit = '0';
        int mask = 1 << (i - 1);
        if ((tileXY.X & mask) != 0) { digit++; }
        if ((tileXY.Y & mask) != 0) { digit++; digit++; }
        quadKey.Append( digit );
      }
      return quadKey.ToString( );
    }


    //   Quadkey digits are base 4 interleaves of the base 2 bits of the X,Y coords

    /// <summary>
    /// Returns the QuadKey of the zoomed out level of the argument
    ///  if at top returns an empty one (whole Map at Zoom 0)
    /// </summary>
    /// <param name="quadKey">A Quad Key</param>
    /// <returns>A Quad Key</returns>
    public static string HigherLevel( string quadKey )
    {
      // sanity
      if (quadKey.Length < 2) return "";

      return quadKey.Substring( 0, quadKey.Length - 1 );
    }
    /// <summary>
    /// Returns the rightmost Quad Item (character)
    ///  if at top returns an empty one (whole Map at Zoom 0)
    /// </summary>
    /// <param name="quadKey">A Quad Key</param>
    /// <returns>The rightmost Quad Digit as string</returns>
    public static string LastQ( string quadKey )
    {
      // sanity
      if (string.IsNullOrWhiteSpace( quadKey )) return "";

      return quadKey.Substring( quadKey.Length - 1, 1 );
    }

    /// <summary>
    /// Get the QuadKey left of the current one
    /// </summary>
    /// <param name="quadKey">A Quad Key</param>
    /// <returns>A Quad Key</returns>
    public static string LeftQ( string quadKey )
    {
      // this is recursive - we need a stop condition
      // at Zoom 1 there is no upper level anymore
      if (string.IsNullOrWhiteSpace( quadKey )) return ""; // this will wrap around

      var last = LastQ( quadKey );
      var high = HigherLevel( quadKey );
      if (last == "0") return LeftQ( high ) + "1";
      else if (last == "1") return high + "0";
      else if (last == "2") return LeftQ( high ) + "3";
      else if (last == "3") return high + "2";
      return "";  // should not...
    }

    /// <summary>
    /// Get the QuadKey right of the current one
    /// </summary>
    /// <param name="quadKey">A Quad Key</param>
    /// <returns>A Quad Key</returns>
    public static string RightQ( string quadKey )
    {
      // this is recursive - we need a stop condition
      // at Zoom 1 there is no upper level anymore
      if (string.IsNullOrWhiteSpace( quadKey )) return ""; // this will wrap around

      var last = LastQ( quadKey );
      var high = HigherLevel( quadKey );
      if (last == "0") return high + "1";
      else if (last == "1") return RightQ( high ) + "0";
      else if (last == "2") return high + "3";
      else if (last == "3") return RightQ( high ) + "2";
      return ""; // should not...
    }

    /// <summary>
    /// Get the QuadKey above of the current one
    /// </summary>
    /// <param name="quadKey">A Quad Key</param>
    /// <returns>A Quad Key</returns>
    public static string AboveQ( string quadKey )
    {
      // this is recursive - we need a stop condition
      // at Zoom 1 there is no upper level anymore
      if (string.IsNullOrWhiteSpace( quadKey )) return ""; // this will wrap around

      var last = LastQ( quadKey );
      var high = HigherLevel( quadKey );
      if (last == "0") return AboveQ( high ) + "2";
      else if (last == "1") return AboveQ( high ) + "3";
      else if (last == "2") return high + "0";
      else if (last == "3") return high + "1";
      return ""; // should not...
    }

    /// <summary>
    /// Get the QuadKey below of the current one
    /// </summary>
    /// <param name="quadKey">A Quad Key</param>
    /// <returns>A Quad Key</returns>
    public static string BelowQ( string quadKey )
    {
      // this is recursive - we need a stop condition
      // at Zoom 1 there is no upper level anymore
      if (string.IsNullOrWhiteSpace( quadKey )) return ""; // this will wrap around

      var last = LastQ( quadKey );
      var high = HigherLevel( quadKey );
      if (last == "0") return high + "2";
      else if (last == "1") return high + "3";
      else if (last == "2") return BelowQ( high ) + "0";
      else if (last == "3") return BelowQ( high ) + "1";
      return ""; // should not...
    }



    /// <summary>
    /// Returns 4 Quads that are the closest neighbours of the argument
    /// </summary>
    /// <param name="mapPixel">A MapPixel</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static string[] Around( Point mapPixel, ushort zoom )
    {
      string[] ret = new string[4];
      ret[0] = TileXYToQuad( MapPixelToTileXY( mapPixel ), zoom ); // self
      var quadrant = QuadrantFromMapPixel( mapPixel );

      switch (quadrant) {
        case TileQuadrant.LeftTop:
          ret[1] = LeftQ( ret[0] );  // left
          ret[2] = AboveQ( ret[1] ); // left up
          ret[3] = AboveQ( ret[0] ); // up
          break;
        case TileQuadrant.RightTop:
          ret[1] = RightQ( ret[0] ); // right
          ret[2] = AboveQ( ret[1] ); // right up
          ret[3] = AboveQ( ret[0] );// up
          break;
        case TileQuadrant.RightBottom:
          ret[1] = RightQ( ret[0] ); // right
          ret[2] = BelowQ( ret[1] ); // right down
          ret[3] = BelowQ( ret[0] );// down
          break;
        case TileQuadrant.LeftBottom:
          ret[1] = LeftQ( ret[0] ); //left
          ret[2] = BelowQ( ret[1] ); // left down
          ret[3] = BelowQ( ret[0] ); // down
          break;
        default: break;
      }

      return ret;
    }
    /// <summary>
    /// Returns 4 Quads that are the closest neighbours of the argument
    /// </summary>
    /// <param name="latLon">A LatLon</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static string[] Around( LatLon latLon, ushort zoom )
    {
      return Around( LatLonToMapPixel( latLon, zoom ), zoom );
    }

    /// <summary>
    /// Returns 4 Quads that are the closest neighbours of the argument
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static string[] Around( double lat, double lon, ushort zoom )
    {
      return Around( new LatLon( lat, lon ), zoom );
    }

  }
}
