using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CoordLib.MercatorTiles
{
  /// <summary>
  /// Mercator Tile Projection base items
  /// </summary>
  public static class Projection
  {
    /// <summary>
    /// Minimum Latitude (limited around the S-Pole)
    /// </summary>
    public static readonly double MinLatitude = -85.05112878; // limit around the Pole
    /// <summary>
    /// Maximum Latitude (limited around the N-Pole)
    /// </summary>
    public static readonly double MaxLatitude = 85.05112878;  // limit around the Pole
    /// <summary>
    /// Minimum Longitude
    /// </summary>
    public static readonly double MinLongitude = -180.0;
    /// <summary>
    /// Maximum Longitude
    /// </summary>
    public static readonly double MaxLongitude = 180.0;

    /// <summary>
    /// Minimum ZoomLevel
    /// </summary>
    public static ushort MinZoom = 1;
    /// <summary>
    /// Maximum ZoomLevel
    /// </summary>
    public static ushort MaxZoom = 22; // 23 will not fit the integer anymore for Mappixels


    /// <summary>
    /// Size of tile
    /// </summary>
    public static Size TileSize => new Size( 256, 256 );

    /// <summary>
    /// Semi-major axis of ellipsoid, in meters
    /// </summary>
    public static double Axis => ConvConsts.EarthRadiusM; //  6378137;

    /// <summary>
    /// Flattening of ellipsoid
    /// </summary>
    public static double Flattening => ConvConsts.EarthFlattening_WGS84; //  f = 1/298.257223563


    /// <summary>
    /// Min. tile in tiles at custom zoom level
    /// </summary>
    public static Point MapMinTileXY( ushort zoom )
    {
      return new Point( 0, 0 );
    }

    /// <summary>
    /// Max. tile in tiles at custom zoom level
    /// </summary>
    /// <param name="zoom">A zoom level</param>
    public static Point MapMaxTileXY( ushort zoom )
    {
      int xy = 1 << (int)zoom;
      return new Point( xy - 1, xy - 1 );
    }

    /// <summary>
    /// Gets whole Map size in TileXY units
    /// </summary>
    /// <param name="zoom">A zoom level</param>
    /// <returns>A Size of [TileXY]</returns>
    public static Size MapSizeTileXY( ushort zoom )
    {
      var sMin = MapMinTileXY( zoom );
      var sMax = MapMaxTileXY( zoom );
      return new Size( sMax.X - sMin.X + 1, sMax.Y - sMin.Y + 1 );
    }

    /// <summary>
    /// Returns MapPixel Size of the whole Map at zoom level
    /// </summary>
    /// <param name="zoom">A zoom level</param>
    /// <returns>Size of MapPixels</returns>

    public static Size MapPixelSize( ushort zoom )
    {
      var s = MapSizeTileXY( zoom );
      return new Size( s.Width * TileSize.Width, s.Height * TileSize.Height );
    }

    /// <summary>
    /// The resolution in meters of a single pixel at a Latitude
    /// </summary>
    /// <param name="zoom">A zoom level</param>
    /// <param name="latitude">Latitude degrees</param>
    /// <returns>Meters</returns>
    public static double MapResolution_mPerPixel( ushort zoom, double latitude )
    {
      return Math.Cos( latitude * (Math.PI / 180.0) ) * 2 * Math.PI * ConvConsts.EarthRadiusM / MapPixelSize( zoom ).Width;
    }

    /// <summary>
    /// The resolution in meters of a single Tile at a Latitude
    /// </summary>
    /// <param name="zoom">A zoom level</param>
    /// <param name="latitude">Latitude degrees</param>
    /// <returns>Meters</returns>
    public static double MapResolution_mPerTile( ushort zoom, double latitude )
    {
      return MapResolution_mPerPixel( zoom, latitude ) * TileSize.Width;
    }

    /// <summary>
    /// Get pixel coordinates from lat/lon as Mercator Projection
    /// </summary>
    public static Point LatLonToMapPixel( double lat, double lon, ushort zoom )
    {
      var ret = Point.Empty;

      lat = lat.Clip( MinLatitude, MaxLatitude );
      lon = lon.Clip( MinLongitude, MaxLongitude );

      // how much x (0..1) related to the 360° full scale
      double x = (lon + 180d) / 360d;
      // how much y (0..1) related to the 360° full scale
      double sinLatitude = Math.Sin( lat.ToRadians( ) );
      double y = 0.5 - Math.Log( (1d + sinLatitude) / (1d - sinLatitude) ) / (4 * Math.PI);

      var s = MapPixelSize( zoom );
      long mapSizeX = s.Width;
      long mapSizeY = s.Height;

      ret.X = (int)(x * mapSizeX + 0.5).Clip( 0, mapSizeX - 1 );
      ret.Y = (int)(y * mapSizeY + 0.5).Clip( 0, mapSizeY - 1 );

      return ret;
    }

    /// <summary>
    /// Gets lat/lon coordinates from pixel coordinates
    /// </summary>
    public static LatLon MapPixelToLatLon( int x, int y, ushort zoom )
    {
      var ret = LatLon.Empty;

      var s = MapPixelSize( zoom );
      double mapSizeX = s.Width;
      double mapSizeY = s.Height;

      double xx = x.Clip( 0, mapSizeX - 1.0 ) / mapSizeX - 0.5;
      double yy = 0.5 - y.Clip( 0, mapSizeY - 1.0 ) / mapSizeY;

      ret.Lat = 90d - 360d * Math.Atan( Math.Exp( -yy * 2d * Math.PI ) ) / Math.PI;
      ret.Lon = 360d * xx;

      return ret;
    }


  }
}
