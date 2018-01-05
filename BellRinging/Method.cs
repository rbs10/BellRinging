using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BellRinging
{
  /// <summary>
  /// 
  /// </summary>
  public class Method
  {
    SequenceOfPermutations _corePermutations;
    Permutation _plainLeadEndPermutation;

    List<Permutation> _leadHeadChanges = new List<Permutation>();
    List<string> _leadHeadCallNames = new List<string>();
    string _name;
    string _code;
    string _changeNotation;

    public SequenceOfPermutations CorePermuations { get { return _corePermutations; } }
    //public Permutation PlainLeadEndPermutation { get { return _plainLeadEndPermutation; } }
    public List<Permutation> LeadHeadChanges
    {
      get
      {
        return _leadHeadChanges;
      }
  }

    public string Name
    {
      get
      {
        return _name;
      }
    }

    public string Letter
    {
      get
      {
        return _code;
      }
    }

    string _variableHuntCall;
    
    public Method(string name, string code, string changeNotation, int noBells,bool allowSingles = true, string variableHuntCall = null )
    {
      _name = name;
      _code = code;
      _variableHuntCall = variableHuntCall;
      _changeNotation = changeNotation;

      if (changeNotation == null)
      {
          _corePermutations = new SequenceOfPermutations(null, 8);

          //_plainLeadEndPermutation = Permutation.FromPlaceNotation("12", 8);
          _plainLeadEndPermutation = Permutation.FromPlaceNotation("18", 8);
         
      }
      else
      {
          int whereIsDash = changeNotation.IndexOf('-');
          if (whereIsDash < 0)
          {
              throw new Exception("Only symmetric methods defined up to the half lead followed by - and the lead and are accepted - not [" + changeNotation + "]");
              //_corePermutations = new SequenceOfPermutations(changeNotation, noBells);
          }
          else
          {
              _corePermutations = new SequenceOfPermutations(changeNotation.Substring(0, whereIsDash), noBells);
              _corePermutations.ReflectAboutFinalPermutation();
          }

          _plainLeadEndPermutation = Permutation.FromPlaceNotation(changeNotation.Substring(whereIsDash + 1).Replace(" ", ""), noBells);


      }
      _leadHeadChanges.Add(_plainLeadEndPermutation);
      _leadHeadCallNames.Add(" ");
      //_leadHeadChanges.Add(Permutation.FromPlaceNotation("16", 8));
      if (variableHuntCall == null)
      {
          _leadHeadChanges.Add(Permutation.FromPlaceNotation("14", 8));
          _leadHeadCallNames.Add("b");
      }
        if (allowSingles)
          {

              _leadHeadChanges.Add(Permutation.FromPlaceNotation("1234", 8));
              _leadHeadCallNames.Add("s");
            //  _leadHeadChanges.Add(Permutation.FromPlaceNotation("14", 8));
          }
      if (variableHuntCall != null)
      {
          _leadHeadChanges.Add(Permutation.FromPlaceNotation(variableHuntCall, 8));
          _leadHeadCallNames.Add("x");
      }

    }

      public string DetailsString
    {
          get
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
              foreach ( var lec in _leadHeadChanges )
              {

            sb.Append(" ");
            sb.Append(lec.ToString());
              }
            return sb.ToString();
        }
    }

     public int WrongPlaceCount
    {
        get
        {
            int ret = 0;
            for (int i = 0; i < _corePermutations.Permuations.Count; i += 2)
            {
                var perm = _corePermutations.Permuations[i];
                if ( !perm.IsCross)
                {
                    ++ret;
                }
            }
          return ret;
        }
    }
    /// <summary>
    /// Convert lead to one with a snap start instead
    /// </summary>
    public void SnapStart()
    {
        _corePermutations.RemoveFirst(2);
        IsSnapStart = true;
    }

    /// <summary>
    /// Convert lead to one with a snap start instead
    /// </summary>
    public void FirstLeadOnly()
    {
        //_corePermutations.RemoveFirst(2);
        IsFirstLead = true;
    }

  /// <summary>
    /// Only have coming into rounds at hand
    /// </summary>
    public void LastLeadOnly()
    {
        //_corePermutations.RemoveFirst(2);
        IsLastLeadAtHand = true;
    }

    /// <summary>
    /// Flag set for special handling of first lead
    /// </summary>
    public bool IsFirstLead { get; private set; }

    /// <summary>
    /// Flag set for special handling of first lead
    /// </summary>
    public bool IsLastLeadAtHand { get; private set; }

    /// <summary>
    /// Flag set on snap start for special handling
    /// </summary>
    public bool IsSnapStart { get; private set; }

    Dictionary<int, Lead> _allLeads;

    public Permutation PlainLeadEndPermutation
    {
      get
      {
        return _plainLeadEndPermutation;
      }
    }
    public IEnumerable<Lead> AllLeads
    {
      get
      {
        InitAllLeadsIfRequired();
        return _allLeads.Values;
      }
    }

    private void InitAllLeadsIfRequired()
    {
      if (_allLeads == null)
      {
        //return GeneratePlainCourse(new Row(8));

        // treble front, tenor back 
        //
        // 720 course ends
        //
        // or just all leads - harder to keep tenor together
        _allLeads = new Dictionary<int, Lead>();

        if (IsSnapStart||IsFirstLead)
        {
            GenerateFirstLead(new Row(8), _allLeads);
        }
        else if ( _variableHuntCall != null )
        {
            foreach (var row in Row.AllRows )
            {
                _allLeads.Add(row.ToNumber(), GenerateLead(row));
            }
        }
        else
        {
            GenerateAllLeads(new Row(8), 6, _allLeads);
        }
          if ( IsLastLeadAtHand )
          {
              //_allLeads = _allLeads. Where(kvp => kvp.Value.ContainsRoundsAt == 31).
              //    ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

              
              //_allLeads = _allLeads.Where(kvp => kvp.Value.LeadHead().CoursingOrder() == "642357").
              //    ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

              _allLeads =_allLeads.Where(kvp => kvp.Value.RowsAsInts.Contains(0)).
                 ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


          }
      }
    }

    public Lead Lead(int leadHead)
    {
      InitAllLeadsIfRequired();
      return _allLeads[leadHead];
    }

    public Lead Lead(Row leadHead)
    {
      return Lead(leadHead.ToNumber());
    }

    /// <summary>
    /// Generate all leads of method by recursively rotating numbers of bells to generate all
    /// lead heads with one bell fixed and then generating plain courses from those leads ends
    /// </summary>
    /// <remarks>
    /// Breaks down if plain course is not n-1 bells long. Course of snap-leads is 10 leads long so
    /// no good there. 
    /// Only really want the first lead of snap anyway!
    /// </remarks>
    private  void GenerateAllLeads(Row startRow, int bellsToRotate, Dictionary<int, Lead> allLeads)
    {
      if (bellsToRotate > 1)
      {
        Permutation p = Permutation.FromRotation(1, bellsToRotate, 8);
        Row row = startRow;
        for (int i = 0; i < bellsToRotate; ++i)
        {
          GenerateAllLeads(row, bellsToRotate - 1, allLeads);
          row = row.Apply(p);
        }
      }
      else
      {
        //Console.WriteLine(startRow);
        foreach (Lead l in GeneratePlainCourse(startRow))
        {
          allLeads.Add(l.LeadHead().ToNumber(),l);
        }
      }
    }

    private void GenerateFirstLead(Row startRow, Dictionary<int, Lead> allLeads)
    {
        var l = GeneratePlainCourse(startRow).First();
        allLeads.Add(l.LeadHead().ToNumber(), l);
    }

    public List<Lead> GeneratePlainCourse(Row startRow)
    {
      List<Lead> leads = new List<Lead>();

      Row row = startRow;
      //Console.WriteLine(startRow);
      do
      {
        //Console.WriteLine(row);
        List<Row> restOfCourse = _corePermutations.Apply(row);
        restOfCourse.Insert(0, row); // include the start row in the lead

        leads.Add(new Lead(this, restOfCourse));
        row = restOfCourse[restOfCourse.Count - 1].Apply(_plainLeadEndPermutation);
      }
      while (!(row.Equals(startRow)));
      return leads;
    }

    public Lead GenerateLead(Row startRow)
    {
        Row row = startRow;
        //Console.WriteLine(startRow);
      
            //Console.WriteLine(row);
            List<Row> restOfCourse = _corePermutations.Apply(row);
            restOfCourse.Insert(0, row); // include the start row in the lead

            return new Lead(this, restOfCourse);
    }

    public IEnumerable<Row> Rows(int lead)
    {
      Row row = Row.FromNumber(lead);
      // the first row is always included
      yield return row;
      foreach (Row r in _corePermutations.Apply(row))
      {
        if (!r.IsRounds())
        {
          yield return r;
        }
        else
        {
          // stop and do not return rounds
          yield break;
        }
      }
    }

    internal void WriteMicroSiril(StringBuilder sb)
    {
        if (_changeNotation != null)
        {
            int i = 0;
            foreach ( var leadEndName in _leadHeadCallNames )
            {
                sb.AppendLine(string.Format("{0}{1} = {2}, +{3}",this.Letter, leadEndName, Name, _leadHeadChanges[i++].ToString()));
            }

            var leadEndBreak = _changeNotation.IndexOf('-');
            string microSirilNotation = "&" + _changeNotation.Replace("X", "-").Substring(0, leadEndBreak);
            // Cambridge = &-3-4-25-36-4-5-6-7
            sb.AppendLine(string.Format("{0} = {1}", Name, microSirilNotation));
        }
        else
        {
            string microSirilNotation = "12";
            sb.AppendLine(string.Format("{0} = +{1}", Name, microSirilNotation));
        }
    }

      internal string WriteChoiceMicroSiril(int call)
    {
        string ret;
        if (_changeNotation != null)
        {
            ret = Letter + _leadHeadCallNames[call];
        }
        else
        {
            ret = Name;
        }
        return ret;
    }

      internal string CallName(int call)
      {
          string ret;
          if (_changeNotation != null)
          {
              ret = _leadHeadCallNames[call];
          }
          else
          {
              // "start at hand"
              ret = "@";
          }
          return ret;
      }
  }
}
