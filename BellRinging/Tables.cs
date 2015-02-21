using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BellRinging
{
  /// <summary>
  /// Tables associated with a problem. Access to tables is public to avoid an excessive performance hit
  /// in clients and to simplify migration to this class.
  /// </summary>
  public class Tables
  {
    public  int NO_CHOICES;
    public  int NO_LEADENDS = 5040;

    public int MAX_LEADS;
    bool modelInternalRounds = true; // set to true if looking for snap touches, must be false for rotational composer

    // for each lead end where the next lead given a specified choice
    public short[,] leadMapping;
    // for each lend end the previous lead end via a specifed choice
    public short[,] reverseLeadMapping;
    // for each lead end and choice the musical score (depends weakly on the bob/single aspect
    // of the choice
    public short[,] music;

    // for each leadHead the other leadHeads it is false against
    public short[,,][] falseLeadHeads;
    public short[,,][] falseMethods;
    //public int[] falseCount;

    // for each lead head how many leads until rounds (= 0 for rounds)
    public short[] leadsToEnd;

    // the maximum music in any lead
    public short maxMusic = 0;

    // the method index associated with each choice
    public int[] methodIndexByChoice;

    public Method[] _methodsByChoice;
    bool[] isCall;
    int[] callIndex;
    Permutation[] _leadHeadPermutations;
    Dictionary<Method, int> _methodIndex = new Dictionary<Method, int>();

    int nMethods;
    void ComputeChoices(Problem problem)
    {
      NO_CHOICES = 0;
      MAX_LEADS = problem.MaxLeads;
      nMethods = 0;
      foreach (Method m in problem.Methods)
      {
        _methodIndex[m] = nMethods;
        ++nMethods;

        foreach (Permutation p in m.LeadHeadChanges)
        {
          ++NO_CHOICES;
        }
      }

      _methodsByChoice = new Method[NO_CHOICES];
      _leadHeadPermutations = new Permutation[NO_CHOICES];
      isCall = new bool[NO_CHOICES];
      callIndex = new int[NO_CHOICES];
      methodIndexByChoice = new int[NO_CHOICES];
      int idx = 0;
      int iMethod = 0;
      foreach (Method m in problem.Methods)
      {
        bool bFirst = true;
        foreach (Permutation p in m.LeadHeadChanges)
        {
          isCall[idx] = !bFirst;
          if (bFirst)
          {
            callIndex[idx] = 0;
          }
          else
          {
            callIndex[idx] = callIndex[idx-1]+1;
          }
          _methodsByChoice[idx] = m;
          methodIndexByChoice[idx] = iMethod;
          _leadHeadPermutations[idx] = p;
          ++idx;
          bFirst = false;
        }
        ++iMethod;
      }
    }

    public System.Threading.ManualResetEventSlim readyToCreateComposition = new System.Threading.ManualResetEventSlim(false);

    public void Initialise(Problem problem)
    {
        this.problem = problem;

      DateTime startInit = DateTime.UtcNow;

      ComputeChoices(problem);

      readyToCreateComposition.Set();

      ClearArrays();
      MusicalPreferences musicalPreferences = problem.MusicalPreferences;

      int methodIndex = 0;
      foreach (Method method in problem.Methods)
      {
          ++methodIndex;
        IEnumerable<Lead> allLeads = method.AllLeads;
        //Console.WriteLine("Generated all leads " + (DateTime.UtcNow - startInit));

        // index the leads
        foreach (Lead l in allLeads)
        {
          if (IncludeLeadHead(l.LeadHead(), methodIndex) || method.Name == "Null")
          {
            short num = l.ToNumber();
            for (int i = 0; i < _methodsByChoice.Length; ++i)
            {
              if (_methodsByChoice[i] == method)
              {
                  Row nextLeadHead = null;
                  if (modelInternalRounds)
                  {
                      if (l.ContainsRoundsAt <= 0) // does not contain rounds internally
                      {
                          nextLeadHead = l.NextLeadHead(_leadHeadPermutations[i]);
                      }
                      else
                      {
                          // note that this is a route to the end - distance = 1 as still have to choose this lead
                          leadsToEnd[num] = 1;
                          
                          // at least for reverse composition when need to consider this
                          nextLeadHead = l.NextLeadHead(_leadHeadPermutations[i]);

                          // no call where comes round internally!
                          if (_leadHeadPermutations[i] != _methodsByChoice[i].PlainLeadEndPermutation)
                          {
                              nextLeadHead = null;
                          }
                      }
                  }
                  else
                  {

                      nextLeadHead = l.NextLeadHead(_leadHeadPermutations[i]);
                  }

            //    // plain lead ends
            //if (num == 3220 || num == 4293 || num == 973 || num == 1492  || num == 4912 || num==2683)
            //{
            //  Console.WriteLine(num);
            //}

                if (nextLeadHead != null && (IncludeLeadHead(nextLeadHead,methodIndex)))
                {
                    
                 // music[num, i] = (short)l.CalcWraps(); // Wraps hunt version
                  music[num, i] = l.Score(musicalPreferences, nextLeadHead); // normal scoring
                   //music[num, i] = (short)(10 - (short)i); // favour simplicity
                   // music[num, i] = (short)(l.Score(musicalPreferences, nextLeadHead) - (i > 0 ? 10 : 0)); - odd is hit - note need take out WriteMusicalChanges
                  
                  leadMapping[num, i] = nextLeadHead.ToNumberExTreble();
                  // ignore mapping back from nextLeadHead=rounds when this lead contains snap finish
                  //if (l.ContainsRoundsAt <= 0)
                  {
                    reverseLeadMapping[nextLeadHead.ToNumberExTreble(), i] = num;
                  }
                }
              }
            }
          }
        }
        Console.WriteLine(method.Name);
      }

      //Console.WriteLine("Indexed all leads " + (DateTime.UtcNow - startInit));
      WriteMusicalChanges();

      //Console.WriteLine("Written music " + (DateTime.UtcNow - startInit));

      // not used by simplecompser falseness check that works on leads
      //ComputeFalsenessTable(problem);
      //Console.WriteLine("Complete falseness table " + (DateTime.UtcNow - startInit));

      ComputeLeadsToEnd();


      //Console.WriteLine("Done leads to end count " + (DateTime.UtcNow - startInit));
    }

    private bool IncludeLeadHead(Row nextLeadHead, int methodIndex)
    {
        // standard include all version
        return !problem.TenorsTogether || 
            problem.Reverse?nextLeadHead.CoursingOrder().EndsWith("7"):
            nextLeadHead.CoursingOrder().StartsWith("7");
        //return true;

        // specials for date touch endings below

        bool ret = nextLeadHead.CoursingOrder().StartsWith("7");
        if ( !ret && methodIndex == 1 )
        {
            int num = nextLeadHead.ToNumber(); 
            // 1819 London finish, 127 is the lead AFTER then end that we work back from

            // 4066 Maypole - 1 lead to go
            //if (num == 4066 || num == 127)
            //{
            //    ret = true;
            //}
            //ret = true;
            if ( nextLeadHead.CoursingOrder() == "642357")
            {
                ret = true;
            }

            // include leads in the sBIM finish
            //if ( num == 3910 || num == 2051 ||
            //     num == 4761 ||
            //     num == 2356 || num == 127)
            //{
            //    ret = true;
            //}
        }
        if ( ret )
        {
            System.Diagnostics.Debug.WriteLine(nextLeadHead.ToNumber());
        }
        return ret;
        // standard include all version
        //return !bTenorsTogether || nextLeadHead.CoursingOrder().StartsWith("7");
    }

    public Problem problem;

    public void ComputeFalsenessTable()
    {
        var p = this.problem;

      falseLeadHeads = new short[nMethods, nMethods, NO_LEADENDS][];

      foreach (Method method1 in p.Methods)
      {
        int methIdx1 = _methodIndex[method1];

        // work out the falseness
        Lead fromRounds = method1.Lead(new Row(8));
        System.Diagnostics.Debug.Assert(fromRounds.LeadHead().Equals(new Row(8)));

        foreach (Method method2 in p.Methods)
        {
            System.Diagnostics.Debug.WriteLine(method1.Name + "/" + method2.Name);

          int methIdx2 = _methodIndex[method2];
          List<Lead> falseLeads = new List<Lead>();

          List<int> snaps = new List<int>();

          foreach (Lead l1 in method2.AllLeads)
          {
              if (l1.IsFalseAgainst(fromRounds))
              {
                // lead is false when starting from rounds
                //
                // want to work out for each remaining lead what it false against
                System.Diagnostics.Debug.WriteLine(l1.LeadHead() + " " + l1.LeadHead().CoursingOrder() );
                falseLeads.Add(l1);
              }
          }

          // not useful to know false against self -- except avoid plain forever!
          // falseLeads.Remove(fromRounds);

          System.Diagnostics.Debug.WriteLine("Total false leads:" + falseLeads.Count);


          // each false row is something that coming from rounds is false against the lead starting rounds - perm F(A)
          //
          // coming from another row A that is got from rounds by permuation A get to a different row F(A(rounds))

          List<Permutation> permutations = new List<Permutation>();
          foreach (Lead l in falseLeads)
          {
            permutations.Add(Permutation.GetPermutation(l.LeadHead().AsIntMapping()));
          }

          // for each lead in the source methods 
          foreach (Lead l in method1.AllLeads)
          {
            bool l1isSnap = l.ContainsRoundsAt > 0;
            falseLeadHeads[methIdx1,methIdx2,l.ToNumber()] = new short[permutations.Count];
            for (int i = 0; i < permutations.Count; ++i)
            {
              Row falseLeadHead = l.LeadHead().Apply(permutations[i]);
              Lead l2 = method2.Lead(falseLeadHead);
              if (l2.ContainsRoundsAt <= 0 || l.IsFalseAgainstIgnoringSnapCompletion(l2))
              {
                falseLeadHeads[methIdx1, methIdx2, l.ToNumber()][i] = falseLeadHead.ToNumberExTreble();
              }
              else
              {
                // ignore falsenessa after rounds
                falseLeadHeads[methIdx1, methIdx2, l.ToNumber()][i] = -1;
              }
            }
          }
        }
      }
    }


    private void ClearArrays()
    {
      
    leadMapping = new short[NO_LEADENDS, NO_CHOICES];
   reverseLeadMapping = new short[NO_LEADENDS, NO_CHOICES];
   music = new short[NO_LEADENDS, NO_CHOICES];

   leadsToEnd = new short[NO_LEADENDS];

    //public int[] falseCount = new int[NO_LEADENDS];

      for (int i = 0; i < NO_LEADENDS; ++i)
      {
        for (int j = 0; j < NO_CHOICES; ++j)
        {
          leadMapping[i, j] = -1;
          reverseLeadMapping[i, j] = -1;
        }
      }

      for (int i = 0; i < leadsToEnd.Length; ++i)
      {
        leadsToEnd[i] = short.MaxValue;
      }
    }

    private void WriteMusicalChanges()
    {
      foreach (short score in music)
      {
        if (score > maxMusic)
        {
          maxMusic = score;
        }
      }

      int[] boxes = new int[maxMusic + 1];
      int total = 0;
      foreach (short score in music)
      {
        ++boxes[score];
        total += score;
      }

      Console.WriteLine(_methodsByChoice[0].Name + " -> " + total);
      for (int i = 0; i < boxes.Length; ++i)
      {
        Console.WriteLine("Score = " + i + " Total Changes = " + boxes[i]);
      }
    }

    private void ComputeLeadsToEnd()
    {

      // Get to all in course leads bobs only in 14 leads - falseness neglected
      // Get to all counse leads in 10 with singles
      //
      // Feels like worth working out the falseness part
      int startOfRowAfterFinish = new Row(8).ToNumberExTreble();

      // for touch coming round at HAND before lead end in 2nds place method
      //int startOfRowAfterFinish = Row.AllRows.First(r => r.ToString() == "12436587").ToNumber();
      leadsToEnd[startOfRowAfterFinish] = 0;
      

      bool bFound = true;
      short maxDepth = (short)( problem.Reverse ? 1 : 0);
      int totalFound = 1; // found rounds already
      while (bFound)
      {
        bFound = false;
        int countAtDepth = 0;
        for (int lead = 0; lead < leadsToEnd.Length; ++lead)
        {
          if (leadsToEnd[lead] == maxDepth)
          {
            for (int callIdx = 0; callIdx < NO_CHOICES; ++callIdx)
            {
              int backLead = reverseLeadMapping[lead, callIdx];
              if (backLead >= 0 && leadsToEnd[backLead] > maxDepth + 1)
              {
                bFound = true;
                leadsToEnd[backLead] = (short)(maxDepth + 1);
                ++countAtDepth;
                ++totalFound;
              }
            }
          }
        }
        Console.WriteLine("Depth = " + maxDepth + " Count = " + countAtDepth + " " + "Total = " + totalFound );
        ++maxDepth;
      }
     Console.WriteLine("Total leads found = " + totalFound);
   }

    public IEnumerable<Row> Rows(short lead, int choice)
    {
      Method m = _methodsByChoice[choice];
      return m.Rows(lead);
    }

    public bool IsCall(int choice)
    {
      return isCall[choice];
    }

    internal int CallIndex(int choice)
    {
      return callIndex[choice];
    }
  }
}
