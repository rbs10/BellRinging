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
    string _name;
    string _code;

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
    
    public Method(string name, string code, string changeNotation, int noBells )
    {
      _name = name;
      _code = code;

      int whereIsDash = changeNotation.IndexOf('-');
      if (whereIsDash < 0)
      {
        throw new Exception("Only symmetric methods defined up to the half lead followed by - and the lead and are accepted - not ["
+ changeNotation + "]");
      }
      _corePermutations = new SequenceOfPermutations(changeNotation.Substring(0, whereIsDash),noBells);
      _corePermutations.ReflectAboutFinalPermutation();

      _plainLeadEndPermutation = Permutation.FromPlaceNotation(changeNotation.Substring(whereIsDash + 1).Replace(" ", ""), noBells);

      _leadHeadChanges.Add(_plainLeadEndPermutation);
      //_leadHeadChanges.Add(Permutation.FromPlaceNotation("16", 8));
      _leadHeadChanges.Add(Permutation.FromPlaceNotation("14",8));
      _leadHeadChanges.Add(Permutation.FromPlaceNotation("1234",8));
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
    /// Flag set for special handling of first lead
    /// </summary>
    public bool IsFirstLead { get; private set; }
 
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
        else
        {
            GenerateAllLeads(new Row(8), 6, _allLeads);
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

    public IEnumerable<Row> Rows(short lead)
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
  }
}
