using System;
using System.Drawing;

using static CoordLib.MercatorTiles.Projection;

namespace CoordLib.MercatorTiles
{
  /// <summary>
  /// Simple Mercator Projection Tiles Method set
  /// based on 256x256 Tiles
  /// where each tile has an XY Coord at a certain Zoom Level
  /// The complete map is layed out in MapPixels
  /// 
  /// see also: https://docs.microsoft.com/en-us/bingmaps/articles/bing-maps-tile-system
  ///  
  /// Level of Detail Map Width and      Ground              Map Scale    Tile Area ( at 96 dpi)
  ///                  Height( pixels)   Resolution
  ///                                   ( meters / pixel)
  ///        ..
  ///         4           4,096        9783.9424           1 : 36,978,669.44   ~2,560km^2
  ///         5           8,192        4891.9712           1 : 18,489,334.72   ~1,280km^2
  ///         6          16,384        2445.9856           1 : 9,244,667.36      ~640km^2
  ///         7          32,768        1222.9928           1 : 4,622,333.68      ~320km^2
  ///         8          65,536         611.4964           1 : 2,311,166.84      ~160km^2
  ///         9         131,072         305.7482           1 : 1,155,583.42       ~80km^2
  ///        10         262,144         152.8741           1 : 577,791.71         ~40km^2
  ///        11         524,288          76.4370           1 : 288,895.85         ~20km^2
  ///        12       1,048,576          38.2185           1 : 144,447.93         ~10km^2
  ///        13       2,097,152          19.1093           1 : 72,223.96           ~5km^2
  ///        14       4,194,304           9.5546           1 : 36,111.98         ~2.5km^2
  ///        15       8,388,608           4.7773           1 : 18,055.99        ~1.25km^2
  ///        
  /// </summary>
  internal static class TileOp
  {

    #region Generic FromMapPixel Functions

    /// <summary>
    /// Get lat/lon coordinates from pixel coordinates
    /// </summary>
    /// <param name="mapPixel"></param>
    /// <param name="zoom">A zoom level</param>
    /// <returns>A Coordinate</returns>
    public static LatLon MapPixelToLatLon( Point mapPixel, ushort zoom )
    {
      return Projection.MapPixelToLatLon( mapPixel.X, mapPixel.Y, zoom );
    }

    /// <summary>
    /// Tile coordinate from MapPixel coordinates
    /// </summary>
    /// <param name="mapPixel">A MapPixel</param>
    /// <returns>A TileXY</returns>
    public static Point MapPixelToTileXY( Point mapPixel )
    {
      // Bing Map Docs wants Floor not implicite rounding
      return new Point( (int)Math.Floor( (double)mapPixel.X / TileSize.Width ), (int)Math.Floor( (double)mapPixel.Y / TileSize.Height ) );
    }

    /// <summary>
    /// Returns the Quadrant on a Tile where a MapPixel lies
    /// </summary>
    /// <returns>A Quadrant</returns>
    public static TileQuadrant QuadrantFromMapPixel( Point mapPixel )
    {
      var tileXY = MapPixelToTileXY( mapPixel );
      Point tileCenter = TileCenterMapPixel( tileXY );

      if (mapPixel.X <= tileCenter.X) {
        // left side
        if (mapPixel.Y <= tileCenter.Y)
          return TileQuadrant.LeftTop;
        else
          return TileQuadrant.LeftBottom;
      }
      else {
        // right side
        if (mapPixel.Y <= tileCenter.Y)
          return TileQuadrant.RightTop;
        else
          return TileQuadrant.RightBottom;
      }
    }

    #endregion

    #region Generic FromTileXY Functions

    /// <summary>
    /// MapPixel coordinate from tile coordinate (left/top corner)
    /// </summary>
    /// <param name="tileXY">A tileXY</param>
    /// <returns>a MapPixel (left/top corner)</returns>
    public static Point TileLtMapPixel( Point tileXY )
    {
      return new Point( tileXY.X * TileSize.Width, tileXY.Y * TileSize.Height );
    }
    /// <summary>
    /// MapPixel coordinate from tile coordinate (right/top corner)
    /// </summary>
    /// <param name="tileXY">A tileXY</param>
    /// <returns>a MapPixel (right/top corner)</returns>
    public static Point TileRtMapPixel( Point tileXY )
    {
      var lt = TileLtMapPixel( tileXY );
      lt.Offset( TileSize.Width, 0 );
      return lt;
    }
    /// <summary>
    /// MapPixel coordinate from tile coordinate (right/bottom corner)
    /// </summary>
    /// <param name="tileXY">A tileXY</param>
    /// <returns>a MapPixel (right/bottom corner)</returns>
    public static Point TileRbMapPixel( Point tileXY )
    {
      var lt = TileLtMapPixel( tileXY );
      lt.Offset( TileSize.Width, TileSize.Height );
      return lt;
    }
    /// <summary>
    /// MapPixel coordinate from tile coordinate (left/bottom corner)
    /// </summary>
    /// <param name="tileXY">A tileXY</param>
    /// <returns>a MapPixel (left/bottom corner)</returns>
    public static Point TileLbMapPixel( Point tileXY )
    {
      var lt = TileLtMapPixel( tileXY );
      lt.Offset( 0, TileSize.Height );
      return lt;
    }

    /// <summary>
    /// MapPixel coordinate from tile coordinate (top/left corner)
    /// </summary>
    /// <param name="tileXY">A tileXY</param>
    /// <returns>a MapPixel (top/left corner)</returns>
    public static Point TileCenterMapPixel( Point tileXY )
    {
      var mapPix = TileLtMapPixel( tileXY ); // left/top
      mapPix.Offset( TileSize.Width / 2, TileSize.Height / 2 ); // shift to center
      return mapPix;
    }

    /// <summary>
    /// Returns the coordinate of the left top of the tileXY at zoom level
    /// </summary>
    /// <param name="tileXY">A tileXY</param>
    /// <param name="zoom">A zoom level</param>
    /// <returns>The coordinate of the left top</returns>
    public static LatLon TileLtLatLon( Point tileXY, ushort zoom )
    {
      return MapPixelToLatLon( TileLtMapPixel( tileXY ), zoom );
    }

    /// <summary>
    /// Returns the coordinate of the center of the tileXY at zoom level
    /// </summary>
    /// <param name="tileXY">A tileXY</param>
    /// <param name="zoom">A zoom level</param>
    /// <returns>The coordinate of the center</returns>
    public static LatLon TileCenterLatLon( Point tileXY, ushort zoom )
    {
      return MapPixelToLatLon( TileCenterMapPixel( tileXY ), zoom );
    }

    #endregion

    #region Generic FromLatLon Functions

    /// <summary>
    /// Returns the TileXY for a coordinate on the tile at zoom level
    /// </summary>
    /// <param name="coord">A coordinate</param>
    /// <param name="zoom">A zoomlevel</param>
    /// <returns>The TileXY where the coordiate is located on</returns>
    public static Point LatLonToTileXY( LatLon coord, ushort zoom )
    {
      var center = LatLonToMapPixel( coord, zoom );
      var tilexy = MapPixelToTileXY( center );
      return tilexy;
    }

    /// <summary>
    /// Get pixel coordinates from lat/lon
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="zoom">A zoom level</param>
    /// <returns>A MapPixel</returns>
    public static Point LatLonToMapPixel( LatLon coord, ushort zoom )
    {
      return Projection.LatLonToMapPixel( coord.Lat, coord.Lon, zoom );
    }

    /// <summary>
    /// Returns the Quadrant on a Tile where a coordinate lies
    /// </summary>
    /// <returns>A Quadrant</returns>
    public static TileQuadrant QuadrantFromLatLon( LatLon coord, ushort zoom )
    {
      return QuadrantFromMapPixel( LatLonToMapPixel( coord, zoom ) );
    }

    #endregion

    #region From Projection mapped here
    /// <summary>
    /// Get pixel coordinates from lat/lon as Mercator Projection
    /// </summary>
    public static Point LatLonToMapPixel( double lat, double lon, ushort zoom ) => Projection.LatLonToMapPixel( lat, lon, zoom );
    /// <summary>
    /// Gets lat/lon coordinates from pixel coordinates
    /// </summary>
    public static LatLon MapPixelToLatLon( int x, int y, ushort zoom ) => Projection.MapPixelToLatLon( x, y, zoom );

    #endregion

  }
}
