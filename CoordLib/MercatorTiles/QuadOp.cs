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
  /// Internal Quad Tree Helper based on:
  ///   Quad strings as in/output (quadKey)
  ///   Zoom is always ushort
  /// 
  /// Based on the Mercator Projection Grid with a Quad Square Tile of 256x256
  /// see also: https://docs.microsoft.com/en-us/bingmaps/articles/bing-maps-tile-system
  ///  
  /// </summary>
  internal static class QuadOp
  {
    // 1..22 digits out of 0,1,2,3 nothing else
    private static Regex _qs = new Regex( @"^[0123]{1,22}$", RegexOptions.Compiled | RegexOptions.Singleline );

    /// <summary>
    /// Checks a Quad for validity
    /// </summary>
    /// <param name="quadKey">A Quad string</param>
    /// <returns>True if valid</returns>
    public static bool CheckQuad( string quadKey )
    {
      if (string.IsNullOrEmpty( quadKey )) return true;  // an empty one counts as valid input (but is not a Quad..)

      Match match = _qs.Match( quadKey );
      return match.Success;
    }

    /// <summary>
    /// returns the ZoomLevel of the argument
    /// </summary>
    public static ushort ZoomLevel( string quadKey ) => (ushort)quadKey.Length; // it is the number of chars in the Quad ..

    /// <summary>
    /// Returns a reduced Quad at the desired zoom
    /// If reduction not possible it returns the argument
    /// </summary>
    /// <param name="quadKey">A quadKey</param>
    /// <param name="zoom">Zoomlevel 1..23 </param>
    /// <returns>A Quad Key (empty if zoom=0)</returns>
    public static string QuadAtZoom( string quadKey, ushort zoom )
    {
      // sanity
      if (quadKey.Length <= zoom) return quadKey; // cannot reduce
      if (zoom < 1) return "";

      return quadKey.Substring( 0, (int)zoom );
    }

    /// <summary>
    /// Converts a LatLon @zoom into a Quad and returns a QuadKey as String
    /// </summary>
    /// <param name="latLon">Coordinate</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>A QuadKey as String</returns>
    public static string LatLonToQuad( LatLon latLon, ushort zoom )
    {
      // sanity
      if (zoom < MinZoom || zoom > MaxZoom) return ""; // an empty string

      return TileXYToQuad( LatLonToTileXY( latLon, zoom ), zoom );
    }

    /// <summary>
    /// Converts a Latitude, Longitude @zoom into a Quad and returns a QuadKey as String
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>A QuadKey as String</returns>
    public static string LatLonToQuad( double lat, double lon, ushort zoom )
    {
      return LatLonToQuad( new LatLon( lat, lon ), zoom );
    }

    /// <summary>
    /// Returns the Center Coord of a Quad
    /// </summary>
    /// <param name="quad"></param>
    /// <returns>The Center Coordinate of this Quad</returns>
    public static LatLon Center( Quad quad )
    {
      return QuadKeyToTileXY( quad ).CenterLatLon( quad.ZoomLevel );
    }

    /// <summary>  
    /// Converts a QuadKey into tile XY coordinates.
    ///  credit MS: https://learn.microsoft.com/en-us/bingmaps/articles/bing-maps-tile-system
    /// </summary>  
    /// <param name="quad">Quad of the tile</param>
    public static TileXY QuadKeyToTileXY( Quad quad )
    {
      var tileX = 0;
      var tileY = 0;
      var quadKey = quad.ToString( );
      ushort levelOfDetail = (ushort)quadKey.Length;

      for (int i = levelOfDetail; i > 0; i--) {
        int mask = 1 << (i - 1);
        switch (quadKey[levelOfDetail - i]) {
          case '0':
            break;
          case '1':
            tileX |= mask;
            break;
          case '2':
            tileY |= mask;
            break;
          case '3':
            tileX |= mask;
            tileY |= mask;
            break;
          default:
            throw new ArgumentException( "Invalid QuadKey digit sequence." );
        }
      }
      return new TileXY( tileX, tileY );
    }


    /// <summary>
    /// Converts TileXY coordinates @zoom into a QuadKey at a specified level of detail.
    /// credit: https://docs.microsoft.com/en-us/bingmaps/articles/bing-maps-tile-system
    /// </summary>
    /// <param name="tileXY">Tile XY coordinate.</param>
    /// <param name="zoom">
    ///     Level of detail, from 1 (lowest detail)
    ///     to 22 (highest detail).
    /// </param>
    /// <returns>A QuadKey as String</returns>
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
    /// True if the argument Quad is included in this Quad
    /// Implies that the argument is at the same or a higher zoom level
    /// </summary>
    public static bool Includes( string _this, string _other )
    {
      if (_other.Length < _this.Length) return false; // no match anyway
      return _other.StartsWith( _this );
    }

    /// <summary>
    /// True if this Quad is part of the argument
    /// Implies that the argument is at the same or a lower zoom level
    /// </summary>
    public static bool IsPartOf( string _this, string _other )
    {
      return Includes( _other, _this ); // inverse the arguments and function
    }

    #region AROUND Functions

    /// <summary>
    /// Returns 4 Quads that are the closest neighbours of the argument
    /// assuming the argument represents a left,top entity
    /// the returned ones are then the ones to the left and above
    /// </summary>
    /// <param name="quadKey">A QuadKey</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static string[] Around( string quadKey )
    {
      string[] ret = new string[4];
      ret[0] = quadKey; // self
      ret[1] = LeftQ( ret[0] );  // left
      ret[2] = AboveQ( ret[1] ); // left up
      ret[3] = AboveQ( ret[0] ); // up
      return ret;
    }


    /// <summary>
    /// Returns 4 Quads that are the closest neighbours of the argument
    /// The selected quad around depend on the quadrant position of 
    ///   the pixel which marks the center of the area
    /// </summary>
    /// <param name="mapPixel">A MapPixel</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static string[] Around4( Point mapPixel, ushort zoom )
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
    /// The selected quad around depend on the quadrant position of 
    ///   the coordinate which marks the center of the area
    /// </summary>
    /// <param name="latLon">A LatLon</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static string[] Around4( LatLon latLon, ushort zoom )
    {
      return Around4( LatLonToMapPixel( latLon, zoom ), zoom );
    }

    /// <summary>
    /// Returns 4 Quads that are the closest neighbours of the argument
    /// The selected quad around depend on the quadrant position of 
    ///   the coordinate which marks the center of the area
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static string[] Around4( double lat, double lon, ushort zoom )
    {
      return Around4( new LatLon( lat, lon ), zoom );
    }

    /// <summary>
    /// Returns 8 surounding Quad of the argument + the argument as center [0]
    /// </summary>
    /// <param name="quadKey">A QuadKey at zoom</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static string[] Around9( string quadKey )
    {
      string[] ret = new string[9];
      ret[0] = quadKey; // self
      // go left and then Clockwise around
      ret[1] = LeftQ( ret[0] );  // left
      ret[2] = AboveQ( ret[1] ); // left up
      ret[3] = RightQ( ret[2] ); // up
      ret[4] = RightQ( ret[3] ); // right up
      ret[5] = BelowQ( ret[4] ); // right 
      ret[6] = BelowQ( ret[5] ); // right down
      ret[7] = LeftQ( ret[6] ); // down
      ret[8] = LeftQ( ret[7] ); // left down

      return ret;
    }

    /// <summary>
    /// Returns 8 surounding Quad of the argument + the argument as center [0]
    /// </summary>
    /// <param name="mapPixel">A MapPixel</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static string[] Around9( Point mapPixel, ushort zoom )
    {
      return Around9( TileXYToQuad( MapPixelToTileXY( mapPixel ), zoom ) );
    }

    /// <summary>
    /// Returns 8 surounding Quad of the argument + the argument as center [0]
    /// </summary>
    /// <param name="latLon">A LatLon</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static string[] Around9( LatLon latLon, ushort zoom )
    {
      return Around9( LatLonToMapPixel( latLon, zoom ), zoom );
    }

    /// <summary>
    /// Returns 8 surounding Quad of the argument + the argument as center [0]
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static string[] Around9( double lat, double lon, ushort zoom )
    {
      return Around9( new LatLon( lat, lon ), zoom );
    }



    /// <summary>
    /// Very dedicated use in Map Displays of 7x7 Tile Maps
    ///  (it returns essentially a 8x8 Tile Map)
    /// Intended to return a Quadlist to lookup items for such a Map.
    /// It would need to lookup items in 49 Quads to complete all on this Map.
    /// 
    /// It returns however only 15 surounding Quads of the argument + the argument as center [0]
    /// Instead of returning 49 quads it will reduce the zoom out by one level and return
    /// the surounding ones + extending on the short side
    /// results in an overflow of 15 original zoom quad areas 
    /// but saves to scan 49 items (i.e. 34 less)
    /// </summary>
    /// <param name="quadKey">A QuadKey at zoom</param>
    /// <returns>Array of neighbours -includes the argument (16 quads @ zoom-1 level)</returns>
    public static string[] Around49EX( string quadKey )
    {
      var hQ = HigherLevel( quadKey ); // zoom out by 1

      string[] ret = new string[16];
      // using the Quad shift functions which take care of the wrapping at the map border at X=0
      // first is to get the original and 8 around
      ret[0] = hQ; // self
      // go left and then Clockwise around
      ret[1] = LeftQ( ret[0] );  // left
      ret[2] = AboveQ( ret[1] ); // left up
      ret[3] = RightQ( ret[2] ); // up
      ret[4] = RightQ( ret[3] ); // right up
      ret[5] = BelowQ( ret[4] ); // right 
      ret[6] = BelowQ( ret[5] ); // right down
      ret[7] = LeftQ( ret[6] ); // down
      ret[8] = LeftQ( ret[7] ); // left down

      // then depending on the location of the original we extend the 2 lesser sides to complete the matrix
      if (LastQ( quadKey ) == "0") {
        // extend left and above
        ret[9] = LeftQ( ret[8] ); // left left down
        ret[10] = AboveQ( ret[9] ); // left left
        ret[11] = AboveQ( ret[10] ); // left left up
        ret[12] = AboveQ( ret[11] ); // left left up up
        ret[13] = RightQ( ret[12] ); // left up up
        ret[14] = RightQ( ret[13] ); //  up up
        ret[15] = RightQ( ret[14] ); // right up up
      }
      else if (LastQ( quadKey ) == "1") {
        // extend right and above
        ret[9] = RightQ( ret[6] ); // right right down
        ret[10] = AboveQ( ret[9] ); // right right
        ret[11] = AboveQ( ret[10] ); // right right up
        ret[12] = AboveQ( ret[11] ); // right right up up
        ret[13] = LeftQ( ret[12] ); // right up up
        ret[14] = LeftQ( ret[13] ); //  up up
        ret[15] = LeftQ( ret[14] ); // left up up
      }
      else if (LastQ( quadKey ) == "2") {
        // extend left and below
        ret[9] = LeftQ( ret[2] ); // left left up
        ret[10] = BelowQ( ret[9] ); // left left
        ret[11] = BelowQ( ret[10] ); // left left down
        ret[12] = BelowQ( ret[11] ); // left left down down
        ret[13] = RightQ( ret[12] ); // left down down
        ret[14] = RightQ( ret[13] ); //  down down
        ret[15] = RightQ( ret[14] ); // right down down
      }
      else if (LastQ( quadKey ) == "3") {
        // extend right and below
        ret[9] = RightQ( ret[4] ); // right right up
        ret[10] = BelowQ( ret[9] ); // right right
        ret[11] = BelowQ( ret[10] ); // right right down
        ret[12] = BelowQ( ret[11] ); // right right down down
        ret[13] = LeftQ( ret[12] ); // right down down
        ret[14] = LeftQ( ret[13] ); //  down down
        ret[15] = LeftQ( ret[14] ); // left down down
      }

      return ret;
    }

    #endregion

  }
}
