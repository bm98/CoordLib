using System;
using System.Globalization;

namespace CoordLib
{
  /// <summary>
  /// The JDN the number of whole and fractional days since Noon, 1st January -4712 UTC.
  /// </summary>
  public class JulianDay : IEquatable<JulianDay>, IComparable<JulianDay>
  {
    // Thank you (no license attached AFAIK):
    // https://github.com/GoodOlClint/Astronomy
    // modified BM for clarity

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

    // DateTime Ticks (100 ns/Tick) per Day
    private const long c_TicksPerDay = 24L * 60L * 60L * 1000L * 1000L * 10L;

    // A.D. 1582 October 15 	12:00:00.0 	2299161.000000
    private const long c_JDN_GregorianStart = 2299161L;

    // ref dates to calc a JDN from Ticks (could be any date)
    // Saturday 	A.D. 2000 January 1 	00:00:00.0 	2451544.500000
    private const double c_JDN_20000101 = 2451544.5;
    // new DateTime( 2000, 1, 1 ).Ticks = 63_082_281_600_000_000_0
    private static readonly long c_Ticks_20000101 = new DateTime( 2000, 1, 1 ).Ticks;
    // a JDN from a Ticks Date Value / past Gregorian Start only !!
    private static double JDN_FromTicksDate( long ticksDate ) => c_JDN_20000101 + (ticksDate - c_Ticks_20000101) / c_TicksPerDay;


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
    public long JulianDaysElapsed => INTF( JulianDayNumber );

    /// <summary>
    /// Day of the week
    /// </summary>
    public DayOfWeek DayOfWeek {
      get {
        int JD = INTF( this.JulianDayNumber + 1.5 );
        return (DayOfWeek)(JD % 7);
      }
    }

    /// <summary>
    /// Day of the year number
    /// </summary>
    public int DayOfYear {
      get {
        int K;
        if (this.IsLeapYear) {
          K = 1;
        }
        else {
          K = 2;
        }

        int N = INTF( (275 * this.Month) / 9 ) - K * INTF( (this.Month + 9) / 12 ) + this.Day - 30;

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
        if (isCentury && this.CalendarOfDate == Calendar.Gregorian) { return ((this.Year % 400) == 0); }
        else { return ((this.Year % 4) == 0); }
      }

    }

    /// <summary>
    /// Returns a Calendar Enum for the given JDN
    /// 
    /// The Gregorian calendar reform moved changed the calendar from the Julian to Gregorian standard.
    /// This means that the day immediately after 4th October 1582 is 15th October 1582.
    /// </summary>
    public Calendar CalendarOfDate {
      get {
        if (this.JulianDayNumber != 0) {
          // A.D. 1582 October 15 	12:00:00.0 	2299161.000000
          if (this.JulianDayNumber < c_JDN_GregorianStart) {
            // JDN is non zero i.e. set and needs to be offset
            return Calendar.Julian;
          }
          else {
            // JDN is non zero i.e. set and does not needs to be offset
            return Calendar.Gregorian;
          }
        }
        else {
          // JDN is not set (have only parts property values)
          if (this.Year > 1582) { return Calendar.Gregorian; } // no offset
          else if (this.Year < 1582) { return Calendar.Julian; } // needs offset
          else if ((this.Year == 1582) && (this.Month < 10)) { return Calendar.Julian; } // needs offset
          else if ((this.Year == 1582) && (this.Month > 10)) { return Calendar.Gregorian; } // no offset
          else if ((this.Year == 1582) && (this.Month == 10) && (this.Day < 5)) { return Calendar.Julian; } // needs offset
          else if ((this.Year == 1582) && (this.Month > 10) && (this.Day > 14)) { return Calendar.Gregorian; } // no offset
          else { throw new IndexOutOfRangeException( "The dates October 5th - 14th 1582 are not valid for JDN" ); }
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
    private int INTF( double input ) => Convert.ToInt32( Math.Floor( input ) );

    #region Constructors

    /// <summary>
    /// cTor: JDN based on JD=0
    /// </summary>
    public JulianDay( )
    {
      // JDN = 0 date
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
    /// cTor: JDN based on Gregorian DateTime given i.e. past Oct 15 A.D. 1582
    /// </summary>
    /// <param name="dateTime">A Gregorian DateTime</param>
    /// <exception cref="ArgumentOutOfRangeException">Invalid dates when calendars changed</exception>
    public JulianDay( DateTime dateTime )
        // Saturday 	A.D. 2000 January 1 	00:00:00.0 	2451544.500000
        // new DateTime( 2000, 1, 1 ).Ticks = 63_082_281_600_000_000_0
        : this( JDN_FromTicksDate( dateTime.Ticks ) )
    {
      // Throw an error to prevent unexpected results.
      if (this.CalendarOfDate == Calendar.Julian) { throw new ArgumentOutOfRangeException( "The System.DateTime class should not be use for dates prior to October 15th 1582. Invalid calculations will result." ); }
    }

    /// <summary>
    /// cTor: JDN with given args
    /// </summary>
    /// <param name="Year">Year -4712..9999</param>
    /// <param name="Month">Month 1..12</param>
    /// <param name="Day">Day 1..31</param>
    public JulianDay( int Year, int Month, int Day ) : this( Year, Month, Day, 0 ) { }

    /// <summary>
    /// cTor: JDN with given args
    /// </summary>
    /// <param name="Year">Year -4712..9999</param>
    /// <param name="Month">Month 1..12</param>
    /// <param name="Day">Day 1..31</param>
    /// <param name="Hour">Hour 0..23</param>
    public JulianDay( int Year, int Month, int Day, int Hour ) : this( Year, Month, Day, Hour, 0 ) { }

    /// <summary>
    /// cTor: JDN with given args
    /// </summary>
    /// <param name="Year">Year -4712..9999</param>
    /// <param name="Month">Month 1..12</param>
    /// <param name="Day">Day 1..31</param>
    /// <param name="Hour">Hour 0..23</param>
    /// <param name="Minute">Minute 0..59</param>
    public JulianDay( int Year, int Month, int Day, int Hour, int Minute ) : this( Year, Month, Day, Hour, Minute, 0 ) { }

    /// <summary>
    /// cTor: JDN with given args
    /// </summary>
    /// <param name="Year">Year -4712..9999</param>
    /// <param name="Month">Month 1..12</param>
    /// <param name="Day">Day 1..31</param>
    /// <param name="Hour">Hour 0..23</param>
    /// <param name="Minute">Minute 0..59</param>
    /// <param name="Second">Second 0..59</param>
    public JulianDay( int Year, int Month, int Day, int Hour, int Minute, int Second ) : this( Year, Month, Day, Hour, Minute, Second, 0 ) { }

    /// <summary>
    /// cTor: JDN with given args
    /// </summary>
    /// <param name="Year">Year -4712..9999</param>
    /// <param name="Month">Month 1..12</param>
    /// <param name="Day">Day 1..31</param>
    /// <param name="Hour">Hour 0..23</param>
    /// <param name="Minute">Minute 0..59</param>
    /// <param name="Second">Second 0..59</param>
    /// <param name="Millisecond">Millisecond 0..999</param>
    public JulianDay( int Year, int Month, int Day, int Hour, int Minute, int Second, int Millisecond )
    {
      // sanity
      if (Year < -4712 || Year > 9999) throw new ArgumentException( "Year must be within -4712..9999" );
      if (Month < 1 || Month > 12) throw new ArgumentException( "Month must be within 1..12" );
      if (Day < 1 || Day > 31) throw new ArgumentException( "Day must be within 1..31" );
      if (Hour < 0 || Hour > 23) throw new ArgumentException( "Hour must be within 0..23" );
      if (Minute < 0 || Minute > 59) throw new ArgumentException( "Minute must be within 0..59" );
      if (Second < 0 || Second > 59) throw new ArgumentException( "Second must be within 0..59" );
      if (Millisecond < 0 || Second > 999) throw new ArgumentException( "Millisecond must be within 0..999" );

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
    public JulianDay( double jdn )
    {
      // sanity
      if (jdn < 0) throw new ArgumentException( "JDN must be >= 0" );

      this.JulianDayNumber = jdn;
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
      // fractions of the day
      d += ((double)this.Hour / 24d);
      d += ((double)this.Minute / 60d) / 24d;
      d += ((double)this.Second / 60d) / 60d / 24d;
      d += ((double)this.Millisecond / 1000d) / 60d / 60d / 24d;

      /* Jean Meeus: Astronomical Algorithms.
          wenn Monat > 2 dann
              Y = Jahr;    M = Monat
          sonst
              Y = Jahr−1;  M = Monat+12
          D = Tag   // inklusive Tagesbruchteil
   
          wenn julianischer Kalender dann
              B = 0
          sonst  // gregorianischer Kalender
              B = 2 − ⌊Y/100⌋ + ⌊Y/400⌋
   
          JD = ⌊365.25(Y+4716)⌋ + ⌊30.6001(M+1)⌋ + D + B − 1524.5       
      */
      if (m <= 2) {
        y = y - 1;
        m = m + 12;
      }
      int B = 0;
      if (this.CalendarOfDate == Calendar.Gregorian) {
        // adjust for difference in leap years usage between the two calendars
        B = 2 - INTF( y / 100 ) + INTF( y / 400 );
      }
      double JD = INTF( 365.25 * (y + 4716) ) + INTF( 30.6001 * (m + 1) ) + d + B - 1524.5;

      this.JulianDayNumber = JD;
    }

    // calc Date values from JDN
    private void CalculateDate( )
    {
      /* Jean Meeus: Astronomical Algorithms.
          Z = ⌊JD + 0,5⌋
          F = JD + 0,5 − Z
   
          wenn julianischer Kalender dann
              A = Z
          sonst  // gregorianischer Kalender
              α = ⌊(Z − 1_867_216.25)/36_524.25⌋
              A = Z + 1 + α − ⌊α/4⌋
   
          B = A + 1524
          C = ⌊(B − 122.1)/365.25⌋
          D = ⌊365.25 C⌋
          E = ⌊(B − D)/30.6001⌋
   
          Tag = B − D − ⌊30.6001 E⌋ + F   // inklusive Tagesbruchteil
          wenn E ≤ 13 dann
              Monat = E − 1;   Jahr = C − 4716
          sonst
              Monat = E − 13;  Jahr = C − 4715       
       */
      double JD = this.JulianDayNumber;
      int Z = INTF( JD + 0.5 );
      double F = JD + 0.5 - Z;

      int A;
      if (Z < c_JDN_GregorianStart) {
        A = Z;
      }
      else {
        int α = INTF( (Z - 1_867_216.25) / 36_524.25 );
        A = Z + 1 + α - INTF( α / 4 );
      }
      int B = A + 1524;
      int C = INTF( (B - 122.1) / 365.25 );
      int D = INTF( 365.25 * C );
      int E = INTF( (B - D) / 30.6001 );

      this.Day = B - D - INTF( 30.6001 * E ); // omit day parts here

      if (E <= 13) {
        this.Month = E - 1;
        this.Year = C - 4716;
      }
      else {
        this.Month = E - 13;
        this.Year = C - 4715;
      }
      // day parts
      this.Hour = INTF( F * 24 );
      this.Minute = INTF( ((F * 24) - this.Hour) * 60 );
      this.Second = INTF( ((((F * 24) - this.Hour) * 60) - this.Minute) * 60 );
      this.Millisecond = INTF( ((((((F * 24) - this.Hour) * 60) - this.Minute) * 60) - this.Second) * 1000 );

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
      return INTF( this.JulianDayNumber );
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
