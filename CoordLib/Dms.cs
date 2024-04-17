using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CoordLib
{
  /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
  /* Geodesy representation conversion functions                        (c) Chris Veness 2002-2017  */
  /*                                                                                   MIT Licence  */
  /* www.movable-type.co.uk/scripts/latlong.html                                                    */
  /* www.movable-type.co.uk/scripts/geodesy/docs/module-dms.html                                    */
  /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
  // note Unicode Degree = U+00B0. Prime = U+2032, Double prime = U+2033
  //
  // ***************
  // NOTE: CHANGED FROM ORIGINAL TO MATCH THE FSIM LLA NOTATION
  // BM20210114
  //
  // N9° 44' 31.60",E118° 45' 31.51"
  //
  // i.e. NWSE leading pos, degs not with leading zeroes, Min,Sec attributes as regular quote and double quotes, Separator default <space>
  //
  // BM20240327 removed the static Separator and introduced it as argument
  //
  /// <summary>
  /// Geodesy representation conversion functions  
  /// Latitude/longitude points may be represented as double degrees, or subdivided into sexagesimal
  /// minutes and seconds.
  /// 1:1 C# translation from above
  /// </summary>
  public class Dms
  {
    /// <summary>
    /// Non Breaking Space char
    /// </summary>
    public const string NbSpace = "\u202F";

    /// <summary>
    /// Parses string representing degrees/minutes/seconds into numeric degrees.
    /// 
    /// This is very flexible on formats, allowing signed double degrees, or deg-min-sec optionally
    /// suffixed by compass direction( NSEW). A variety of separators are accepted( eg 3° 37′ 09″W).
    /// Seconds and minutes may be omitted.
    /// 
    ///  var lat = Dms.parseDMS("N51° 28' 40.12\"");
    ///  var lon = Dms.parseDMS( "W0° 00' 5.31\"" );
    ///  var p1 = new LatLon( lat, lon ); N51.4778°, W0.0015°
    /// </summary>
    /// <param name="dmsStr">{string|number} dmsStr - Degrees or deg/min/sec in variety of formats.</param>
    /// <returns>{number} Degrees as double number.</returns>
    public static double ParseDMS( string dmsStr )
    {
      // check for signed double degrees without NSEW, if so return it directly
      if (double.TryParse( dmsStr, out double dValue )) return dValue;

      double deg = double.NaN;
      // shall never fail..
      try {

        // strip off any sign or compass dir'n & split out separate d/m/s
        // var dms = String( dmsStr ).trim( ).replace(/^-/, '' ).replace(/[NSEW]$/i, '' ).split(/[^0-9.,]+/);
        var dms = Regex.Replace( dmsStr.Trim( ), @"^-", "" );
        dms = Regex.Replace( dms, @"^[NSEWnsew]", "" ); // at start of line
        string[] dmsA = Regex.Split( dms, @"[^0-9.,]+" );
        //if ( dms[dms.length - 1] == '' ) dms.splice( dms.length - 1 );  // from trailing symbol
        if (dmsA[dmsA.Length - 1] == "") {
          Array.Resize( ref dmsA, dmsA.Length - 1 ); //dms.splice( dms.length - 1 );  // from trailing symbol
        }
        if (dmsA.Length == 0) return double.NaN; // NaN;

        // and convert to double degrees...
        switch (dmsA.Length) {
          case 3:  // interpret 3-part result as d/m/s
            deg = double.Parse( dmsA[0] ) / 1 + double.Parse( dmsA[1] ) / 60 + double.Parse( dmsA[2] ) / 3600;
            break;
          case 2:  // interpret 2-part result as d/m
            deg = double.Parse( dmsA[0] ) / 1 + double.Parse( dmsA[1] ) / 60;
            break;
          case 1:  // just d (possibly double) or non-separated dddmmss
            deg = double.Parse( dmsA[0] );
            // check for fixed-width unseparated format eg 0033709W
            //if (/[NS]/i.test(dmsStr)) deg = '0' + deg;  // - normalise N/S to 3-digit degrees
            if (Regex.Match( dmsStr, @"[NSns]" ).Success) { dmsA[0] = "0" + dmsA[0]; }
            // can be 3,5,7 long (make 7 pad with 0)
            dmsA[0].PadRight( 7, '0' );
            //if (/[0-9]{7}/.test(deg)) deg = deg.slice(0,3)/1 + deg.slice(3,5)/60 + deg.slice(5)/3600;
            if (Regex.Match( dmsA[0], @"^[0-9]{7}" ).Success) {
              deg = double.Parse( dmsA[0].Substring( 0, 3 ) )
                  + double.Parse( dmsA[0].Substring( 3, 2 ) ) / 60.0
                  + double.Parse( dmsA[0].Substring( 5, 2 ) ) / 3600.0;
            }
            break;
          default:
            return double.NaN;
        }
        if (Regex.IsMatch( dmsStr.Trim( ), "^-|^[WSws]|[WSws]$" ))
          deg = -deg; // take '-', west and south as -ve

      }
      catch { }

      return deg;
    }

    /// <summary>
    /// Parse a Route Coord string (DD[MM[SS]]{N|S}DDD[MM[SS]]{E|W})
    /// </summary>
    /// <param name="rtCoordStr">A route coord string</param>
    /// <returns>A LatLon</returns>
    public static LatLon ParseRouteCoord( string rtCoordStr )
    {
      Regex _rx = new Regex( "^(?<coord>(?<lat>[0-8][0-9]([0-5][0-9]([0-5][0-9])?)?[NS])(?<lon>[0-1][0-9][0-9]([0-5][0-9]([0-5][0-9])?)?[EW]))$", RegexOptions.Compiled | RegexOptions.IgnoreCase );
      Match match = _rx.Match( rtCoordStr );
      if (match.Success) {
        return new LatLon( ParseDMS( match.Groups["lat"].Value ), ParseDMS( match.Groups["lon"].Value ) );
      }
      return LatLon.Empty;
    }

    /// <summary>
    /// Converts a LatLon to a Route COORD string 
    /// DDMM[SS]{N|S}DDDMM[SS]{E|W} 0..89°59'59" N/S 0..180°00'00" E/W
    /// </summary>
    /// <param name="latLon">A LatLon</param>
    /// <param name="format">{string} [format=dms] - Return value as 'd', 'dm', 'dms' for deg, deg+min, deg+min+sec.</param>
    /// <returns>A coord string</returns>
    public static string ToRouteCoord( LatLon latLon, string format = "dms" )
    {
      string latS = ToDMS( latLon.Lat, true, format, "", 0 ); // 89°59'59" 
      string lonS = ToDMS( latLon.Lon, false, format, "", 0 ); // 180°00'00"
      latS = latS.Replace( "°", "" ).Replace( "'", "" ).Replace( "\"", "" );
      lonS = lonS.Replace( "°", "" ).Replace( "'", "" ).Replace( "\"", "" );
      // for LAT
      int lLen = format.Length * 2;
      if (lonS.Length < lLen) lonS = "0" + lonS;
      lLen++; //  for LON
      if (lonS.Length < lLen) lonS = "0" + lonS;
      if (lonS.Length < lLen) lonS = "0" + lonS;
      latS += (latLon.Lat < 0) ? "S" : "N";
      lonS += (latLon.Lon < 0) ? "W" : "E";
      return latS + lonS;
    }

    /// <summary>
    /// Converts double degrees to deg/min/sec format
    ///  - degree, prime, double-prime symbols are added, but sign is discarded, though no compass
    /// direction is added.
    /// </summary>
    /// <param name="deg">{number} deg - Degrees to be formatted as specified.</param>
    /// <param name="lat">bool - Format Latitude(00 Deg), else it is Longitude(000 Deg)</param>
    /// <param name="format">{string} [format=dms] - Return value as 'd', 'dm', 'dms' for deg, deg+min, deg+min+sec.</param>
    /// <param name="separator">Item separator string - default a space</param>
    /// <param name="dPlaces">{number} [dp=0|2|4] - Number of double places to use – default 0 for dms, 2 for dm, 4 for d.</param>
    /// <returns>{string} Degrees formatted as deg/min/secs according to specified format.</returns>
    public static string ToDMS( double deg, bool lat = true, string format = "dms", string separator = " ", int dPlaces = -1 )
    {
      if (deg == double.NaN) return ""; // give up here if we can't make a number from deg

      // default values
      if (dPlaces == -1) {
        switch (format) {
          case "d": case "deg": dPlaces = 4; break;
          case "dm": case "deg+min": dPlaces = 2; break;
          case "dms": case "deg+min+sec": dPlaces = 0; break;
          default: format = "dms"; dPlaces = 0; break; // be forgiving on invalid format
        }
      }

      deg = Math.Abs( deg );  // (unsigned result ready for appending compass dir'n)

      string dms = "", d = "", m = "", s = "";
      double dN = 0, mN = 0, sN = 0;
      string degFmt = lat ? "00" : "000";
      switch (format) {
        case "d":
        case "deg": {
          // round/right-pad degrees, left-pad with leading zeros (note may include doubles)
          if (dPlaces > 0)
            d = Math.Round( deg, dPlaces ).ToString( degFmt + "." + "000000".Substring( 0, dPlaces ) );
          else
            d = Math.Round( deg, dPlaces ).ToString( degFmt );
          dms = d + '°';
        }
        break;

        case "dm":
        case "deg+min": {
          dN = Math.Floor( deg );
          mN = Math.Round( ((deg * 60) % 60), dPlaces );// get component min & round/right-pad
          if (mN == 60) { mN = 0; dN++; }               // check for rounding up
          d = dN.ToString( degFmt );
          if (dPlaces > 0)
            m = mN.ToString( "00." + "000000".Substring( 0, dPlaces ) );
          else
            m = mN.ToString( "00" );
          dms = d + '°' + separator + m + "'";
        }
        break;

        case "dms":
        case "deg+min+sec": {
          dN = Math.Floor( deg );
          mN = Math.Floor( (deg * 60) % 60 );// get component min & round/right-pad
          sN = Math.Round( (deg * 3600 % 60), dPlaces );  // get component sec & round/right-pad
          if (sN == 60) { sN = 0; mN++; } // check for rounding up
          if (mN == 60) { mN = 0; dN++; } // check for rounding up
          d = dN.ToString( degFmt );
          m = mN.ToString( "00" );
          if (dPlaces > 0)
            s = sN.ToString( "00." + "000000".Substring( 0, dPlaces ) );
          else
            s = sN.ToString( "00" );
          dms = d + '°' + separator + m + "'" + separator + s + '"';
        }
        break;
        default: break; // invalid format spec!
      }

      return dms;
    }

    /// <summary>
    /// Converts double degrees to deg/min/sec string array
    ///  - returns a string array where [0] +-Degree, [1] Min and [2] Sec
    /// </summary>
    /// <param name="deg">{number} deg - Degrees to be formatted as specified.</param>
    /// <param name="lat">bool - Format Latitude(00 Deg), else it is Longitude(000 Deg)</param>
    /// <returns>{string} Degrees formatted as deg/min/secs according to specified format.</returns>
    public static string[] ToDMSarray( double deg, bool lat )
    {
      string dms;
      if (lat) {
        dms = ToLat( deg, "dms", "" );
      }
      else {
        dms = ToLon( deg, "dms", "" );
      }
      dms = dms.Replace( "°", "|" ).Replace( "'", "|" ).Replace( "\"", "" ); ; // +-deg|min|sec"

      return dms.Split( new char[] { '|' }, StringSplitOptions.None );
    }

    /// <summary>
    /// Converts numeric degrees to deg/min/sec latitude (2-digit degrees, prefixed with N/S).
    /// </summary>
    /// <param name="deg">{number} deg - Degrees to be formatted as specified.</param>
    /// <param name="format">{string} [format=dms] - Return value as 'd', 'dm', 'dms' for deg, deg+min, deg+min+sec.</param>
    /// <param name="separator">Item separator string - default a space</param>
    /// <param name="dPlaces">{number} [dp=0|2|4] - Number of double places to use – default 0 for dms, 2 for dm, 4 for d.</param>
    /// <returns>{string} Degrees formatted as deg/min/secs according to specified format.</returns>
    public static string ToLat( double deg, string format = "dms", string separator = " ", int dPlaces = -1 )
    {
      var lat = ToDMS( deg, true, format, separator, dPlaces );
      return (lat == "") ? "–" : ((deg < 0 ? 'S' : 'N') + separator + lat);
    }


    /// <summary>
    /// Convert numeric degrees to deg/min/sec longitude (3-digit degrees, prefixed with E/W)
    /// </summary>
    /// <param name="deg">{number} deg - Degrees to be formatted as specified.</param>
    /// <param name="format">{string} [format=dms] - Return value as 'd', 'dm', 'dms' for deg, deg+min, deg+min+sec.</param>
    /// <param name="separator">Item separator string - default a space</param>
    /// <param name="dPlaces">{number} [dp=0|2|4] - Number of double places to use – default 0 for dms, 2 for dm, 4 for d.</param>
    /// <returns>{string} Degrees formatted as deg/min/secs according to specified format.</returns>
    public static string ToLon( double deg, string format = "dms", string separator = " ", int dPlaces = -1 )
    {
      var lon = ToDMS( deg, false, format, separator, dPlaces );
      return (lon == "") ? "–" : (deg < 0 ? 'W' : 'E') + separator + lon;
    }


    /// <summary>
    /// Converts numeric degrees to deg/min/sec as a bearing (0°..360°)
    /// </summary>
    /// <param name="deg">{number} deg - Degrees to be formatted as specified.</param>
    /// <param name="format">{string} [format=dms] - Return value as 'd', 'dm', 'dms' for deg, deg+min, deg+min+sec.</param>
    /// <param name="separator">Item separator string - default a space</param>
    /// <param name="dPlaces">{number} [dp=0|2|4] - Number of double places to use – default 0 for dms, 2 for dm, 4 for d.</param>
    /// <returns>{string} Degrees formatted as deg/min/secs according to specified format.</returns>
    public static string ToBrng( double deg, string format = "dms", string separator = " ", int dPlaces = -1 )
    {
      deg = Geo.Wrap360( deg );  // normalise -ve values to 180°..360°
      var brng = ToDMS( deg, false, format, separator, dPlaces ); // use Longitude format 000 Deg
      return (brng == "") ? "–" : brng.Replace( "360", "0" ) + "°";  // just in case rounding took us up to 360°!
    }


    /// <summary>
    /// Returns compass point (to given precision) for supplied bearing.
    ///      * @example
    ///      * var point = Dms.compassPoint(24);    // point = 'NNE'
    ///      * var point = Dms.compassPoint( 24, 1 ); // point = 'N'
    /// </summary>
    /// <param name="bearing">{number} bearing - Bearing in degrees from north.</param>
    /// <param name="precision">{number} [precision=3] - Precision (1:cardinal / 2:intercardinal / 3:secondary-intercardinal).</param>
    /// <returns>{string} Compass point for supplied bearing.</returns>
    public static string CompassPoint( double bearing, int precision = 3 )
    {
      // note precision could be extended to 4 for quarter-winds (eg NbNW), but I think they are little used
      bearing = ((bearing % 360) + 360) % 360; // normalise to range 0..360°

      var cardinals = new string[] {
          "N", "NNE", "NE", "ENE",
          "E", "ESE", "SE", "SSE",
          "S", "SSW", "SW", "WSW",
          "W", "WNW", "NW", "NNW" };
      int n = (int)(4 * Math.Pow( 2, precision - 1 )); // no of compass points at req’d precision (1=>4, 2=>8, 3=>16)
      var cardinal = cardinals[(int)Math.Round( bearing * n / 360 ) % n * 16 / n];

      return cardinal;
    }



  }
}
