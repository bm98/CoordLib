using System;

namespace CoordLib
{
  /// <summary>
  /// Handle MS FSim LLA items
  /// 
  /// N9° 44' 31.60",E118° 45' 31.51",+000045.00
  /// </summary>
  public class LLA
  {
    /// <summary>
    /// Convert from values to an LL string
    ///   N9° 44' 31.60",E118° 45' 31.51"
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <returns>An LL string</returns>
    public static string ToLL( double lat, double lon )
    {
      string ll = Dms.ToLat( lat, format: "dms", separator: ' ', dPlaces: 2, leadingZeros: false ) + ",";
      ll += Dms.ToLon( lon, format: "dms", separator: ' ', dPlaces: 2, leadingZeros: false );
      // we have a separator ' ' between e.g. N and deg 'N 9° 58...' which needs to be removed
      ll = ll.Replace( "N ", "N" ).Replace( "W ", "W" ).Replace( "S ", "S" ).Replace( "E ", "E" ); // cheap, could use regex...
      return ll;
    }

    /// <summary>
    /// Convert from values to an LLA string
    ///   N9° 44' 31.60",E118° 45' 31.51",+000045.00
    ///   N53° 53' 55.87",W166° 32' 40.78",+000000.00
    ///   if ALT is NaN returns Alt=0 only
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="alt">Altitude (must be in meters for FSim use)</param>
    /// <returns>An LLA string</returns>
    public static string ToLLA( double lat, double lon, double alt )
    {
      string lla = ToLL( lat, lon );
      lla += "," + string.Format( Dms.c_us, "{0:+000000.00;000000.00}", double.IsNaN( alt ) ? 0 : alt ); // $"{alt:+000000.00;000000.00}";
      return lla;
    }

    /// <summary>
    /// Converts the string from an LL to its double values or returns false
    /// </summary>
    /// <param name="ll">An LL string</param>
    /// <param name="lat">out Lat</param>
    /// <param name="lon">out Lon</param>
    /// <returns>True if successfull</returns>
    public static bool TryParseLL( string ll, out double lat, out double lon )
    {
      lat = 0; lon = 0; // init defaults
      string[] e = ll.Split( new char[] { ',' } );
      if (e.Length != 2) return false; // nope
      lat = Dms.ParseDMS( e[0] );
      lon = Dms.ParseDMS( e[1] );
      return !(double.IsNaN( lat ) || double.IsNaN( lon ));
    }

    /// <summary>
    /// Converts the string from an LLA to its double values or returns false
    /// </summary>
    /// <param name="lla">An LLA string</param>
    /// <param name="lat">out Lat</param>
    /// <param name="lon">out Lon</param>
    /// <param name="alt">out Alt</param>
    /// <returns>True if successfull</returns>
    public static bool TryParseLLA( string lla, out double lat, out double lon, out double alt )
    {
      lat = 0; lon = 0; alt = 0; // init defaults
      string[] e = lla.Split( new char[] { ',' } );
      if (e.Length != 3) return false; // nope
      lat = Dms.ParseDMS( e[0] );
      lon = Dms.ParseDMS( e[1] );
      if (double.TryParse( e[2], Dms.c_real, Dms.c_us, out alt )) {
        return !(double.IsNaN( lat ) || double.IsNaN( lon ));
      }
      return false;
    }


  }
}
