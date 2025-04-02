using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace CoordLib
{
  /// <summary>
  /// Calendar Enum
  /// </summary>
  public enum Calendar
  {
    /// <summary>
    /// Julian Calendar
    /// </summary>
    Julian,
    /// <summary>
    /// Gregorian Calendar (DateTime base)
    /// </summary>
    Gregorian
  }

  /// <summary>
  /// The JDN the number of whole and fractional days since Noon, 1st January -4712 UTC.
  /// </summary>
  public class JulianDay : IEquatable<JulianDay>, IComparable<JulianDay>
  {
    // Thank you (no license attached AFAIK):
    // https://github.com/GoodOlClint/Astronomy

    #region Public Static Methods

    /// <summary>
    /// A JDN obj for Now
    /// </summary>
    /// <returns>A JDN obj</returns>
    public static JulianDay Now( ) => new JulianDay( DateTime.UtcNow );


    #endregion

    #region Public Properties

    /// <summary>
    /// Julian Year number
    /// </summary>
    public int Year { get; protected set; }
    /// <summary>
    /// Julian Month of year number
    /// </summary>
    public int Month { get; protected set; }
    /// <summary>
    /// Julian Day of month number
    /// </summary>
    public int Day { get; protected set; }
    /// <summary>
    /// Julian Hour of day number
    /// </summary>
    public int Hour { get; protected set; }
    /// <summary>
    /// Julian Minute of hour number
    /// </summary>
    public int Minute { get; protected set; }
    /// <summary>
    /// Julian Second of minute number
    /// </summary>
    public int Second { get; protected set; }
    /// <summary>
    /// Julian Millisecond of second number
    /// </summary>
    public int Millisecond { get; protected set; }

    /// <summary>
    /// Julian Day number since start of the calendar
    /// </summary>
    public double JulianDayNumber { get; protected set; }

    /// <summary>
    /// Julian Day number (as long) since start of the calendar
    /// </summary>
    public long JulianDaysElapsed => INT( JulianDayNumber );

    /// <summary>
    /// Day of the week
    /// </summary>
    public DayOfWeek DayOfWeek {
      get {
        int JD = INT( this.JulianDayNumber + 1.5 );
        return (DayOfWeek)(JD % 7);
      }
    }

    /// <summary>
    /// Day of the year number
    /// </summary>
    public int DayOfYear {
      get {
        int K;
        if (this.IsLeapYear) { K = 1; }
        else { K = 2; }

        Debug.WriteLine( "K\t= " + K );
        Debug.WriteLine( "M\t= " + this.Month );
        Debug.WriteLine( "D\t= " + this.Day );
        int N = INT( (275 * this.Month) / 9 ) - K * INT( (this.Month + 9) / 12 ) + this.Day - 30;
        Debug.WriteLine( "N\t= " + N );

        return N;
      }
    }

    /// <summary>
    /// On the Julian Calendar a year divisible by 4 is a leap year
    /// On the Gregorian Calendar a year divisible by 4, except centuries (divisible by 100) not divisible by 400, is a leap year.
    /// </summary>
    public bool IsLeapYear {
      get {
        bool isCentury = (this.Year % 100 == 0);
        if (isCentury && this.Calendar == Calendar.Gregorian) { return ((this.Year % 400) == 0); }
        else { return ((this.Year % 4) == 0); }
      }

    }

    /// <summary>
    /// Returns a Calendar Enum for the given JDN
    /// 
    /// The Gregorian calendar reform moved changed the calendar from the Julian to Gregorian standard.
    /// This means that the day immediately after 4th October 1582 is 15th October 1582.
    /// </summary>
    public Calendar Calendar {
      get {
        if (this.JulianDayNumber != 0) {
          if (this.JulianDayNumber < 2299161) { return Calendar.Julian; }
          else { return Calendar.Gregorian; }
        }
        else {
          if (this.Year > 1582) { return Calendar.Gregorian; }
          else if (this.Year < 1582) { return Calendar.Julian; }
          else if ((this.Year == 1582) && (this.Month < 10)) { return Calendar.Julian; }
          else if ((this.Year == 1582) && (this.Month > 10)) { return Calendar.Gregorian; }
          else if ((this.Year == 1582) && (this.Month == 10) && (this.Day < 5)) { return Calendar.Julian; }
          else if ((this.Year == 1582) && (this.Month > 10) && (this.Day > 14)) { return Calendar.Gregorian; }
          else { throw new IndexOutOfRangeException( "The dates October 5th - 14th 1582 are not valid" ); }
        }
      }
    }
    #endregion

    /// <summary>
    /// Add Days to the current JDN
    /// </summary>
    /// <param name="days">Days to add</param>
    /// <returns>A new JDN obj with added days</returns>
    public JulianDay AddDays( double days ) => new JulianDay( this.JulianDayNumber + days );

    /// <summary>
    /// Add Minutes to the current JDN
    /// </summary>
    /// <param name="minutes">Minutes to add</param>
    /// <returns>A new JDN obj with added minutes</returns>
    public JulianDay AddMinutes( double minutes ) => this.AddDays( (minutes / 60) / 24 );

    /// <summary>
    /// Add Seconds to the current JDN
    /// </summary>
    /// <param name="seconds">Seconds to add</param>
    /// <returns>A new JDN obj with added seconds</returns>
    public JulianDay AddSeconds( double seconds ) => this.AddMinutes( (seconds / 60) / 60 );

    // floor of input
    private int INT( double input ) => Convert.ToInt32( Math.Floor( input ) );

    #region Constructors

    /// <summary>
    /// cTor: JDN based on JD=0
    /// </summary>
    public JulianDay( )
    {
      this.Year = -4712;
      this.Month = 1;
      this.Day = 1;
      this.Hour = 12;
      this.JulianDayNumber = 0;
    }

    /// <summary>
    /// cTor: Copy
    /// </summary>
    public JulianDay( JulianDay other ) : this( other.JulianDayNumber ) { }

    /// <summary>
    /// cTor: JDN based on Gregorian DateTime given
    /// </summary>
    /// <param name="dateTime">A Gregorian DateTime</param>
    /// <exception cref="ArgumentOutOfRangeException">Invalid dates when calendars changed</exception>
    public JulianDay( DateTime dateTime )
        : this( 2451544.5 + ((dateTime.Ticks - 630822816000000000) / 10000d / 1000d) / 60d / 60d / 24d )
    {
      //DateTime objects are always Gregorian Calendar based. Throw an error to prevent unexpected results.
      if (this.Calendar == Calendar.Julian) { throw new ArgumentOutOfRangeException( "The System.DateTime class should not be use for dates prior to October 15th 1582. Invalid calculations will result." ); }
    }

    /// <summary>
    /// cTor: JDN with given args
    /// </summary>
    /// <param name="Year">Year</param>
    /// <param name="Month">Month</param>
    /// <param name="Day">Day</param>
    public JulianDay( int Year, int Month, int Day ) : this( Year, Month, Day, 0 ) { }

    /// <summary>
    /// cTor: JDN with given args
    /// </summary>
    /// <param name="Year">Year</param>
    /// <param name="Month">Month</param>
    /// <param name="Day">Day</param>
    /// <param name="Hour">Hour</param>
    public JulianDay( int Year, int Month, int Day, int Hour ) : this( Year, Month, Day, Hour, 0 ) { }

    /// <summary>
    /// cTor: JDN with given args
    /// </summary>
    /// <param name="Year">Year</param>
    /// <param name="Month">Month</param>
    /// <param name="Day">Day</param>
    /// <param name="Hour">Hour</param>
    /// <param name="Minute">Minute</param>
    public JulianDay( int Year, int Month, int Day, int Hour, int Minute ) : this( Year, Month, Day, Hour, Minute, 0 ) { }

    /// <summary>
    /// cTor: JDN with given args
    /// </summary>
    /// <param name="Year">Year</param>
    /// <param name="Month">Month</param>
    /// <param name="Day">Day</param>
    /// <param name="Hour">Hour</param>
    /// <param name="Minute">Minute</param>
    /// <param name="Second">Second</param>
    public JulianDay( int Year, int Month, int Day, int Hour, int Minute, int Second ) : this( Year, Month, Day, Hour, Minute, Second, 0 ) { }

    /// <summary>
    /// cTor: JDN with given args
    /// </summary>
    /// <param name="Year">Year</param>
    /// <param name="Month">Month</param>
    /// <param name="Day">Day</param>
    /// <param name="Hour">Hour</param>
    /// <param name="Minute">Minute</param>
    /// <param name="Second">Second</param>
    /// <param name="Millisecond">Millisecond</param>
    public JulianDay( int Year, int Month, int Day, int Hour, int Minute, int Second, int Millisecond )
    {

      this.Year = Year;
      this.Month = Month;
      this.Day = Day;
      this.Hour = Hour;
      this.Minute = Minute;
      this.Second = Second;
      this.Millisecond = Millisecond;
      this.CalculateFromDate( );
    }


    /// <summary>
    /// cTor: JDN from a JDN number
    /// </summary>
    public JulianDay( double JDN )
    {
      this.JulianDayNumber = JDN;
      this.CalculateDate( );
    }

    #endregion

    // calc JDN from Date values
    private void CalculateFromDate( )
    {
      this.JulianDayNumber = 0;
      int y = this.Year;
      int m = this.Month;
      double d = this.Day;
      d += ((double)this.Hour / 24d);
      d += ((double)this.Minute / 60d) / 24d;
      d += ((double)this.Second / 60d) / 60d / 24d;
      d += ((double)this.Millisecond / 1000d) / 60d / 60d / 24d;
      if (m <= 2) {
        y = y - 1;
        m = m + 12;
      }
      int B = 0;
      if (this.Calendar == Calendar.Gregorian) {
        int A = INT( y / 100 );
        B = 2 - A + INT( A / 4 );
        Debug.WriteLine( "A\t= " + A );
      }

      double JD = INT( 365.25 * (y + 4716) ) + INT( 30.6001 * (m + 1) ) + d + B - 1524.5;

      Debug.WriteLine( "B\t= " + B );
      Debug.WriteLine( "JD\t= " + JD );
      this.JulianDayNumber = JD;
    }

    // calc Date values from JDN
    private void CalculateDate( )
    {
      double JD = this.JulianDayNumber + 0.5;
      int Z = (int)Math.Truncate( JD );
      double F = JD - Z;
      int A;
      if (Z < 2299161) { A = Z; }
      else {
        int a = INT( (Z - 1867216.25) / 36524.25 );
        A = Z + 1 + a - INT( a / 4 );
        Debug.WriteLine( "a\t= " + a );
      }
      int B = A + 1524;
      int C = INT( (B - 122.1) / 365.25 );
      int D = INT( 365.25 * C );
      int E = INT( (B - D) / 30.6001 );

      this.Day = B - D - INT( 30.6001 * E );

      if (E < 14) { this.Month = E - 1; }
      else { this.Month = E - 13; }

      if (this.Month > 2) { this.Year = C - 4716; }
      else { this.Year = C - 4715; }

      this.Hour = INT( F * 24 );
      this.Minute = INT( ((F * 24) - this.Hour) * 60 );
      this.Second = INT( ((((F * 24) - this.Hour) * 60) - this.Minute) * 60 );
      this.Millisecond = INT( ((((((F * 24) - this.Hour) * 60) - this.Minute) * 60) - this.Second) * 1000 );

#if DEBUG
      /*
      Debug.WriteLine( "A\t= " + A );
      Debug.WriteLine( "B\t= " + B );
      Debug.WriteLine( "C\t= " + C );
      Debug.WriteLine( "D\t= " + D );
      Debug.WriteLine( "E\t= " + E );
      Debug.WriteLine( "Day\t= " + this.Day );
      Debug.WriteLine( "Month\t= " + this.Month );
      Debug.WriteLine( "Year\t= " + this.Year );
      */
#endif

    }

    #region Mathematical Operators

    /// <summary>
    /// Add days to a JDN
    /// </summary>
    /// <param name="JDE1">Arg1</param>
    /// <param name="days">Days to add</param>
    /// <returns>A Julian</returns>
    public static JulianDay operator +( JulianDay JDE1, double days )
    {
      return JDE1.AddDays( days );
    }

    /// <summary>
    /// Subtract days from a JDN
    /// </summary>
    /// <param name="JDE1">Arg1</param>
    /// <param name="days">Days to subtract</param>
    /// <returns>A JDN obj</returns>
    public static JulianDay operator -( JulianDay JDE1, double days )
    {
      return JDE1.AddDays( -days );
    }


    /// <inheritdoc/>
    public static bool operator ==( JulianDay JDE1, JulianDay JDE2 )
    {
      return JDE1.Equals( JDE2 );
    }
    /// <inheritdoc/>
    public static bool operator !=( JulianDay JDE1, JulianDay JDE2 )
    {
      return !JDE1.Equals( JDE2 );
    }

    /// <inheritdoc/>
    public static bool operator >( JulianDay JDE1, JulianDay JDE2 )
    {
      return JDE1.JulianDayNumber > JDE2.JulianDayNumber;
    }
    /// <inheritdoc/>
    public static bool operator <( JulianDay JDE1, JulianDay JDE2 )
    {
      return JDE1.JulianDayNumber < JDE2.JulianDayNumber;
    }

    /// <inheritdoc/>
    public static bool operator >=( JulianDay JDE1, JulianDay JDE2 )
    {
      return JDE1.JulianDayNumber >= JDE2.JulianDayNumber;
    }
    /// <inheritdoc/>
    public static bool operator <=( JulianDay JDE1, JulianDay JDE2 )
    {
      return JDE1.JulianDayNumber <= JDE2.JulianDayNumber;
    }

    #endregion

    /// <inheritdoc/>
    public override string ToString( )
    {
      int year = this.Year;
      string era = "CE";
      if (year <= 0) {
        era = "BCE";
        year = Math.Abs( year ) + 1;
      }
      return string.Format( "{0}, {1} {2}, {3} {4}", this.DayOfWeek, this.Day, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName( this.Month ), year, era );
    }

    /// <inheritdoc/>
    public override int GetHashCode( )
    {
      return INT( this.JulianDayNumber );
    }

    /// <inheritdoc/>
    public bool Equals( JulianDay other )
    {
      return this.JulianDayNumber == other.JulianDayNumber;
    }

    /// <inheritdoc/>
    public override bool Equals( object obj )
    {
      if (obj == null) return false;
      if (obj is JulianDay jdn) {
        return (this.JulianDayNumber == jdn.JulianDayNumber);
      }
      return false;
    }

    /// <inheritdoc/>
    public int CompareTo( JulianDay other )
    {
      return this.JulianDayNumber.CompareTo( other.JulianDayNumber );
    }


  }
}
