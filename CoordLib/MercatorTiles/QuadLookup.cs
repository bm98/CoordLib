using System;
using System.Collections.Generic;
using System.Linq;

namespace CoordLib.MercatorTiles
{
  /// <summary>
  /// Lookup tree for Quad tagged items
  /// 
  ///  The lookup is defined by the minimum and maximum zoom level and the branch limit
  ///  
  ///  Mimimum zoom omits traversing the first levels on the cost of searching the first
  ///  entry point by the number of items of this level (max - see comment below)
  ///  i.e. starting at L4 would result in a maximum of 256 entry points which need to 
  ///  be found by a linear albeit indexed search (Dictionary Key lookup)
  ///  
  ///  The tree will expand to a next level when the max number of leave items (branch limit) is exceeded
  ///  except if we have reached the maximum zoom, there all items will be stored regardless
  ///  of its count.
  ///  
  ///  Geo features are seldom distributed evenly accross the globe
  ///   so the tree will never be a balanced one
  ///   hence a deeper level than the anticipated max number of items
  ///   would suggest is needed in order to balance the linear search at 
  ///   the end element versus another level of depth
  /// 
  ///  Quads are base4 items
  ///  L1           4
  ///  L2          16
  ///  L3          64
  ///  L4         256
  ///  L5       1'024
  ///  L6       4'096
  ///  L7      16'384
  ///  L8      65'536
  ///  L9     262'144
  ///  L10  1'048'576
  ///  L11  4'194'304
  ///  L12 16'777'216
  ///  L13 67'108'864
  ///  Ln       4^n
  ///  
  /// From testing the complete MSFS Aiport DB (~40'677 items Added and ~22'612 PartOf Querys at zoom 12 to find an Airport)
  /// with a QLookup(3,11,  32) will reach maximum depth at L11 / Add:  0.68% PartOf: 0.53% - TOT 1.21% CPU time
  /// with a QLookup(3,10,  64) will reach maximum depth at L10 / Add:  0.69% PartOf: 0.67% - TOT 1.36% CPU time
  /// with a QLookup(3,10, 128) will reach maximum depth at L9  / Add:  0.63% PartOf: 0.98% - TOT 1.61% CPU time
  /// with a QLookup(3,10, 256) will reach maximum depth at L9  / Add:  0.61% PartOf: 1.61% - TOT 2.22% CPU time
  /// with a QLookup(3,10, 512) will reach maximum depth at L8  / Add:  0.43% PartOf: 2.28% - TOT 2.71% CPU time
  /// with a QLookup(3,10,1024) will reach maximum depth at L7  / Add:  0.37% PartOf: 4.89% - TOT 5.26% CPU time
  /// 
  /// ~450 kByte was allocated for the Lookup Tables 
  ///   each Item record contains a quad string of 22 chars + Airport reference
  ///   + Level Dictionary overhead
  /// 
  /// </summary>
  public class QuadLookup<T> //where T : class
  {
    // root of the tree
    private QLevel _root;

    // from cTor
    private int _minZoom;
    private int _maxZoom;
    private int _branchLimit;

    // dynamics
    private int _maxLevel;
    private int _count;

    /// <summary>
    /// Returns the number of items stored
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Returns the maximum zoom level used
    /// </summary>
    public int MaxLevel => _maxLevel;

    /// <summary>
    /// A lookup item
    /// </summary>
    private struct LookupItem
    {
      public Quad QuadKey;
      public T Item;
      public LookupItem( Quad q, T item )
      {
        QuadKey = q;
        Item = item;
      }
    }

    // A Level of the tree
    private class QLevel
    {
      private Dictionary<Quad, QLevel> _nextLevel = null;
      private List<LookupItem> _content = null;

      private readonly ushort _zoom = 0;
      private int _maxZoom;
      private int _branchLimit;

      // end of the tree mark
      bool _EOT;

      /// <summary>
      /// Create a Tree up to Max with Step increments to the next level
      ///  e.g. start 1, max 5, step 2 -> 1,3,5 
      /// </summary>
      /// <param name="thisZoom">Zoom for this level</param>
      /// <param name="maxZoom">MaxZoom where content is stored</param>
      /// <param name="branchLimit">Max number of items in intermediate branches (MaxZoom..4^MaxZoom)</param>
      public QLevel( int thisZoom, int maxZoom, int branchLimit )
      {
        _zoom = (ushort)thisZoom;
        _maxZoom = maxZoom;
        _branchLimit = branchLimit;
        // per default we store here, extension is done in Add
        // so create the content container
        _EOT = true;
        _content = new List<LookupItem>( );
      }

      /// <summary>
      /// Return ALL items know to this Level
      /// which includes all sublevels
      /// </summary>
      public IList<T> Items {
        get {
          var ret = new List<T>( );
          if (_EOT) {
            return _content.Select( x => x.Item ).ToList( );
          }
          else {
            // all below
            /*
            foreach (var nLevel in _nextLevel) {
              ret.AddRange( nLevel.Value.Items );
            }
            */
            return _nextLevel.SelectMany( x => x.Value.Items ).ToList( );
          }
          //return ret;
        }
      }

      /// <summary>
      /// Add an item based on a Quad
      /// </summary>
      /// <param name="quad">Quad tag</param>
      /// <param name="item">The item to store</param>
      /// <returns>The zoom level where the item was stored</returns>
      public int Add( Quad quad, T item )
      {
        Quad qkey = quad.AtZoom( _zoom );
        if (_EOT) {
          if (_zoom == _maxZoom || _content.Count < _branchLimit) {
            // end of tree - add here with a Quad
            _content.Add( new LookupItem( quad, item ) );
            return _zoom;
          }
          // extend to a further level
          _nextLevel = new Dictionary<Quad, QLevel>();
          foreach (var i in _content) {
            Quad iKey = i.QuadKey.AtZoom( _zoom );
            if (!_nextLevel.ContainsKey( iKey )) {
              // create the level if needed
              _nextLevel.Add( iKey, new QLevel( _zoom + 1, _maxZoom, _branchLimit ) );
            }
            _nextLevel[iKey].Add( i.QuadKey, i.Item );
          }
          // make this an intermediate branch
          _content = null;
          _EOT = false;
        }

        // add the Input item to a leave
        if (!_nextLevel.ContainsKey( qkey )) {
          // create the level if needed
          _nextLevel.Add( qkey, new QLevel( _zoom + 1, _maxZoom, _branchLimit ) );
        }
        return _nextLevel[qkey].Add( quad, item );
      }

      /// <summary>
      /// Attempts to retrieve items that are part of the argument
      /// Implies that the argument is at a lower or equal level than the maxZoom of the tree
      /// </summary>
      /// <param name="queryQ">A Quad, cannot be more detailed than the MaxZoom</param>
      /// <returns>A list of items</returns>
      public IList<T> GetItemsWhichArePartOf( Quad queryQ )
      {
        // it is ensured that the query is not more detailed than the max zoom level
        // answers may need to include all of the lower levels if there are

        if (queryQ.ZoomLevel > _zoom) {
          // cannot answer the query here; it is more detailed - try one down
          Quad qkey = queryQ.AtZoom( _zoom );
          if (!_EOT && _nextLevel.ContainsKey( qkey )) {
            // there is one down - return it's answer
            return _nextLevel[qkey].GetItemsWhichArePartOf( queryQ );
          }
        }

        // not done above, the query is to be answered here
        var ret = new List<T>( );
        if (_EOT) {
          // we only have content - dig it out
          /*
          foreach (var ll in _content.Where( x => x.QuadKey.IsPartOf( queryQ ) ){
            ret.Add( ll.Item );
          }
          */
          return _content.Where( x => x.QuadKey.IsPartOf( queryQ ) ).Select( y => y.Item ).ToList( );
        }
        // finally we are a branch return all leaves which match the partOf criteria
        foreach (var nLevel in _nextLevel) {
          if (nLevel.Key.IsPartOf( queryQ ))
            ret.AddRange( nLevel.Value.Items );
        }
        return ret;
      }

      /// <summary>
      /// Attempts to return an item where the argument is included
      /// Implies that the argument is at a higher or equal zoom level than the maxZoom of the tree
      /// </summary>
      /// <param name="queryQ">A Quad, must be more detailed than the MaxZoom</param>
      /// <returns>An Item that includes the query Quad or null</returns>
      public T GetItemWhichIncludes( Quad queryQ )
      {
        // it is ensured that the query is always more detailed than the max zoom level
        // so it can only be answered in the lowest level

        if (_EOT) {
          // we only have content - dig it out if we find one
          var selection = _content.Where( x => x.QuadKey.Includes( queryQ ) );
          return (selection.Count( ) > 0) ? selection.Select( y => y.Item ).FirstOrDefault( ) : default( T );
        }

        // we are a branch - find the more detailed branch where the query must be resolved
        Quad qkey = queryQ.AtZoom( _zoom );
        if (_nextLevel.ContainsKey( qkey )) {
          // there is one down - return it's answer
          return _nextLevel[qkey].GetItemWhichIncludes( queryQ );
        }
        // nothing to be found
        return default( T );
      }


    }// Level


    /// <summary>
    /// Create a Tree from Min up to Max zoom levels
    /// </summary>
    /// <param name="startZoom">Zoom for this level (1..Projection.MaxZoom) </param>
    /// <param name="maxZoom">MaxZoom where content is stored (1..Projection.MaxZoom)</param>
    /// <param name="branchLimit">Max number of items in intermediate branches (MaxZoom..4^MaxZoom)</param>
    public QuadLookup( int startZoom, int maxZoom, int branchLimit )
    {
      _minZoom = startZoom;
      _maxZoom = maxZoom;
      _branchLimit = branchLimit;
      // some sanity..
      if (_minZoom < 1) _minZoom = 1;
      if (_minZoom > Projection.MaxZoom) _minZoom = Projection.MaxZoom;
      if (_maxZoom < 1) _maxZoom = 1;
      if (_maxZoom > Projection.MaxZoom) _maxZoom = Projection.MaxZoom;
      if (_branchLimit < _maxZoom) _branchLimit = _maxZoom;
      var max = Math.Pow( 4, _maxZoom );
      if (_branchLimit > max) _branchLimit = (int)max;

      // reset all
      _maxLevel = 0;
      _count = 0;
      _root = new QLevel( _minZoom, _maxZoom, _branchLimit );
    }

    /// <summary>
    /// Add an item based on a Quad
    /// NOTE: the Quad depth (zoom level) must be >= MaxZoom of the tree - will throw an ArgumentException
    /// </summary>
    /// <param name="quad">Quad tag</param>
    /// <param name="itemRef">The item to store, must be a class (ref only)</param>
    public void Add( Quad quad, T itemRef )
    {
      // sanity
      if (quad.ZoomLevel < _maxZoom) throw new ArgumentException( $"The zoom level of the Quad ({quad.ZoomLevel}) is less than the MaxZoom ({_maxZoom}) of this Lookup" );

      var m = _root.Add( quad, itemRef );
      _maxLevel = (m > _maxLevel) ? m : _maxLevel; // store the max level
      _count++;
    }

    /// <summary>
    /// Attempts to retrieve items that are part of the argument
    /// Implies that the argument is at a lower or equal level than the maxZoom of the tree
    /// </summary>
    /// <param name="queryQ">A Quad, cannot be more detailed than the MaxZoom</param>
    /// <returns>A list of items</returns>
    public IList<T> GetItemsWhichArePartOf( Quad queryQ )
    {
      // sanity
      if (queryQ.ZoomLevel > Projection.MaxZoom) throw new ArgumentException( $"The zoom level of the Quad ({queryQ.ZoomLevel}) is more than the MaxZoom ({Projection.MaxZoom}) of this Lookup" );

      return _root.GetItemsWhichArePartOf( queryQ );
    }


    /// <summary>
    /// Attempts to return a list of items where the argument is included
    /// Implies that the argument is at a higher or equal zoom level than the maxZoom of the tree
    /// </summary>
    /// <param name="queryQ">A Quad, must be more detailed than the MaxZoom</param>
    /// <returns>An Item that includes the query Quad or null</returns>
    public T GetItemWhichIncludes( Quad queryQ )
    {
      // sanity
      if (queryQ.ZoomLevel < _maxZoom) throw new ArgumentException( $"The zoom level of the Quad ({queryQ.ZoomLevel}) is less than the MaxZoom ({_maxZoom}) of this Lookup" );

      return _root.GetItemWhichIncludes( queryQ );
    }

  }
}
