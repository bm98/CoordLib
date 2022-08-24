using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CoordLib.MercatorTiles
{
  /// <summary>
  /// A MapPixel for Mercator Tile Projections
  /// </summary>
  public struct MapPixel
  {
    private Point _mapPixel;

    /// <summary>
    /// An emtpy (unusable) MapPixel at -1/-1
    /// </summary>
    public static MapPixel Empty => new MapPixel( -1, -1 );

    /// <summary>
    /// Returns the MapPixel for a coordinate on the tile at zoom level
    /// </summary>
    /// <param name="latLon,">A coordinate</param>
    /// <param name="zoom">A zoomlevel</param>
    /// <returns>The MapPixel for the coordiate</returns>
    public static MapPixel LatLonToMapPixel( LatLon latLon, ushort zoom ) => TileOp.LatLonToMapPixel( latLon, zoom ).AsMapPixel( );
    /// <summary>
    /// Returns the MapPixel for a coordinate on the tile at zoom level
    /// </summary>
    /// <param name="lat,">Latitude</param>
    /// <param name="lon,">Longitude</param>
    /// <param name="zoom">A zoomlevel</param>
    /// <returns>The MapPixel for the coordiate</returns>
    public static MapPixel LatLonToMapPixel( double lat, double lon, ushort zoom ) => LatLonToMapPixel( new LatLon( lat, lon ), zoom );


    /// <summary>
    /// cTor: From X,Y
    /// </summary>
    public MapPixel( int x = 0, int y = 0 ) { _mapPixel = new Point( x, y ); }

    /// <summary>
    /// cTor: Copy constructor
    /// </summary>
    public MapPixel( MapPixel other ) { _mapPixel = other._mapPixel; }

    /// <summary>
    /// cTor: From a Point
    /// </summary>
    public MapPixel( Point point ) { _mapPixel = point; }

    /// <summary>
    /// cTor: From LatLon at Zoom Level
    /// </summary>
    public MapPixel( LatLon latLon, ushort zoom ) { _mapPixel = TileOp.LatLonToMapPixel( latLon, zoom ); }

    /// <summary>
    /// cTor: From Latitude, Longitude at Zoom Level
    /// </summary>
    public MapPixel( double lat, double lon, ushort zoom ) { _mapPixel = TileOp.LatLonToMapPixel( new LatLon( lat, lon ), zoom ); }

    /// <summary>
    /// The MapPixel X value
    /// </summary>
    public int X => _mapPixel.X;
    /// <summary>
    /// The MapPixel Y value
    /// </summary>
    public int Y => _mapPixel.Y;

    /// <summary>
    /// Returns the MapPixel as Point
    /// </summary>
    /// <returns>A Point</returns>
    public Point AsPoint( ) => _mapPixel;

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==( MapPixel _this, MapPixel _other )
    {
      return _this._mapPixel == _other._mapPixel; // based on Point equality
    }
    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=( MapPixel _this, MapPixel _other )
    {
      return !(_this._mapPixel == _other._mapPixel);
    }

    /// <summary>
    /// Equality towads another object
    /// </summary>
    public override bool Equals( object obj )
    {
      return obj is MapPixel xY &&
             EqualityComparer<Point>.Default.Equals( _mapPixel, xY._mapPixel );
    }

    /// <summary>
    /// Returns a Hashcode for this object
    /// </summary>
    public override int GetHashCode( )
    {
      int hashCode = 359691488;
      hashCode = hashCode * -1521134295 + _mapPixel.GetHashCode( );
      return hashCode;
    }

    /// <summary>
    /// Translates this MapPixel by the specified amount
    /// </summary>
    public void Offset( int dx, int dy ) => _mapPixel.Offset( dx, dy );



    /// <summary>
    /// Tile coordinate from MapPixel coordinates
    /// </summary>
    /// <returns>A TileXY</returns>
    public TileXY ToTileXY( ) => TileOp.MapPixelToTileXY( _mapPixel ).AsTileXY( );

    /// <summary>
    /// Returns the Quadrant on a Tile where a MapPixel lies
    /// </summary>
    /// <returns>A Quadrant</returns>
    public TileQuadrant Quadrant( ) => TileOp.QuadrantFromMapPixel( _mapPixel );


    // World Mapped @ Zoom Level

    /// <summary>
    /// Get lat/lon coordinates from pixel coordinates
    /// </summary>
    /// <param name="zoom">A zoom level</param>
    /// <returns>A Coordinate</returns>
    public LatLon ToLatLon( ushort zoom ) => Projection.MapPixelToLatLon( _mapPixel.X, _mapPixel.Y, zoom );

    /// <summary>
    /// Returns resolution of this pixel at a zoom level in meters
    /// - square pixels i.e. both sides have the same Resolution
    /// </summary>
    /// <param name="zoom"></param>
    /// <returns>Meters</returns>
    public float Resolution_m( ushort zoom )
    {
      var latLon = ToLatLon( zoom );
      return (float)Projection.MapResolution_mPerPixel( zoom, latLon.Lat );
    }

    /// <summary>
    /// Returns SizeF of this pixel at a zoom level in meters
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
