using CoordLib;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace NTEST_CoordLib
{
  // Thank you (no license attached AFAIK):
  // https://github.com/GoodOlClint/Astronomy

  [TestClass]
  public class JDNTests
  {

    [TestMethod( )]
    public void TestDateToJulianDayNumber( )
    {
      double expected = 2436116.31;
      double actual = new JulianDay( 1957, 10, 4, 19, 26, 24 ).JulianDayNumber;
      Assert.AreEqual( expected, actual );
    }

    [TestMethod( )]
    public void TestDateToJulianDayNumber2( )
    {
      double expected = 1842713.0;
      double actual = new JulianDay( 333, 1, 27, 12, 0, 0 ).JulianDayNumber;
      Assert.AreEqual( expected, actual );
    }

    [TestMethod( )]
    public void TestJulianDayNumberToDate( )
    {
      double JD = 2436116.31;
      JulianDay expected = new JulianDay( 1957, 10, 4, 19, 26, 24 );
      JulianDay actual = new JulianDay( JD );
      Assert.AreEqual( expected, actual );
    }

    [TestMethod( )]
    public void TestJulianDayNumberToDate2( )
    {
      double JD = 1842713.0;
      JulianDay expected = new JulianDay( 333, 1, 27, 12, 0, 0 );
      JulianDay actual = new JulianDay( JD );
      Assert.AreEqual( expected, actual );
    }

    [TestMethod( )]
    public void TestJulianDayNumberToDate3( )
    {
      double JD = 1507900.13;
      JulianDay expected = new JulianDay( -584, 5, 28, 15, 07, 12 );
      JulianDay actual = new JulianDay( JD );
      Assert.AreEqual( expected, actual );
    }

    [TestMethod( )]
    public void TestDayOfYear( )
    {
      JulianDay JD = new JulianDay( 1978, 11, 14 );
      int expected = 318;
      int actual;
      actual = JD.DayOfYear;
      Assert.AreEqual( expected, actual );
    }


    [TestMethod( )]
    public void TestOperators( )
    {
      JulianDay JD = new JulianDay( 1978, 11, 14 );
      JulianDay expected = new JulianDay( 1978, 11, 15 );
      Assert.AreEqual( expected, (JD + 1.0) ); // add days

      expected = new JulianDay( 1978, 11, 13 );
      Assert.AreEqual( expected, (JD - 1.0) ); // sub days


      JulianDay arg2 = new JulianDay( JD );
      Assert.AreEqual( true, JD == arg2 );
      Assert.AreEqual( true, JD <= arg2 );
      Assert.AreEqual( true, JD >= arg2 );

      arg2 = new JulianDay( 1978, 11, 15 );
      Assert.AreEqual( true, JD <= arg2 );
      arg2 = new JulianDay( 1978, 11, 15 );
      Assert.AreEqual( true, JD < arg2 );

      arg2 = new JulianDay( 1978, 11, 13 );
      Assert.AreEqual( true, JD >= arg2 );
      Assert.AreEqual( true, JD > arg2 );
    }

  }
}
