using System;
using System.Collections.Generic;

namespace CoordLib.UTMGrid
{
  /// <summary>
  /// UTM conversions
  /// https://en.wikipedia.org/wiki/Universal_Transverse_Mercator_coordinate_system
  /// 
  /// The UTM system divides the Earth into 60 zones, each 6° of longitude in width.
  /// Each zone is segmented into 20 latitude Bands. Each latitude band is 8 degrees high,
  ///  and is lettered starting from "C" at 80°S, increasing up the English alphabet until "X", 
  ///  omitting the letters "I" and "O" 
  /// </summary>
  internal static class UtmOp
  {
    /// <summary>
    /// Returns the UTM Zone Number with Norway exceptions handled
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <returns>The ZoneNumber</returns>
    public static int UtmZoneNo( double lat, double lon )
    {
      if (double.IsNaN( lat ) || double.IsNaN( lon )) return 0; // catch empty LatLon

      int ZoneNumber = (int)Math.Floor( (lon + 180) / 6 ) + 1;

      //Make sure the longitude 180 is in Zone 60
      if (lon >= 180) {
        ZoneNumber = 60;
      }

      // Special zone for Norway
      if (lat >= 56 && lat < 64 && lon >= 3 && lon < 12) {
        ZoneNumber = 32;
      }

      // Special zones for Svalbard
      if (lat >= 72 && lat < 84) {
        if (lon >= 0 && lon < 9) {
          ZoneNumber = 31;
        }
        else if (lon >= 9 && lon < 21) {
          ZoneNumber = 33;
        }
        else if (lon >= 21 && lon < 33) {
          ZoneNumber = 35;
        }
        else if (lon >= 33 && lon < 42) {
          ZoneNumber = 37;
        }
      }
      return ZoneNumber;
    }

    /// <summary>
    /// Returns the UTM Zone Number with Norway exceptions handled
    /// </summary>
    /// <param name="latLon">A LatLon item</param>
    /// <returns>The ZoneNumber</returns>
    public static int UtmZoneNo( LatLon latLon )
    {
      return UtmZoneNo( latLon.Lat, latLon.Lon );
    }


    /// <summary>
    /// Calculates the MGRS letter designator for the given latitude.
    /// latitude The latitude in WGS84 to get the letter designator for.
    /// The letter designator.
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <returns>The ZoneLetter</returns>
    public static string UtmLetterDesignator( double lat, double lon )
    {
      if (double.IsNaN( lat ) || double.IsNaN( lon )) return "Z"; // catch empty LatLon

      if (lat <= 84 && lat >= 72) {
        // the X band is 12 degrees high
        return "X";
      }
      else if (lat < 72 && lat >= -80) {
        // Latitude bands are lettered C through X, excluding I and O
        var bandLetters = "CDEFGHJKLMNPQRSTUVWX";
        var bandHeight = 8;
        var minLatitude = -80;
        int index = (int)Math.Floor( (lat - minLatitude) / bandHeight );
        return bandLetters.Substring( index, 1 );
      }
      else if (lat > 84) {
        if (lon >= 0)
          return "Z"; // East
        else
          return "Y"; // West
      }
      else if (lat < -80) {
        if (lon >= 0)
          return "B"; // East
        else
          return "A"; // West
      }
      return "Z";
    }

    /// <summary>
    /// Calculates the MGRS letter designator for the given latitude.
    /// latitude The latitude in WGS84 to get the letter designator for.
    /// The letter designator.
    /// </summary>
    /// <param name="latLon">LatLon obj</param>
    /// <returns>The ZoneLetter</returns>
    public static string UtmLetterDesignator( LatLon latLon )
    {
      return UtmLetterDesignator( latLon.Lat, latLon.Lon );
    }

    /// <summary>
    /// Returns the UtmZone Designator NNC 
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// </summary>
    public static string UtmZone( double lat, double lon )
    {
      return $"{UtmZoneNo( lat, lon ):00}{UtmLetterDesignator( lat, lon )}";
    }

    /// <summary>
    /// Returns the UtmZone Designator NNC 
    /// <param name="latLon">LatLon obj</param>
    /// </summary>
    public static string UtmZone( LatLon latLon )
    {
      return UtmZone( latLon.Lat, latLon.Lon );
    }

    /// <summary>
    /// UTM Zone list
    /// </summary>
    public static List<int> UTM_ZoneList = new List<int>( ) {
      1,2,3,4,5,6,7,8,9,10,
      11,12,13,14,15,16,17,18,19,20,
      21,22,23,24,25,26,27,28,29,30,
      31,32,33,34,35,36,37,38,39,40,
      41,42,43,44,45,46,47,48,49,50,
      51,52,53,54,55,56,57,58,59,60,
    };

    /// <summary>
    /// MGRS Band Letter List
    /// </summary>
    public static List<string> UTM_BandList = new List<string>( ) {
      "A","B",
      "C","D","E","F","G","H","J","K","L","M","N","P","Q","R","S","T","U","V","W","X",
      "Y","Z",
    };

    /// <summary>
    /// Returns the center coordinate of an UTM Cell
    ///   given the Zone and the Band
    /// </summary>
    /// <param name="utmZone">The Zone number</param>
    /// <param name="utmBand">The Band letter</param>
    /// <returns>The center coordinate</returns>
    public static LatLon UTMCellCenterCoord( int utmZone, string utmBand )
    {
      // sanity
      if (!UTM_ZoneList.Contains( utmZone )) return LatLon.Empty;
      if (!UTM_BandList.Contains( utmBand )) return LatLon.Empty;

      // calc center
      double lon = -180.0 + utmZone * 6.0 - 3.0;
      // Special zones will not taken care of as the center cannot be calculated in reverse

      double lat = 0.0;
      if (utmBand == "Z") {
        // north of 84 East
        lat = 84 + 3;
      }
      else if (utmBand == "Y") {
        // north of 84 West
        lat = 84 + 3;
      }
      else if (utmBand == "X") {
        // the X band is 12 degrees high 72..84
        lat = 72 + 6;
      }
      else if (utmBand == "B") {
        // the X band is 10 degrees high -80..-90 East
        lat = -80 - 5;
      }
      else if (utmBand == "A") {
        // the X band is 10 degrees high -80..-90 West
        lat = -80 - 5;
      }
      else {
        // regular bands
        var bandLetters = "CDEFGHJKLMNPQRSTUVWX";
        int mult = bandLetters.IndexOf(utmBand); // 0...
        lat = -80 + mult * 8 + 4; // center of the band
      }

      return new LatLon( lat, lon );
    }


  }
}
