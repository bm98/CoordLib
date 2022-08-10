using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CoordLib
{
  /// <summary>
  /// Size Struct with LatLon items as Points
  /// Note: Math operations are performed on the numbers and not in any particular projection
  /// </summary>
  public struct LLSize
  {
    /// <summary>
    /// static cTor:
    /// </summary>
    static LLSize( )
    {
      Empty = new LLSize( );
    }


    /// <summary>
    /// An Empty LLSize
    /// </summary>
    public static readonly LLSize Empty;

    /// <summary>
    /// Get: true if it is empty (Height and Width value of 0)
    /// </summary>
    public bool IsEmpty => (WidthLon == 0d) && (HeightLat == 0d);

    /// <summary>
    /// Width or Longitude degrees
    /// </summary>
    public double WidthLon { get; set; }
    /// <summary>
    /// Height or Latitude degrees
    /// </summary>
    public double HeightLat { get; set; }

    /// <summary>
    /// cTor: empty (will always init with 0 Values
    /// </summary>
    public LLSize( bool _ = true )
    {
      WidthLon = 0; HeightLat = 0;
    }

    /// <summary>
    /// cTor: copy
    /// </summary>
    public LLSize( LLSize size )
    {
      WidthLon = size.WidthLon;
      HeightLat = size.HeightLat;
    }
    /// <summary>
    /// cTor: From LLPoint values
    /// </summary>
    public LLSize( LLPoint pt )
    {
      HeightLat = pt.Lat;
      WidthLon = pt.Lon;
    }

    /// <summary>
    /// cTor: from height (latitude) and width (longitude) as coordinates [deg]
    /// </summary>
    public LLSize( double heightLat, double widthLng )
    {
      HeightLat = heightLat;
      WidthLon = widthLng;
    }

    /// <summary>
    /// Addition: Adds the width and height of one Size structure to the width and height of another Size structure.
    /// </summary>
    public static LLSize operator +( LLSize sz1, LLSize sz2 ) => Add( sz1, sz2 );

    /// <summary>
    /// Subtraction: Subtracts the width and height of one Size structure from the width and height of another Size structure.
    /// </summary>
    public static LLSize operator -( LLSize sz1, LLSize sz2 ) => Subtract( sz1, sz2 );

    /// <summary>
    /// Equality: Tests whether two Size structures are equal.
    /// </summary>
    public static bool operator ==( LLSize sz1, LLSize sz2 ) => sz1.WidthLon == sz2.WidthLon && sz1.HeightLat == sz2.HeightLat;

    /// <summary>
    /// Inequality: Tests whether two Size structures are different.
    /// </summary>
    public static bool operator !=( LLSize sz1, LLSize sz2 ) => !(sz1 == sz2);

    /// <summary>
    /// Conversion: Returns an LLPoint from the values (X=Height,Lat / Y=Width,Lon)
    /// </summary>
    /// <param name="size"></param>
    public static explicit operator LLPoint( LLSize size ) => new LLPoint( size.HeightLat, size.WidthLon );

    /// <summary>
    /// Adds the width and height of one Size structure to the width and height of another Size structure.
    /// </summary>
    public static LLSize Add( LLSize sz1, LLSize sz2 ) => new LLSize( sz1.HeightLat + sz2.HeightLat, sz1.WidthLon + sz2.WidthLon );

    /// <summary>
    /// Subtracts the width and height of one Size structure from the width and height of another Size structure.
    /// </summary>
    public static LLSize Subtract( LLSize sz1, LLSize sz2 ) => new LLSize( sz1.HeightLat - sz2.HeightLat, sz1.WidthLon - sz2.WidthLon );

    /// <summary>
    /// Tests to see whether the specified object is a Size structure with the same dimensions as this Size structure.
    /// </summary>
    public override bool Equals( object obj )
    {
      if (!(obj is LLSize)) return false;

      LLSize ef = (LLSize)obj;
      return ef.WidthLon == WidthLon && ef.HeightLat == HeightLat &&
             ef.GetType( ).Equals( GetType( ) );
    }

    /// <summary>
    /// Returns a hash code for this Size structure.
    /// </summary>
    public override int GetHashCode( )
    {
      if (IsEmpty) return 0;
      return WidthLon.GetHashCode( ) ^ HeightLat.GetHashCode( );
    }

    /// <summary>
    /// Returns the Values as LLPoint Struct
    /// </summary>
    public LLPoint ToLLPoint( ) => (LLPoint)this;

    /// <summary>
    /// Creates a human-readable string that represents this Size structure.
    /// </summary>
    public override string ToString( )
    {
      return "{WidthLon=" + WidthLon.ToString( CultureInfo.InvariantCulture ) + ", HeightLng=" +
             HeightLat.ToString( CultureInfo.InvariantCulture ) + "}";
    }


  }
}
