using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CoordLib;
using CoordLib.MercatorTiles;
using CoordLib.Extensions;

namespace TEST_CoordLib
{
  public partial class Form1 : Form
  {
    public Form1( )
    {
      InitializeComponent( );
    }

    private class Item
    {
      public int ID { get; set; }
      public string QString { get; set; }
      public Quad IQuad { get; set; }
    }
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
        RTB.Text = "EQUALITY MATCH";
      }
      if (latLon != latLon1) {
        RTB.Text = "INEQUALITY FAILED";
      }

      latLon1.Lat += 0.01;
      if (latLon == latLon1) {
        RTB.Text = "EQUALITY FAILED";
      }
      if (latLon != latLon1) {
        RTB.Text = "INEQUALITY MATCH";
      }
    }


    private void btMagVar_Click( object sender, EventArgs e )
    {
      lblMVresult.Text = CoordLib.WMM.MagVarEx.MagFromTrueBearing( double.Parse( txMVbearing.Text ), new LatLon( 46.5, 7.25 ), cbxUseTable.Checked ).ToString( );

//      lblMVresult.Text = CoordLib.WMM.MagVarEx.MagVar_deg( new LatLon( 80, 0 ), false ).ToString( );

    }
  }
}
