using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using static CoordLib.MercatorTiles.QuadOp;
using static CoordLib.MercatorTiles.Projection;

namespace CoordLib.MercatorTiles
{
  /// <summary>
  /// Quad Tree Item
  /// Based on the Mercator Projection Grid with a Quad Square Tile of 256x256
  /// see also: https://docs.microsoft.com/en-us/bingmaps/articles/bing-maps-tile-system
  /// 
  /// The Quad at a ZoomLevel 1..22 is composed of digits base4 (0,1,2,3) other digits or characters are invalid
  /// Min Zoom is 1; Max Zoom is 22
  /// Note: a Quad which is empty (ZoomLevel 0) would represent the entire map 
  /// 
  /// </summary>
  public struct Quad
  {
    #region STATIC 

    /// <summary>
    /// An empty quad
    /// </summary>
    public static Quad Empty => new Quad( "" );

    /// <summary>
    /// Returns a Quad from a coordinate at zoom
    /// </summary>
    /// <param name="latLon">Coordinate</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>A Quadkey as String</returns>
    public static Quad LatLonToQuad( LatLon latLon, ushort zoom ) => new Quad( QuadOp.LatLonToQuad( latLon, zoom ) );
    /// <summary>
    /// Returns a Quad from a coordinate at zoom
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>A Quadkey as String</returns>
    public static Quad LatLonToQuad( double lat, double lon, ushort zoom ) => LatLonToQuad( new LatLon( lat, lon ), zoom );


    /// <summary>
    /// Returns 4 Quads that are the closest neighbours of the argument
    /// assuming the argument represents a left,top entity
    /// the returned ones are then the ones to the left and above
    /// </summary>
    /// <param name="quad">A Quad</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static IList<Quad> Around( Quad quad )
    {
      var ret = new List<Quad>( );
      string[] qs = QuadOp.Around( quad.ToString( ) );
      foreach (var q in qs) {
        ret.Add( new Quad( q ) );
      }
      return ret;
    }


    /// <summary>
    /// Returns 4 Quads that are the closest neighbours of the argument
    /// including the one that includes the argument
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static IList<Quad> Around4( double lat, double lon, ushort zoom )
    {
      // sanity
      if (zoom < MinZoom || zoom > MaxZoom) return new List<Quad>( ); // an empty list

      var ret = new List<Quad>( );
      string[] qs = QuadOp.Around4( lat, lon, zoom );
      foreach (var q in qs) {
        ret.Add( new Quad( q ) );
      }
      return ret;
    }

    /// <summary>
    /// Returns 4 Quads that are the closest neighbours of the argument
    /// including the one that includes the argument
    /// </summary>
    /// <param name="latLon">A LatLon</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static IList<Quad> Around4( LatLon latLon, ushort zoom )
    {
      return Around4( latLon.Lat, latLon.Lon, zoom );
    }


    /// <summary>
    /// Returns 8 surounding Quad of the argument + the argument as center [0]
    /// </summary>
    /// <param name="quad">A QuadKey at zoom</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static IList<Quad> Around9( Quad quad )
    {
      var ret = new List<Quad>( );
      string[] qs = QuadOp.Around9( quad.ToString( ) );
      foreach (var q in qs) {
        ret.Add( new Quad( q ) );
      }
      return ret;
    }

    /// <summary>
    /// Returns 8 surounding Quad of the argument + the argument as center [0]
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static IList<Quad> Around9( double lat, double lon, ushort zoom )
    {
      // sanity
      if (zoom < MinZoom || zoom > MaxZoom) return new List<Quad>( ); // an empty list

      var ret = new List<Quad>( );
      string[] qs = QuadOp.Around9( lat, lon, zoom );
      foreach (var q in qs) {
        ret.Add( new Quad( q ) );
      }
      return ret;
    }

    /// <summary>
    /// Returns 8 surounding Quad of the argument + the argument as center [0]
    /// </summary>
    /// <param name="latLon">A LatLon</param>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static IList<Quad> Around9( LatLon latLon, ushort zoom )
    {
      return Around9( latLon.Lat, latLon.Lon, zoom );
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
    /// <param name="quad">A Quad at  original zoom</param>
    /// <returns>Array of neighbours -includes the argument</returns>
    public static IList<Quad> Around49EX( Quad quad )
    {
      var ret = new List<Quad>( );
      string[] qs = QuadOp.Around49EX( quad.ToString( ) );
      foreach (var q in qs) {
        ret.Add( new Quad( q ) );
      }
      return ret;
    }

    #endregion

    // STRUCT

    // our quad as string
    private string _quad;

    /// <summary>
    /// cTor: Initialize from string (beware, no validity checks are made 
    /// </summary>
    public Quad( string quadString = "" ) { _quad = CheckQuad( quadString ) ? quadString : ""; }
    /// <summary>
    /// cTor: As copy of the argument
    /// </summary>
    public Quad( Quad other ) { _quad = CheckQuad( other._quad ) ? other._quad : ""; }
    /// <summary>
    /// cTor: From a LatLon coordinate at a zoom level
    /// </summary>
    public Quad( LatLon latLon, ushort zoom ) { _quad = (zoom >= MinZoom && zoom <= MaxZoom) ? QuadOp.LatLonToQuad( latLon, zoom ) : ""; }
    /// <summary>
    /// cTor: From a Latitude, Longitude coordinate at a zoom level
    /// </summary>
    public Quad( double lat, double lon, ushort zoom ) { _quad = (zoom >= MinZoom && zoom <= MaxZoom) ? QuadOp.LatLonToQuad( lat, lon, zoom ) : ""; }
    /// <summary>
    /// cTor: From a TileXY coordinate at a zoom level
    /// </summary>
    public Quad( Point tileXY, ushort zoom ) { _quad = TileXYToQuad( tileXY, zoom ); }

    /// <summary>
    /// True for an Empty Quad
    /// </summary>
    public bool IsEmpty => string.IsNullOrEmpty( _quad );

    /// <summary>
    /// Returns the string representation
    /// </summary>
    /// <returns>A string</returns>
    public override string ToString( )
    {
      return _quad;
    }

    /// <summary>
    /// Returns a reduced Quad at the desired zoom
    /// If reduction not possible it returns the argument
    /// </summary>
    /// <param name="zoom">Zoomlevel</param>
    /// <returns>A Quad</returns>
    public Quad AtZoom( ushort zoom )
    {
      return new Quad( QuadOp.QuadAtZoom( _quad, zoom ) );
    }

    /// <summary>
    /// Returns the Center Coord of a Quad
    /// </summary>
    /// <returns>The Center Coordinate of this Quad</returns>
    public LatLon Center( ) => QuadOp.Center( this );

    /// <summary>
    /// True if the argument Quad is included in this Quad
    /// Implies that the argument is at the same or a higher zoom level
    /// </summary>
    public bool Includes( Quad other ) => QuadOp.Includes( this._quad, other._quad );

    /// <summary>
    /// True if this Quad is part of the argument
    /// Implies that the argument is at the same or a lower zoom level
    /// </summary>
    public bool IsPartOf( Quad other ) => QuadOp.IsPartOf( this._quad, other._quad );


    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==( Quad _this, Quad _other )
    {
      return _this._quad == _other._quad; // based on string equality
    }
    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=( Quad _this, Quad _other )
    {
      return !(_this._quad == _other._quad);
    }

    /// <summary>
    /// Equality towads another object
    /// </summary>
    public override bool Equals( object obj )
    {
      return obj is Quad quad && _quad == quad._quad;
    }

    /// <summary>
    /// Returns a Hashcode for this object
    /// </summary>
    public override int GetHashCode( )
    {
      int hashCode = -1047403195;
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode( _quad );
      return hashCode;
    }
    /// <summary>
    /// returns the ZoomLevel of this Quad
    /// Note: ZoomLevel 0 means an empty Quad.
    /// </summary>
    public ushort ZoomLevel => ZoomLevel( _quad );

    /// <summary>
    /// Returns the QuadKey of the zoomed out level of the argument
    /// can resolve in an empty string which is not really a Quad anymore
    /// </summary>
    /// <returns>A Quad Key</returns>
    public Quad HigherLevel( ) => new Quad( QuadOp.HigherLevel( _quad ) );
    /// <summary>
    /// Returns the rightmost Quad Item (character)
    /// can resolve in an empty string which is not really a Quad cahr anymore
    /// </summary>
    /// <returns>The rightmost Quad Digit as string</returns>
    public string LastQ( ) => QuadOp.LastQ( _quad );


    /// <summary>
    /// Get the QuadKey left of the current one
    /// </summary>
    /// <returns>A Quad Key</returns>
    public Quad LeftQ( ) => new Quad( QuadOp.LeftQ( _quad ) );
    /// <summary>
    /// Get the QuadKey right of the current one
    /// </summary>
    /// <returns>A Quad Key</returns>
    public Quad RightQ( ) => new Quad( QuadOp.RightQ( _quad ) );
    /// <summary>
    /// Get the QuadKey above of the current one
    /// </summary>
    /// <returns>A Quad Key</returns>
    public Quad AboveQ( ) => new Quad( QuadOp.AboveQ( _quad ) );
    /// <summary>
    /// Get the QuadKey below of the current one
    /// </summary>
    /// <returns>A Quad Key</returns>
    public Quad BelowQ( ) => new Quad( QuadOp.BelowQ( _quad ) );


    /// <summary>
    /// Returns 4 Quads that are the closest neighbours of this quad
    /// assuming this quad represents a left,top entity
    /// the returned ones are then the ones to the left and above
    /// </summary>
    /// <returns>Array of neighbours -includes the argument</returns>
    public IList<Quad> Around( )
    {
      return Around( this );
    }

    /// <summary>
    /// Returns 8 surounding Quad of this quad + this quad as center [0]
    /// </summary>
    /// <returns>Array of neighbours -includes the argument</returns>
    public IList<Quad> Around9( )
    {
      return Around9( this );
    }

    /// <summary>
    /// Very dedicated use in Map Displays of 7x7 Tile Maps
    ///  (it returns essentially a 8x8 Tile Map)
    /// Intended to return a Quadlist to lookup items for such a Map.
    /// It would need to lookup items in 49 Quads to complete all on this Map.
    /// 
    /// It returns however only 15 surounding Quads of this quad + this quad as center [0]
    /// Instead of returning 49 quads it will reduce the zoom out by one level and return
    /// the surounding ones + extending on the short side
    /// results in an overflow of 15 original zoom quad areas 
    /// but saves to scan 49 items (i.e. 34 less)
    /// </summary>
    /// <returns>Array of neighbours -includes the argument</returns>
    public IList<Quad> Around49EX( )
    {
      return Around49EX( this );
    }


  }
}
