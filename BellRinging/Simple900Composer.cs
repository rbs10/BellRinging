using System;
using System.Collections.Generic;
using System.Text;

namespace BellRinging
{
  public class Simple900Composer
  {
    Tables _tables = new Tables();
    ICompositionReceiver _receiver;

    public ICompositionReceiver Receiver { get { return _receiver; } set { _receiver = value; } }


    public void InitialiseStandardMethods()
    {
      MethodLibrary lib = new MethodLibrary();
      Problem p = new Problem();
      foreach (string method in
        new string[] { 
          //"Cambridge", 
          "Yorkshire",
          //"Pudsey", "Lincolnshire", "Superlative",
          "Rutland",
          //"London", 
          "Bristol",
          //"Belfast", 
          "Glasgow" })
      {
        char letter = method[0];
        if (method == "Lincolnshire") letter = 'N';
        if (method == "Belfast") letter = 'F';
        p.AddMethod(new Method(method,letter.ToString(),lib.GetNotation(method),8));
      }
      _tables.Initialise(p);
    }

    public void Initialise(string method, string notation)
    {
      Problem p = new Problem();
      p.AddMethod(new Method(method, "",notation, 8));
      _tables.Initialise(p);
    }

    public void Initialise(string method)
    {
      Problem p = new Problem();
      MethodLibrary lib = new MethodLibrary();
      p.AddMethod(new Method(method, "", lib.GetNotation(method), 8));
      _tables.Initialise(p);
    }
    public void Initialise(string method1, string code1, string notation1, string method2, string code2,
       string notation2)
    {
      Problem p = new Problem();
      p.AddMethod(new Method(method1,code1,notation1, 8));
      p.AddMethod(new Method(method2,code2,notation2, 8));
      _tables.Initialise(p);
    }

    //int[] compositionsWithMusicScore;
    
 
    // best music found at a given number of leads
    int[] bestMusic;

    int totalMusic = 0;

    long totalLeads;
    int maxLength;
    int totalCompositions;

    int maxLeads;

    bool bComposing;

    public void StartCompose()
    {
      maxLeads = _tables.MAX_LEADS;

      _composition = new Composition(_tables);

      bestMusic = new int[maxLeads*32+1];
      //compositionsWithMusicScore = new int[1 + _tables.maxMusic * _tables.MAX_LEADS];

      //DoComposeWrapper(null);

      bComposing = true;
      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DoComposeWrapper));
    }

    public long TotalLeads
    {
      get
      {
        if (_composition == null)
        {
          return 0;
        }
        else
        {
          Composition c = _composition.Clone() as Composition;

          long count = 0;
          long cMax = 0;
          for (int i = 0; i < _tables.MAX_LEADS - 1; ++i)
          {
            count = count * _tables.NO_CHOICES + c.choices[i];
            cMax = cMax * _tables.NO_CHOICES + _tables.NO_CHOICES - 1;
          }

          return count;  //100.0 * count / cMax;// totalLeads;
        }
      }
    }

    public long LeadsPerSecond
    {
      get
      {
        if (_composition == null)
        {
          return 0;
        }
        else
        {
          TimeSpan time = (bComposing ? DateTime.UtcNow : _endTime) - _startTime;
          return (long)(totalLeads / time.TotalSeconds);
        }
      }
    }


    public string MinBackTrackDescription
    {
      get
      {
        return minBackTrackPoint.ToString() + "/" + timesToPoint.ToString();
      }
    }

    public void Compose()
    {
      StartCompose();
      do
      {
        lock (this)
        {
          WriteStats();
        }
        System.Threading.Thread.Sleep(10000);
      }
      while (bComposing);

      WriteStats();
 
    }

    private void WriteStats()
    {
      return;

      Console.WriteLine("Leads " + totalLeads + " Max length " + maxLength + " Compositions " + totalCompositions + " best music " + bestMusic);
      //for (int i = 0; i <= bestMusic; ++i)
      //{
      //  Console.WriteLine(i + " " + compositionsWithMusicScore[i]);
      //} 
      for (int i = 0; i < maxLeads; ++i)
      {
        Console.WriteLine("Leads = " + i + " Best music = " + bestMusic[i]);
      }
      //for (int j = 0; j < MAX_LEADS; ++j)
      //{
      //  Console.WriteLine("Trim? " + j + " " + trimByMusic[j]);
      //}
    }

    DateTime _startTime;
    DateTime _endTime;

    void DoComposeWrapper(object o)
    {
      try
      {
        _startTime = DateTime.UtcNow;
        DoCompose();
        //Console.WriteLine("Composition complete in " + (DateTime.UtcNow - start));
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      finally
      {
        _endTime = DateTime.UtcNow;
        bComposing = false;
      }
    }

    Composition _composition;

    int firstUnprovenLead = 0;

    int minBackTrackPoint;
    int timesToPoint;

    //int maxLeadIndex = 0;

    void DoCompose()
    {
      var start = new Row(8).ToNumber();

      var currentLead = start;

      int[] choices = _composition.choices;
      int[] leads = _composition.leads;

      // lead is the lead just rung, call is the call made from that lead
      choices[0] = 3;
      leads[0] = start;
      _composition.maxLeadIndex = 0;
      minBackTrackPoint = int.MaxValue;
      timesToPoint = 0;

      int maxLeadIndex = 0;


      //int[] lastCheckedComp = null;
      //int[] lastCheckedLeads = null;
      //int lastFalseAt;

      while (true)
      {
        ++totalLeads;

        var nextLead = _tables.leadMapping[currentLead, choices[maxLeadIndex]];

        //if (nextLead >= 0 && _tables._methodsByChoice[0].Lead(nextLead).ContainsRoundsAt > 0)
        //{
        //  Console.WriteLine("Found snap");
        //}
        if (nextLead == start
           // && maxLeadIndex == 29
          )
        {
          // for pure Bristol slower with deferred truth and no rewind to point of falseness
          // 2.6s with truth test here c.f. 1s approx with incremental truth test
          //
          // slippe out to 4 seconds with some bit of indirection ... - but small changes to what
          // is indirected do not change much!
          _composition.maxLeadIndex = maxLeadIndex;

          //int firstDiffAt = -1;
          //if (lastCheckedComp != null)
          //{
          //  for (int i = 0; i < lastCheckedComp.Length; ++i)
          //  {
          //    if (lastCheckedComp[i] != choices[i])
          //    {
          //      firstDiffAt = i;
          //      break;
          //    }
          //    if (lastCheckedLeads[i] != leads[i])
          //    {
          //      firstDiffAt = i;
          //      break;
          //    }
          //  }
          //}

          int falseAt = _composition.RunsFalseAt(ref firstUnprovenLead);
          int minBackTrack;
          if (falseAt < 0)
          {
            totalMusic += _tables.music[currentLead, choices[maxLeadIndex]];
            WriteComposition(maxLeadIndex);
            totalMusic -= _tables.music[currentLead, choices[maxLeadIndex]];
            // true - no need to backtrack other than because of outcome of other choices here
            minBackTrack = maxLeadIndex;
          }
          else
          {
            // backtrack and make another choice in the lead where ran false
            minBackTrack = falseAt;
          }

          //lastCheckedComp = choices.Clone() as int[];
          //lastCheckedLeads = leads.Clone() as int[];
          //lastFalseAt = falseAt;

          // falseAt is the first lead that was false - therefore we made either a false choice of method
          // at that lead or a false call at the previous lead
          bool bDone = BackTrack(ref currentLead, choices, leads, ref maxLeadIndex, (int)minBackTrack);
          if (bDone)
          {
            return;
          }
        }
        else
        {

          // continue down tree if
          if (nextLead >= 0  // choice is allowed
            && maxLeadIndex < maxLeads - 1 // array length (else continue from rounds)
            //&&  IsTrue(nextLead, leads, maxLeadIndex)  // next lead is true against what got so far
            && maxLeadIndex + _tables.leadsToEnd[nextLead] < maxLeads // could come round in time
            && IsNotTriviallyFalse(nextLead,leads,maxLeadIndex)
            //&& totalMusic >= bestMusic[maxLeads - 1] - (maxLeads - 1 - maxLeadIndex) * _tables.maxMusic // could be more miscal
            //&& totalMusic >= bestMusic[maxLeads - 1] - (maxLeads - 1 - maxLeadIndex) * _tables.maxMusic -1 // could be more miscal
            /*
             * 
             *  YORKSHIRE
             * 
             * got us to 5
            && ((totalMusic >= 3) || (maxLeadIndex < 20))
            && ((totalMusic >= 4) || (maxLeadIndex < 30))
            && ((totalMusic >= 2) || (maxLeadIndex < 10))
             * 
             * 
            
             * also 5 quite quick
            && ((totalMusic >= 5) || (maxLeadIndex < 36))
            && ((totalMusic >= 4) || (maxLeadIndex < 24))
            && ((totalMusic >= 3) || (maxLeadIndex < 16))
            && ((totalMusic >= 2) || (maxLeadIndex < 8))
             * 
             *  5 all searched           
            && ((totalMusic >= 5) || (maxLeadIndex < 28))
            && ((totalMusic >= 4) || (maxLeadIndex < 21))
            && ((totalMusic >= 3) || (maxLeadIndex < 14))

             * 7 in 60 leads
            && ((totalMusic >= 6) || (maxLeadIndex < 35))
            && ((totalMusic >= 5) || (maxLeadIndex < 28))
            && ((totalMusic >= 4) || (maxLeadIndex < 21))
            && ((totalMusic >= 3) || (maxLeadIndex < 14))
            
             * * 6 in 1346 all done - nothing else
            && ((totalMusic >= 7) || (maxLeadIndex < 42))
            && ((totalMusic >= 6) || (maxLeadIndex < 35))
            && ((totalMusic >= 5) || (maxLeadIndex < 28))
            && ((totalMusic >= 4) || (maxLeadIndex < 21))
            && ((totalMusic >= 3) || (maxLeadIndex < 14)) 
             * 
             * 8 at 1920 running to 60 leads
            && ((totalMusic >= 7) || (maxLeadIndex < 48))
            && ((totalMusic >= 6) || (maxLeadIndex < 35))
            && ((totalMusic >= 5) || (maxLeadIndex < 28))
            && ((totalMusic >= 4) || (maxLeadIndex < 21))
            && ((totalMusic >= 3) || (maxLeadIndex < 14))* */

            //&& ((totalMusic >= 2) || (maxLeadIndex < 14))
            //&& ((totalMusic >= 3) || (maxLeadIndex < 21))
            //&& ((totalMusic >= 4) || (maxLeadIndex < 28))
            //&& ((totalMusic >= 5) || (maxLeadIndex < 35))
            //&& ((totalMusic >= 6) || (maxLeadIndex < 48))
            //&& ((totalMusic >= 7) || (maxLeadIndex < 54))
            //&& ((totalMusic >= 2) || (maxLeadIndex < 7))
            )
          {
            totalMusic += _tables.music[currentLead, choices[maxLeadIndex]];
            currentLead = nextLead;
            ++maxLeadIndex;
            if (maxLeadIndex > maxLength) maxLength = maxLeadIndex;
            choices[maxLeadIndex] = 0;
            leads[maxLeadIndex] = currentLead;

            if (maxLeadIndex < firstUnprovenLead) throw new Exception("Change before lwm");
          }
          else
          {
            bool bDone = BackTrack(ref currentLead, choices, leads, ref maxLeadIndex, int.MaxValue);
            if (bDone)
            {
              return;
            }
          }
        }
      }
    }


    private bool BackTrack(ref int currentLead, int[] choices, int[] leads, ref int maxLeadIndex, int lastPossiblyTrueLead)
    {
      ++choices[maxLeadIndex];

      while (choices[maxLeadIndex] == _tables.NO_CHOICES || maxLeadIndex > lastPossiblyTrueLead)
      {
        if (maxLeadIndex > 0)
        {
          --maxLeadIndex;
          currentLead = leads[maxLeadIndex];
          totalMusic -= _tables.music[currentLead, choices[maxLeadIndex]];
          ++choices[maxLeadIndex];
        }
        else
        {
          return true;
        }
      }
      // changed stuff at maxLeadIndex; maxLeadIndex-1 not changed so does not need checking again
      firstUnprovenLead = Math.Min(firstUnprovenLead, maxLeadIndex);

      // keep stats on backtracking as a progress measure
      if (maxLeadIndex < minBackTrackPoint)
      {
        timesToPoint = 0;
        minBackTrackPoint = maxLeadIndex;
      }
      else if (minBackTrackPoint == maxLeadIndex)
      {
        ++timesToPoint;
      }

      return false;
    }

    private bool IsTrue(int nextLead, int[] leads, int noLeads)
    {
      int firstUnprovenLead = noLeads;
      _composition.maxLeadIndex = noLeads;
      int falseAt = _composition.RunsFalseAt(ref firstUnprovenLead);
      return falseAt < 0;
    }


    private bool IsNotTriviallyFalse(int nextLead, int[] leads, int maxLeadIndex)
    {
      for (int i = 0; i <= maxLeadIndex; ++i)
      {
        if (leads[i] == nextLead) return false;
      }
      return true;
    }

    public Tables Tables
    {
      get
      {
        return _tables;
      }
  }
   
    private void WriteComposition(int noLeads)
    {
      lock (this)
      {
     
        ++totalCompositions;

        int totalScore = _composition.Score;// -_composition.Calls;
        //_composition.CalcWraps();

       // compositionsWithMusicScore[totalScore]++;
        int changes = _composition.Changes;
        if (
          changes == 900 &&
         // _composition.Calls < 9
         // totalScore >= bestMusic[changes]  &&
        _composition.choices[0] > 2 &&
          //totalScore >= 120 &&
          // _composition.Calls < 10&&
          // totalScore > 2&&
          true
          )
        {
          if (_receiver != null)
          {
            _composition.TimeToFind = DateTime.UtcNow - _startTime;
            _receiver.AddComposition(_composition.Clone());
          }

          bestMusic[changes] = totalScore;
         return;
        Console.WriteLine("Leads = " + (noLeads+1) + " ( " + (noLeads +1)*32 +"  changes) Total music " + totalMusic ); 
        for (int i = 0; i <= noLeads; ++i)
        {
          Console.WriteLine(Row.FromNumber(_composition.leads[i]) + " " + "-BSZ"[_composition.choices[i]] + " " + _tables.music[_composition.leads[i], _composition.choices[i]]);
        }
        }
      }
    }
  }
}
