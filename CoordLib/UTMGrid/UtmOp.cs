using System;
using System.Collections.Generic;
using System.Text;

namespace CoordLib.UTMGrid
{
  /// <summary>
  /// UTM conversions
  /// https://en.wikipedia.org/wiki/Universal_Transverse_Mercator_coordinate_system
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


  }
}
