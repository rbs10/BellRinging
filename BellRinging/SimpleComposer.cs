using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BellRinging
{
  public class SimpleComposer
  {
    Tables _tables = new Tables();
    ICompositionReceiver _receiver;

    public ICompositionReceiver Receiver { get { return _receiver; } set { _receiver = value; } }


    public void InitialiseStandardMethods_Standard()
    {
      MethodLibrary lib = new MethodLibrary();
      Problem p = new Problem();
      foreach (string method in
        new string[] { 
             "Yorkshire"
          
         // ,"Superlative"
         // ,"Yorkshire"
         // ,"London"
         // ,"Cambridge"
         // ,"Bristol"
         //, "Pudsey", "Lincolnshire"
         // ,"Rutland"
          //"Belfast", //"Glasgow",
          //"Londonderry",
           })
      {
        char letter = method[0];
        if (method == "Lincolnshire") letter = 'N';
        if (method == "Belfast") letter = 'F';
        p.AddMethod(new Method(method,letter.ToString(),lib.GetNotation(method),8));
      }
      _tables.Initialise(p);
    }

    public void InitialiseStandardMethods()
    {
        MethodLibrary lib = new MethodLibrary();
        Problem p = new Problem();
        int index = 0;
        foreach (string method in
          new string[] { 
             "Yorkshire"
          
         // ,"Superlative"
         // ,"Yorkshire"
         // ,"London"
         // ,"Cambridge"
         // ,"Bristol"
         //, "Pudsey", "Lincolnshire"
         // ,"Rutland"
          //"Belfast", //"Glasgow",
          //"Londonderry",
           })
        {
            ++index;
            char letter = method[0];
            if (method == "Lincolnshire") letter = 'N';
            if (method == "Belfast") letter = 'F';
            var methodObject = new Method(method, letter.ToString(), lib.GetNotation(method), 8);
            if (index > 1)
            {
                //methodObject.FirstLeadOnly();
               // methodObject.LastLeadOnly();
            }
            p.AddMethod(methodObject);
        }
        _tables.Initialise(p);
    }

    public void Initialise(string method, string notation)
    {
      Problem p = new Problem();
      p.AddMethod(new Method(method, method.Substring(0,1),notation, 8));
      _tables.Initialise(p);
    }

    public void Initialise(string method)
    {
      MethodLibrary lib = new MethodLibrary();
      var notation = lib.GetNotation(method);
      Initialise(method, notation);
    }

    public void InitialiseWithSnapStart(string method)
    {
        Problem p = new Problem();
        MethodLibrary lib = new MethodLibrary();
        var notation = lib.GetNotation(method);
        p.AddMethod(new Method(method, "", notation, 8));
        var methodWithSnapStart = new Method(method + "(snap)", "!", notation, 8);
        methodWithSnapStart.SnapStart();
        p.AddMethod(methodWithSnapStart);
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
    int[] bestQuality;
    int[] bestCalls;
    int bestTotalMusic;


    long totalLeads;
    int maxLength;
    int totalCompositions;

    int maxLeads;

    bool bComposing;

    public void StartCompose(Action initFunction)
    {

      maxLeads = _tables.MAX_LEADS;

      _composition = new Composition(_tables);

      bestMusic = new int[maxLeads*32+1];
      bestQuality = new int[maxLeads * 32 + 1];
      bestCalls = new int[maxLeads * 32 + 1];

      int maxGroup = 63 * 1000;
      bestMusic = new int[maxGroup];
      bestQuality = new int[maxGroup];
      bestCalls = new int[maxGroup];
      //compositionsWithMusicScore = new int[1 + _tables.maxMusic * _tables.MAX_LEADS];

      //DoComposeWrapper(null);

      bComposing = true;
      System.Threading.ThreadPool.QueueUserWorkItem((o) =>DoComposeWrapper(initFunction));
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
           var temp = _composition.Clone();
           temp.rot = 0;
           temp.maxLeadIndex = (int)(temp.leads.Length - 1);
           return temp.ToString();
        //return minBackTrackPoint.ToString() + "/" + timesToPoint.ToString();
      }
    }

    public void Compose()
    {
        StartCompose(() => { });
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

    void DoComposeWrapper(Action initFunction)
    {
      try
      {
          initFunction();
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
      int regenPointer = -1;

      int[] choices = _composition.choices;
      int[] leads = _composition.leads;

      // lead is the lead just rung, call is the call made from that lead
      choices[0] = 0;

// London?
      //choices[0] = 3;

      // for snap start have set up "3" from rounds as the snap start lead
      // so start the composition there (and nothing beyond so no problem there)
      //choices[0] = 3; // snap start

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

        // the start of the next lead as determined by what we do in at this choice
        int nextLead = _tables.leadMapping[currentLead, choices[maxLeadIndex]];
 
        
        // if the next lead starts with rounds then it does not matter what the method is - we are done
        // if the next lead has a snap in it then need to ring it (and not next lead)
        bool end;

        // if allow snap finishes
          /*
        int snapFinish = -1;
        if (nextLead >= 0 && (snapFinish = _tables._methodsByChoice[choices[maxLeadIndex]].Lead(currentLead).ContainsRoundsAt) > 0)
        {
            Console.WriteLine("Found snap");
        }
        end = snapFinish > 0 || nextLead == start;*/
        //end = snapFinish > 0 ;//|| nextLead == start;

        // no snap version
        end = nextLead == start;

        //if (nextLead >= 0 && _tables._methodsByChoice[0].Lead(nextLead).ContainsRoundsAt > 0)
        //{
        //  Console.WriteLine("Found snap");
        //}
        if (end
            // Force a minimum touch length
           //&& maxLeadIndex >= 39
            && IsNotTriviallyFalseOrRepetitive(-1 /* next lead is rounds and is a repeat - ignore that */
            , leads, choices, maxLeadIndex)
          )
        {
          // for pure Bristol slower with deferred truth and no rewind to point of falseness
          // 2.6s with truth test here c.f. 1s approx with incremental truth test
          //
          // slippe out to 4 seconds with some bit of indirection ... - but small changes to what
          // is indirected do not change much!
          _composition.maxLeadIndex = maxLeadIndex;
          _composition.rot = 0;

            //if ( _composition.ToString().StartsWith("000012112000"))
            ////                                       000012112000202000121120002010001211200012
            //{

            //    Console.WriteLine("Hi");
            //}

          int firstUnprovenLead = -1; // no false check !!! TODO:
          var falseAt = _composition.RunsFalseAt5(ref firstUnprovenLead);
          int minBackTrack;
         // if (true /*falseAt < 0*/)
          
            //if ( maxLeadIndex == 41 )
            //{
            //    string magic = "000012112000202000121120002010001211200012";
            //      //"112000202000121120002010001211200012000012";
            //    int p = 0;
            //    foreach ( char c in magic )
            //    {
            //        choices[p] = c - '0';
            //        if (p + 1 < leads.Length)
            //        {
            //            leads[p + 1] = _tables.leadMapping[leads[p], choices[p]];
            //        }
            //        ++p;
            //    }
            //    var str = _composition.ToString();

            //    int ful2 = -1;
            //    var false2 = _composition.RunsFalseAt(ref ful2);
            //    Console.WriteLine("Hi");
            //}

          if (falseAt < 0)
          {
              for (_composition.rot = 0; _composition.rot <= maxLeadIndex; ++_composition.rot)
              {
                  //var music = _composition.Music;
                  
                  WriteComposition();
              }
             
            // true - no need to backtrack other than because of outcome of other choices here
            minBackTrack = maxLeadIndex;
          }
          else
          {
            // backtrack and make another choice in the lead where ran false
            minBackTrack = falseAt;
          }


          // falseAt is the first lead that was false - therefore we made either a false choice of method
          // at that lead or a false call at the previous lead
          bool bDone = BackTrack(ref currentLead, choices, leads, ref maxLeadIndex, (int)minBackTrack, ref regenPointer);
          if (bDone)
          {
            return;
          }
        }
        else
        {

          // continue down tree if
          if (nextLead >= 0  // choice is allowed

              // avoid lots of singles
          //&& ( maxLeadIndex > maxLeads - 5 || choices[maxLeadIndex] != 2 )

            && maxLeadIndex < maxLeads - 1 // array length (else continue from rounds)

            // this is expensive - better to work out once got something that comes round
            //&&  IsTrue(nextLead, leads, maxLeadIndex)  // next lead is true against what got so far

            && maxLeadIndex + _tables.leadsToEnd[nextLead] < maxLeads // could come round in time

            // constraint to go through Cambridge group sBIM finish
            //&& (nextLead == 3910 || maxLeadIndex != maxLeads - 5 ) // 63 - 5 = 58

            // try and get a largely tenors together compostion
            //&& ( maxLeadIndex > 58 || Row.FromNumber(nextLead).IsTenorsTogetherLeadEnd)

            //&& totalChoices < 20

            && IsNotTriviallyFalseOrRepetitive(nextLead,leads,choices,maxLeadIndex)

          

            )
          {
            //totalChoices += choices[maxLeadIndex];
            currentLead = nextLead;
            ++maxLeadIndex;
            if (maxLeadIndex > maxLength) maxLength = maxLeadIndex;
            //if (maxLeadIndex == 2)
            //{
            //    // try and prime touch with single W (3) M (2) above
            //    choices[maxLeadIndex] = 2;
            //}
            //else
            if ( regenPointer >= 0 )
            {
                choices[maxLeadIndex] = choices[regenPointer];
                ++regenPointer;
            }
            else
            {
                choices[maxLeadIndex] = 0;
            }
            leads[maxLeadIndex] = currentLead;

            if (maxLeadIndex < firstUnprovenLead) throw new Exception("Change before lwm");
          }
          else
          {
            // not continuing on from here because not much hope
            bool bDone = BackTrack(ref currentLead, choices, leads, ref maxLeadIndex, int.MaxValue, ref regenPointer);
            if (bDone)
            {
              return;
            }
          }
        }
      }
    }



    private bool BackTrack(ref int currentLead, int[] choices, int[] leads, ref int maxLeadIndex, int lastPossiblyTrueLead,
        ref int regenPointer)
    {
        // copy new changes in from the start
        //regenPointer = 0;

        ++choices[maxLeadIndex];
      

      while (choices[maxLeadIndex] == _tables.NO_CHOICES || maxLeadIndex > lastPossiblyTrueLead
          // backtrack if ends with a call
          //|| ( maxLeadIndex == 62 && choices[maxLeadIndex] > 0) 
          )
      {
        if (maxLeadIndex > 0)
        {
          --maxLeadIndex;
          currentLead = leads[maxLeadIndex];
          //totalChoices -= choices[maxLeadIndex];
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


    private bool IsNotTriviallyFalseOrRepetitive(int nextLead, int[] leads, int[] choices, int maxLeadIndex)
    {
      Method m = _tables._methodsByChoice[choices[maxLeadIndex]];
      for (int i = 0; i <= maxLeadIndex; ++i)
      {
          if (leads[i] == nextLead)
          {
              return false;
          }

          // Filter out repeated methods for an 8-spliced search
          //if (_tables._methodsByChoice[choices[i]] == m)
          //{
          //    if (i < maxLeadIndex)
          //    {
          //        return false;
          //    }
          //}
      }
      return true;
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
   
    private void WriteComposition()
    {
      lock (this)
      {
     
        ++totalCompositions;

        _composition.CalcStats();

        int totalScore = _composition._music;// -_composition.Calls;
        var quality = _composition._quality;
        //quality = _composition.choices.Where((c, i) => i <= noLeads && c > 2).Count();
        var calls = bestMusic.Length - 1 - _composition._calls;
        //_composition.CalcWraps();

       // compositionsWithMusicScore[totalScore]++;
        int changes = _composition._changes;
        var group = calls;
        //var group = 0; // just look for most music
        //if ( changes == 2016 )
           

        //     if(
        //    changes>=1250 &&
        //    // changes == 2015 - (6 * 7 * 32) &&
        //    // changes == maxLeads * 32 - 1 &&
        //    //changes % 32 == 31 &&
        //    // _composition.Calls < 9
        //    //(changes % 2 != 0 ) &&
        //    //changes == 2015 && 
        //    (
        //  totalScore >= bestMusic[group] ||
        //  quality >= bestQuality[group] 
        //  //||calls >=bestCalls[changes]
        //  )

        //// &&    quality > bestQuality[group] 
        //    //totalScore >= 120 &&
        //    // _composition.Calls < 10&&
        //    // totalScore > 2&&
        //    // true
        //  )
        if( totalScore > bestMusic[group])
        {
            if (_receiver != null)
            {
                _composition.TimeToFind = DateTime.UtcNow - _startTime;
                _receiver.AddComposition(_composition.Clone());
            }

            if (totalScore > bestMusic[group])
            {
                bestMusic[group] = totalScore;
            }
            if (totalScore > bestTotalMusic)
            {
                bestTotalMusic = totalScore;
            }
            if (quality > bestQuality[group])
            {
                bestQuality[group] = quality;
            }
            if (calls > bestCalls[group])
            {
                bestCalls[group] = calls;
            }
            return;
           
        }
       
      }
    }
  }
}
