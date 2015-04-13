using System;
using System.Collections.Generic;
using System.Text;

namespace BellRinging
{
  public class RotationalBlockComposer
  {
    const int NO_CALLS = 2;
    const int NO_LEADENDS = 5040;
    const int MAX_LEADS = 160;
    int[,] leadMapping = new int[NO_LEADENDS, NO_CALLS];
    int[,] reverseLeadMapping = new int[NO_LEADENDS, NO_CALLS];
    int[,] music = new int[NO_LEADENDS, NO_CALLS];
    int[][] falseMapping;
    int[] falseCount = new int[NO_LEADENDS];
    int[] leadsToEnd = new int[NO_LEADENDS];
    bool[] courseEnds = new bool[NO_LEADENDS];
    bool bTenorsTogether = true;


    public void Initialise(string methodName, string notation)
    {
      ClearArrays();

      // work out all of the leads of the method
      Method method = new Method(methodName, "", notation, 8);

      IEnumerable<Lead> allLeads = method.AllLeads;

      List<Permutation> leadHeadChanges = method.LeadHeadChanges;

      MusicalPreferences musicalPreferences = new MusicalPreferences();
      musicalPreferences.InitELF();

      // index the leads
      foreach (Lead l in allLeads)
      {
        int num = l.ToNumber();

        for (int i = 0; i < leadHeadChanges.Count; ++i)
        {
          Row nextLeadHead = l.NextLeadHead(leadHeadChanges[i]);
          if (!bTenorsTogether || nextLeadHead.CoursingOrder().StartsWith("7"))
          {
            music[num, i] = l.Score(musicalPreferences, nextLeadHead);
            leadMapping[num, i] = nextLeadHead.ToNumber();
            reverseLeadMapping[nextLeadHead.ToNumber(), i] = num;
          }
        }
      }

      // note the course ends
      foreach (Lead l1 in allLeads)
      {
        courseEnds[l1.ToNumber()] = l1.LeadHead().IsFromCourseEnd();
      }

      WriteMusicalChanges();

      // work out the falseness
      Lead fromRounds = method.Lead(new Row(8));
      System.Diagnostics.Debug.Assert(fromRounds.LeadHead().Equals(new Row(8)));

      List<Lead> falseLeads = new List<Lead>();
      foreach (Lead l1 in allLeads)
      {
        if (l1.IsFalseAgainst(fromRounds))
        {
          // lead is false when starting from rounds
          //
          // want to work out for each remaining lead what it false against
          falseLeads.Add(l1);
        }
      }

      // not useful to know false against self -- except avoid plain forever!
      // falseLeads.Remove(fromRounds);

      Console.WriteLine("Total false leads:" + falseLeads.Count);

      // each false row is something that coming from rounds is false against the lead starting rounds - perm F(A)
      //
      // coming from another row A that is got from rounds by permuation A get to a different row F(A(rounds))

      List<Permutation> permutations = new List<Permutation>();
      foreach (Lead l in falseLeads)
      {
        permutations.Add(Permutation.GetPermutation(l.LeadHead().AsIntMapping()));
      }

      falseMapping = new int[NO_LEADENDS][];
      foreach (Lead l in allLeads)
      {
        falseMapping[l.ToNumber()] = new int[permutations.Count];
        for (int i = 0; i < permutations.Count; ++i)
        {
          Row falseLeadHead = l.LeadHead().Apply(permutations[i]);
          falseMapping[l.ToNumber()][i] = falseLeadHead.ToNumber();
        }
      }

      WorkOutLeadsToEnd();
    }

    private void ClearArrays()
    {
      for (int i = 0; i < NO_LEADENDS; ++i)
      {
        for (int j = 0; j < NO_CALLS; ++j)
        {
          leadMapping[i, j] = -1;
          reverseLeadMapping[i, j] = -1;
        }
      }
    }
    int maxMusic = 0;

    int[] compositionsWithMusicScore;
    int[] trimByMusic = new int[MAX_LEADS];
    private void WriteMusicalChanges()
    {
      foreach (int score in music)
      {
        if (score > maxMusic)
        {
          maxMusic = score;
        }
      }

      int[] boxes = new int[maxMusic + 1];
      foreach (int score in music)
      {
        ++boxes[score];
      }

      for (int i = 0; i < boxes.Length; ++i)
      {
        Console.WriteLine("Score = " + i + " Total Changes = " + boxes[i]);
      }

      compositionsWithMusicScore = new int[1 + maxMusic * MAX_LEADS];
    }

    private void WorkOutLeadsToEnd()
    {
      // Get to all in course leads bobs only in 14 leads - falseness neglected
      // Get to all counse leads in 10 with singles
      //
      // Feels like worth working out the falseness part
      int rounds = new Row(8).ToNumber();
      for (int i = 0; i < leadsToEnd.Length; ++i)
      {
        leadsToEnd[i] = int.MaxValue;
      }
      leadsToEnd[rounds] = 0;

      bool bFound = true;
      int maxDepth = 0;
      int totalFound = 1; // found rounds already
      while (bFound)
      {
        bFound = false;
        int countAtDepth = 0;
        for (int lead = 0; lead < leadsToEnd.Length; ++lead)
        {
          if (leadsToEnd[lead] == maxDepth)
          {
            for (int callIdx = 0; callIdx < NO_CALLS; ++callIdx)
            {
              int backLead = reverseLeadMapping[lead, callIdx];
              if (backLead >= 0 && leadsToEnd[backLead] > maxDepth + 1)
              {
                bFound = true;
                leadsToEnd[backLead] = (int)(maxDepth + 1);
                ++countAtDepth;
                ++totalFound;
              }
            }
          }

        }
        Console.WriteLine("Depth = " + maxDepth + " Count = " + countAtDepth + " " + "Total = " + totalFound);
        ++maxDepth;
      }
    }

    int[] calls = new int[MAX_LEADS];
    int[] leads = new int[MAX_LEADS];
    int totalMusic = 0;
    int regenPointer = int.MinValue;

    long totalLeads;
    int maxLength;
    int totalCompositions;
    int[] bestMusic = new int[MAX_LEADS];


    bool bComposing;

    public void Compose()
    {
      bComposing = true;
      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DoComposeWrapper));
      do
      {
        lock (this)
        {
          Console.WriteLine("Leads " + totalLeads + " Max length " + maxLength + " Compositions " + totalCompositions + " best music " + bestMusic);
          //for (int i = 0; i <= bestMusic; ++i)
          //{
          //  Console.WriteLine(i + " " + compositionsWithMusicScore[i]);
          //} 
          for (int i = 157; i < MAX_LEADS; ++i)
          {
            Console.WriteLine("Leads = " + (i + 1) + " Best music = " + bestMusic[i]);
          }
          //for (int j = 0; j < MAX_LEADS; ++j)
          //{
          //  Console.WriteLine("Trim? " + j + " " + trimByMusic[j]);
          //}
        }
        System.Threading.Thread.Sleep(10000);
      }
      while (bComposing);
      Console.WriteLine("Leads " + totalLeads + " Max length " + maxLength + " Compositions " + totalCompositions + " best music " + bestMusic);
      //for (int i = 0; i <= bestMusic; ++i)
      //{
      //  Console.WriteLine(i + " " + compositionsWithMusicScore[i]);
      //} 
      for (int i = 0; i < MAX_LEADS; ++i)
      {
        Console.WriteLine("Leads = " + (i+1) + " Best music = " + bestMusic[i]);
      }
    }

    void DoComposeWrapper(object o)
    {
      try
      {
        DateTime start = DateTime.UtcNow;
        DoCompose();
        Console.WriteLine("Composition complete in " + (DateTime.UtcNow - start));
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      finally
      {
        bComposing = false;
      }
    }

     void DoCompose()
    {
      unchecked // makes arguable difference - not obvious!
      {
        long lastRegenTotalLeads = 0;
        var start = new Row(8).ToNumber();

        var currentLead = start;
        int noLeads = 0;

        //fixed(int * c = &calls[0]){};

        // lead is the lead just rung, call is the call made from that lead
        calls[0] = 0;
        leads[0] = start;

        /* see if star Bristol composition is true and how the leads to 
         * end property works out
         * 
         * find noLeads + leadsToEnd[nextLead] goes up to 39
         * at end noLeads = calls.Length-1
         * nextLead = rounds
         * leadsToEnd = 0
        int hwm = 0;
        for ( noLeads = 0; noLeads < calls.Length; ++noLeads)
        {
          char c = "1112212121122111221221112211121211122122"[noLeads];
          calls[noLeads] = c - '1';
          int nextLead = leadMapping[leads[noLeads], calls[noLeads]];
          leads[(noLeads + 1) % leads.Length] = nextLead;
          bool isTrue = IsTrue(nextLead, leads, noLeads);
          int toEnd = leadsToEnd[nextLead];
          hwm = Math.Max(hwm, noLeads + toEnd);
          //if (!isTrue)
          //{
          //  throw new Exception("not true");
          //}
        }
         * */

        //string a = "1112212121122111221221112211121211122122";
        //string sm = a;
        //for (int i = 0; i < a.Length; ++i)
        //{
        //  a = a.Substring(1) + a[0];
        //  if (sm.CompareTo(a) > 0)
        //  {
        //    sm = a;
        //  }
        //}
        //Console.WriteLine("grail = " + sm);
        //if (a != "1112212121122111221221112211121211122122")
        //{
        //  throw new Exception("Not back to start");
        //}
        //int maxM = 0;

        //string grail = "1112121112212211122121211221112212211122";

        while (true)
        {
          ++totalLeads;

          //if (totalLeads >= 14777055)
          //{
          //  string s = "";
          //  for (int i = 0; i <= noLeads; ++i)
          //  {
          //    s += "123-"[calls[i]];
          //  }

          //  Console.WriteLine(s);
          //  Console.WriteLine(grail);
          //  s += "";
          //}

          int nextLead = leadMapping[currentLead, calls[noLeads]];

          //{
          //  int m = 0;
          //  for (int i = 0; i <= noLeads; ++i)
          //  {
          //    if ("123-"[calls[i]] == grail[i])
          //    {
          //      m = i;
          //    }
          //    else
          //    {
          //      break;
          //    }
          //  }
          //  if (m > maxM)
          //  {
          //    maxM = m;
          //    Console.WriteLine("Match max any change = " + maxM);
          //  }
          //}

          //if (maxM == 31)
          //{
          //  Console.WriteLine("Hi");
          //}

          if (nextLead == start)
          {
            // if have rolled forwards by a fraction of a copy
            // then skip this composition as we will find its rotation
            //
            // regenPointer 
            if (true)
            {
              // suppress int compositions
              if (noLeads + 1 >= 5000 / 32)
              {

                ++totalCompositions;
                //int m = 0;
                //for (int i = 0; i <= noLeads; ++i)
                //{
                //  if ("123-"[calls[i]] == grail[i])
                //  {
                //    m = i;
                //  }
                //  else
                //  {
                //    break;
                //  }
                //}
                //bool bPrint = calls[0] == 0 && calls[1] == 0 && calls[2] == 0 && calls[3] == 1;
                //bPrint = m > maxM;
                //if (m > maxM)
                //{
                //  maxM = m;
                //  Console.WriteLine("Match max = " + maxM);
                //}

                bool bPrint = true;

                // for each rotation of the composition
                for (int rot = 0; rot <= noLeads; ++rot)
                {
                  int lead = rot;
                  int leadHead = start;
                  totalMusic = 0;
                  for (int i = 0; i <= noLeads && leadHead >= 0; ++i)
                  {
                    //if (rot == 0 && bPrint) Console.Write("123-"[calls[lead]]);
                    totalMusic += music[leadHead, calls[lead]];
                    leadHead = leadMapping[leadHead, calls[lead]];
                    ++lead;
                    if (lead > noLeads)
                    {
                      lead = 0;
                    }
                  }
                  // if leadHead < 0 then went through some change that violated our rules on included calls
                  if (leadHead >= 0)
                  {
                    //if (rot == 0 && bPrint) Console.WriteLine();
                    //totalMusic += music[currentLead, calls[noLeads]];
                    // got a composition
                    WriteComposition(noLeads, rot);
                  }
                }
              }
            }

            //totalMusic -= music[currentLead, calls[noLeads]];
            //return;
          }

          // falseness checking - could have falsecount updated each time go forward or back
          //
          //                    - could use table to check if new lead is false against composition so far
          //
          // as add methods to the mix then want to check against what in place so far (as number of checks remains constant)
          //
          // leadsToEnd[rounds] = 0
          // leadsToEnd[one lead back] = 1
          // noLeads = calls.Length - 1 is last place we get to (increment if less than this)
          // noLeads = calls.Length - 1 was only worth getting here if lead is rounds
          // noLeads + leadsToEnd  = calls.Length - 1 is always true on the last lead worth getting to
          // so increment if noLeads + leadsToEnd < calls.Length - 1
          //
          // if nextLead = rounds -1 
          // leadsToEnd=1
          // noLeads <= calls.Length - 2 want to go forwards

          // noLeads < calls.Length - 1 want to go forwards

          //
          // if nextLead is rounds-1
          // OK so long as noLeads < calls.Length-1

          if (nextLead >= 0
             && IsTrue(nextLead, leads, noLeads)
            //&& noLeads < calls.Length - 1
             && noLeads + leadsToEnd[nextLead] < calls.Length
            && nextLead != start
            //&& totalMusic >= bestMusic[MAX_LEADS-1] - (calls.Length - 1 - noLeads) * maxMusic
            )
          {
            //if (totalMusic < bestMusic - (calls.Length - 1 - noLeads)*maxMusic)
            //{
            //  trimByMusic[noLeads]++;
            //}
            //totalMusic += music[currentLead, calls[noLeads]];
            currentLead = nextLead;
            ++noLeads;
            if (noLeads > maxLength) maxLength = noLeads;

            // if we are doing a tenors together composition then pad with "0" until get to a course
            // end so that copy in whole courses
            if (regenPointer < 0)
            {
              // if we have brought up a course end then can move to normal mode
              if (!bTenorsTogether || courseEnds[currentLead])
              {
                regenPointer = 0;
                calls[noLeads] = calls[regenPointer];
              }
              else
              {
                calls[noLeads] = 0;
              }
            }
            else
            {
              calls[noLeads] = calls[regenPointer];
            }
            ++regenPointer;
            leads[noLeads] = currentLead;
          }
          else
          {
            // to come in here is a rejection of the current lead and therefore a backtrack even if 
            // do not move the pointer back
            regenPointer = int.MinValue;
            ++calls[noLeads];
            if (calls[noLeads] == NO_CALLS)
            {
              // backtrack 

              lastRegenTotalLeads = totalLeads;

              while (calls[noLeads] == NO_CALLS)
              {
                if (noLeads > 0)
                {
                  --noLeads;
                  currentLead = leads[noLeads];
                  ++calls[noLeads];
                }
                else
                {
                  return;
                }
              }
              // if tenors together then 
            }
          }
        }
      }
    }

    private bool IsTrue(int nextLead, int[] leads, int noLeads)
    {
      for (int i = 0; i <= noLeads; ++i)
      {
        if (nextLead == leads[i]) return false;

        for (int j = 0; j < falseMapping[nextLead].Length; ++j)
        {
          if (leads[i] == falseMapping[nextLead][j])
          {
            return false;
          }
        }
      }
      return true;
    }

    private void WriteComposition(int noLeads, int rot)
    {
      lock (this)
      {

        compositionsWithMusicScore[totalMusic]++;

        //++bestMusic[noLeads]; // count compositions for now
     
        if (totalMusic > bestMusic[noLeads])
        {
          Console.WriteLine("Rot = " + rot);
          bestMusic[noLeads] = totalMusic;
          Console.WriteLine("Leads = " + (noLeads + 1) + " ( " + (noLeads + 1) * 32 + "  changes) Total music " + totalMusic);
          for (int i = 0; i <= noLeads; ++i)
          {
            Console.WriteLine(Row.FromNumber(leads[i]) + " " + "-BSZ"[calls[i]] + " " + music[leads[i], calls[i]]);
          }
        }
      }
    }
  }
}
