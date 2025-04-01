using System;
using System.Collections.Generic;
using System.Drawing;

using CoordLib.Extensions;

namespace CoordLib.MercatorTiles
{
  /// <summary>
  /// A Tile Quadrant designation
  /// </summary>
  public enum TileQuadrant
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    LeftTop = 0,
    RightTop,
    RightBottom,
    LeftBottom
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

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
  [Serializable]
  public struct TileXY
  {
    private Point _tileXY;

    /// <summary>
    /// An emtpy (unusable) TileXY at -1/-1
    /// </summary>
    public static TileXY Empty => new TileXY( -1, -1 );

    /// <summary>
    /// Returns the TileXY for a coordinate on the tile at zoom level
    /// </summary>
    /// <param name="latLon,">A coordinate</param>
    /// <param name="zoom">A zoomlevel</param>
    /// <returns>The TileXY where the coordiate is located on</returns>
    public static TileXY LatLonToTileXY( LatLon latLon, ushort zoom ) => TileOp.LatLonToTileXY( latLon, zoom ).AsTileXY( );
    /// <summary>
    /// Returns the TileXY for a coordinate on the tile at zoom level
    /// </summary>
    /// <param name="lat,">Latitude</param>
    /// <param name="lon,">Longitude</param>
    /// <param name="zoom">A zoomlevel</param>
    /// <returns>The TileXY where the coordiate is located on</returns>
    public static TileXY LatLonToTileXY( double lat, double lon, ushort zoom ) => LatLonToTileXY( new LatLon( lat, lon ), zoom );

    /// <summary>
    /// Returns the Quadrant on a Tile where a coordinate lies
    /// </summary>
    /// <param name="latLon,">A coordinate</param>
    /// <param name="zoom">A zoomlevel</param>
    /// <returns>A Quadrant</returns>
    public static TileQuadrant QuadrantFromLatLon( LatLon latLon, ushort zoom ) => TileOp.QuadrantFromLatLon( latLon, zoom );
    /// <summary>
    /// Returns the Quadrant on a Tile where a coordinate lies
    /// </summary>
    /// <param name="lat,">Latitude</param>
    /// <param name="lon,">Longitude</param>
    /// <param name="zoom">A zoomlevel</param>
    /// <returns>A Quadrant</returns>
    public static TileQuadrant QuadrantFromLatLon( double lat, double lon, ushort zoom ) => QuadrantFromLatLon( new LatLon( lat, lon ), zoom );

    /// <summary>
    /// cTor: From X,Y (defaults to 0/0)
    /// </summary>
    public TileXY( int x = 0, int y = 0 ) { _tileXY = new Point( x, y ); }

    /// <summary>
    /// cTor: Copy constructor
    /// </summary>
    public TileXY( TileXY other ) { _tileXY = other._tileXY; }

    /// <summary>
    /// cTor: From a Point
    /// </summary>
    public TileXY( Point point ) { _tileXY = point; }

    /// <summary>
    /// cTor: From LatLon at Zoom Level
    /// </summary>
    public TileXY( LatLon latLon, ushort zoom ) { _tileXY = TileOp.LatLonToTileXY( latLon, zoom ); }

    /// <summary>
    /// cTor: From Latitude, Longitude at Zoom Level
    /// </summary>
    public TileXY( double lat, double lon, ushort zoom ) { _tileXY = TileOp.LatLonToTileXY( new LatLon( lat, lon ), zoom ); }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==( TileXY _this, TileXY _other )
    {
      return _this._tileXY == _other._tileXY; // based on Point equality
    }
    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=( TileXY _this, TileXY _other )
    {
      return !(_this._tileXY == _other._tileXY);
    }

    /// <summary>
    /// Equality towads another object
    /// </summary>
    public override bool Equals( object obj )
    {
      return obj is TileXY xY &&
             EqualityComparer<Point>.Default.Equals( _tileXY, xY._tileXY );
    }

    /// <summary>
    /// Returns a Hashcode for this object
    /// </summary>
    public override int GetHashCode( )
    {
      int hashCode = 359691488;
      hashCode = hashCode * -1521134295 + _tileXY.GetHashCode( );
      return hashCode;
    }

    /// <summary>
    /// Translates this Tile by the specified amount, wraps around
    /// </summary>
    public void Offset( int dx, int dy, ushort zoom )
    {
      // this may over, under cut the XY coordinates
      _tileXY.Offset( dx, dy );
      this.Wrap( zoom); // so wrap if needed
    }

    /// <summary>
    /// Wraps the Tile for the zoom level
    /// </summary>
    /// <param name="zoom">A zoom level</param>
    public void Wrap( ushort zoom )
    {
      var dim = Projection.MapSizeTileXY( zoom );
      _tileXY.X = (_tileXY.X + dim.Width) % dim.Width;
      _tileXY.Y = (_tileXY.Y + dim.Height) % dim.Height;
    }

    /// <summary>
    /// The tiles X value
    /// </summary>
    public int X => _tileXY.X;
    /// <summary>
    /// The tiles Y value
    /// </summary>
    public int Y => _tileXY.Y;

    /// <summary>
    /// Returns the TileXY as Point
    /// </summary>
    /// <returns>A Point</returns>
    public Point AsPoint( ) => _tileXY;

    /// <summary>
    /// Returns a Quad(Key) for this Tile at Zoom Level
    /// </summary>
    /// <param name="zoom">A ZoomLevel</param>
    /// <returns>A Quad</returns>
    public Quad QuadKey( ushort zoom ) => new Quad( _tileXY, zoom );

    /// <summary>
    /// MapPixel coordinate from tile coordinate (left/top corner)
    /// </summary>
    /// <returns>a MapPixel (left/top corner)</returns>
    public MapPixel LeftTopMapPixel => TileOp.TileLtMapPixel( _tileXY ).AsMapPixel( );
    /// <summary>
    /// MapPixel coordinate from tile coordinate (right/top corner)
    /// </summary>
    /// <returns>a MapPixel (right/top corner)</returns>
    public MapPixel RightTopMapPixel => TileOp.TileRtMapPixel( _tileXY ).AsMapPixel( );
    /// <summary>
    /// MapPixel coordinate from tile coordinate (right/bottom corner)
    /// </summary>
    /// <returns>a MapPixel (right/bottom corner)</returns>
    public MapPixel RightBottomMapPixel => TileOp.TileLbMapPixel( _tileXY ).AsMapPixel( );
    /// <summary>
    /// MapPixel coordinate from tile coordinate (left/bottom corner)
    /// </summary>
    /// <returns>a MapPixel (left/bottom corner)</returns>
    public MapPixel LeftBottomMapPixel => TileOp.TileRbMapPixel( _tileXY ).AsMapPixel( );

    /// <summary>
    /// MapPixel coordinate from tile coordinate (left/top corner)
    /// </summary>
    /// <returns>a MapPixel (left/top corner)</returns>
    public MapPixel CenterMapPixel => TileOp.TileCenterMapPixel( _tileXY ).AsMapPixel( );


    // World Mapped @ Zoom Level

    /// <summary>
    /// Returns the coordinate of the left top corner of the tileXY at zoom level
    /// </summary>
    /// <param name="zoom">A zoom level</param>
    /// <returns>The coordinate of the left top corner</returns>
    public LatLon LeftTopLatLon( ushort zoom ) => TileOp.TileLtLatLon( _tileXY, zoom );

    /// <summary>
    /// Returns the coordinate of the center of the tileXY at zoom level
    /// </summary>
    /// <param name="zoom">A zoom level</param>
    /// <returns>The coordinate of the center</returns>
    public LatLon CenterLatLon( ushort zoom ) => TileOp.TileCenterLatLon( _tileXY, zoom );

    /// <summary>
    /// Returns resolution of this tile at a zoom level in meters
    /// - square tiles i.e. both sides have the same Resolution
    /// </summary>
    /// <param name="zoom"></param>
    /// <returns>Meters</returns>
    public float Resolution_m( ushort zoom )
    {
      var latLon = CenterLatLon( zoom );
      return (float)Projection.MapResolution_mPerTile( zoom, latLon.Lat );
    }

    /// <summary>
    /// Returns SizeF of this tile at a zoom level in meters
    /// </summary>
    /// <param name="zoom"></param>
    /// <returns>A SizeF in meters</returns>
    public SizeF Size_m( ushort zoom )
    {
      float res = this.Resolution_m( zoom );
      return new SizeF( res, res );
    }

  }
}
