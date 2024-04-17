using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      string ll = Dms.ToLat( lat, "dms", " ", 2 ) + ",";
      ll += Dms.ToLon( lat, "dms", " ", 2 );
      return ll;
    }

    /// <summary>
    /// Convert from values to an LLA string
    ///   N9° 44' 31.60",E118° 45' 31.51",+000045.00
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="alt">Altitude (must be in meters for FSim use)</param>
    /// <returns>An LLA string</returns>
    public static string ToLLA( double lat, double lon, double alt )
    {
      string lla = ToLL( lat, lon ) + ",";
      lla += $"{alt:+000000.00;000000.00}";
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
      if (double.TryParse( e[2], out alt )) {
        return !(double.IsNaN( lat ) || double.IsNaN( lon ));
      }
      return false;
    }


  }
}
