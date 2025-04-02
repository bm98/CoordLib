using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using CoordLib;
using CoordLib.MercatorTiles;
using CoordLib.Extensions;
using System.Diagnostics;

namespace TEST_CoordLib
{
  public partial class Form1 : Form
  {
    public Form1( )
    {
      InitializeComponent( );
    }

    private class Item : IEquatable<Item>
    {
      public int ID { get; set; }
      public string QString { get; set; }
      public Quad IQuad { get; set; }

      public bool Equals( Item other )
      {
        return other.ID == ID
          && other.QString == QString
          && other.IQuad == IQuad;
      }
    }

    // deterministic
    private void LoadLookupItemsDet( )
    {
      RTB.Text = "";
      Quad q; Item item; int id = 1;
      for (int y = 0; y < 10; y++) {
        q = new Quad( new Point( 1, y ), 10 ); item = new Item( ) { ID = id++, QString = q.ToString( ), IQuad = q }; _lookup.Add( item.IQuad, item );
        RTB.Text += $"{q}\n";
      }
      for (int y = 0; y < 10; y++) {
        q = new Quad( new Point( 10, y ), 10 ); item = new Item( ) { ID = id++, QString = q.ToString( ), IQuad = q }; _lookup.Add( item.IQuad, item );
        RTB.Text += $"{q}\n";
      }
      for (int y = 0; y < 10; y++) {
        q = new Quad( new Point( 100, y ), 10 ); item = new Item( ) { ID = id++, QString = q.ToString( ), IQuad = q }; _lookup.Add( item.IQuad, item );
        RTB.Text += $"{q}\n";
      }
      for (int y = 0; y < 10; y++) {
        q = new Quad( new Point( 1000, y ), 10 ); item = new Item( ) { ID = id++, QString = q.ToString( ), IQuad = q }; _lookup.Add( item.IQuad, item );
        RTB.Text += $"{q}\n";
      }

    }

    private void LoadLookupItems( ushort maxZoom )
    {
      for (int i = 15; i < 115; i++) {
        var q = new Quad( new Point( i * 13, i * 7 ), maxZoom );
        var item = new Item( ) { ID = i, QString = q.ToString( ), IQuad = q };
        _lookup.Add( item.IQuad, item );
      }
    }

    QuadLookup<Item> _lookup;

    private void btLookup1_Click( object sender, EventArgs e )
    {
      _lookup = new QuadLookup<Item>( 1, 4, 10 );
      LoadLookupItemsDet( );
    }

    private void btLookup2_Click( object sender, EventArgs e )
    {
      var qq = new Quad( "11111" );
      var res = _lookup.GetItemsWhichArePartOf( qq );
    }

    private void button1_Click( object sender, EventArgs e )
    {
      LatLon latLon = new LatLon( 12.34, 56.75 );
      LatLon latLon1 = new LatLon( latLon );

      if (latLon == latLon1) {
        RTB.Text += $"{latLon} == {latLon1}:  EQUALITY MATCH\n";
      }
      if (latLon != latLon1) {
        RTB.Text += $"{latLon} != {latLon1}:  INEQUALITY FAILED\n";
      }

      latLon1.Lat += 0.01;
      if (latLon == latLon1) {
        RTB.Text += $"{latLon} == {latLon1}:  EQUALITY FAILED\n";
      }
      if (latLon != latLon1) {
        RTB.Text += $"{latLon} != {latLon1}:  INEQUALITY MATCH\n";
      }
    }


    private void btMagVar_Click( object sender, EventArgs e )
    {
      Stopwatch _timer1 = new Stopwatch( );

      CoordLib.WMM.MagVarEx.MagFromTrueBearing( double.Parse( txMVbearing.Text ), new LatLon( 46.5, 7.25 ), true ); // init lookup value

      // direct result
      lblMVresult.Text = CoordLib.WMM.MagVarEx.MagFromTrueBearing( double.Parse( txMVbearing.Text ), new LatLon( 46.5, 7.25 ), cbxUseTable.Checked ).ToString( );

      // loops
      _timer1.Start( );
      double res;
      for (int i = 0; i < 100_000; i++) {
        res = CoordLib.WMM.MagVarEx.MagFromTrueBearing( double.Parse( txMVbearing.Text ), new LatLon( 46.5, 7.25 ), true );
      }
      _timer1.Stop( );
      RTB.Text += $"Lookup 100_000x => {_timer1.ElapsedMilliseconds} ms\n";

      _timer1.Start( );
      for (int i = 0; i < 100_000; i++) {
        res = CoordLib.WMM.MagVarEx.MagFromTrueBearing( double.Parse( txMVbearing.Text ), new LatLon( 46.5, 7.25 ), false );
      }
      _timer1.Stop( );
      RTB.Text += $"Calc   100_000x => {_timer1.ElapsedMilliseconds} ms\n";

      // RESULTS 
      /*
          Lookup 100_000x => 225 ms
          Calc   100_000x => 1055 ms
       */
    }

    private void btQuadList_Click( object sender, EventArgs e )
    {
      for (int i = 0; i < 360; i++) {
        for (int j = 0; j <= 0; j++) {
          RTB.Text += $"LatLon {j:00}-{i:000}  QuadMax: {Quad.LatLonToQuad( new LatLon( j, i ), 9 )}\n";
        }
      }
    }

    // calc the dist of one degree at various latitudes
    private void btDistTable_Click( object sender, EventArgs e )
    {


      RTB.Text = $"LAT,dLon,dLat\n";
      for (int i = 0; i < 90; i++) {
        var ll = new LatLon( i, 0 );
        var distLat = ll.DistanceTo( new LatLon( i + 1, 0 ), ConvConsts.EarthRadiusM ); // lat dist [m]
        var distLon = ll.DistanceTo( new LatLon( i, 1 ), ConvConsts.EarthRadiusM ); // lon dist  [m]
        //RTB.Text += $"{i,2:0},{distLon:#0.0},{distLat:#0.0}\n";
        //RTB.Text += $"{distLon:#0.0},\n";
        RTB.Text += $"{ConvConsts.LonDistPerDeg( i, ConvConsts.EarthRadiusNm ):#0.0000},\n";

      }
    }

    private void btCalcMvFromLatLon_Click( object sender, EventArgs e )
    {
      txMv.Text = CoordLib.WMM.MagVarEx.MagVar_deg(
        new LatLon( double.Parse( txLat.Text ), double.Parse( txLon.Text ) ),
        new DateTime( 2025, 1, 1, 12, 0, 0 ),
        false ).ToString( "G5" );
    }
  }
}
