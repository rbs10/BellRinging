using System;
using System.Collections.Generic;
using System.Text;

namespace BellRinging
{
  /// <summary>
  /// Definition of what we want to solve - methods, constraints
  /// </summary>
  public class Problem
  {
    List<Method> _methods = new List<Method>();

    bool _tenorsTogether;

    int _minRows;

    int _maxRows;

    int _maxLeads;

    public Problem()
    {
    }

    public void AddMethod(Method m)
    {
      _methods.Add(m);
    }

    public IEnumerable<Method> Methods
    {
      get
      {
        return _methods.AsReadOnly();
      }
    }

    public bool AllowSingles { get; set; }

    public bool TenorsTogether
    {
      get
      {
        return _tenorsTogether;
      }
      set
      {
        _tenorsTogether = value;
      }
    }

    public int MaxLeads
    {
        get
        {
            return _maxLeads;
        }
        set
        {
            _maxLeads = value;
        }
    }

      /// <summary>
      /// The choice to start thinking about first at the start of the composition
      /// </summary>
    public int FirstChoice { get; set; }

    public MusicalPreferences MusicalPreferences { get; set; }

    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }

    public int MinLeads { get; set; }

    public bool Reverse { get; set; }

    public int BlockLength { get; set; }

    public bool VariableHunt { get; set; }

    public int MusicDelta { get; set; }

    public bool RotateCompositions { get; set; }

    public bool ExcludeUnrungMethodsFromBalance { get; set; }

    public int MaxCalls { get; set; }
  }
}
