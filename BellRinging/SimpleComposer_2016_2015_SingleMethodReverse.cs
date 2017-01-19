using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BellRinging
{
    public class SimpleComposer_2016_2015_SingleMethodReverse 
    {
    Tables _tables = new Tables();
    ICompositionReceiver _receiver;

    public ICompositionReceiver Receiver { get { return _receiver; } set { _receiver = value; } }


    public string DescribeState()
    {
        return _composition.ToString();
        
    }

    Problem problem;
    public void Initialise(string method2)
    {
        MethodLibrary lib = new MethodLibrary();
        //int l = 7 * 32 * 9 +1;// -10 * 32;
        int l = 2019;
        bestTotalMusic = 140; //225 * l / 2500; // start looking for some music
        var backstrokeStart = true;
        //backstrokeStart = false;
        Problem p = new Problem()
        {
            TenorsTogether = true,
            AllowSingles = true,
            
            MaxLeads = l/32 +2,
            MinLength = l,
            MaxLength = l, //10*32*7,
            Reverse = false,
            MusicDelta = 40,
            MaxCalls = 11,
            MaxSingles = 999,
            MaxBobs=999
        };
        this.problem = p;
        int index = 0;
        var methods  = new string[] {
            // "Glasgow"
             // "London", 
              //"Pudsey"
              //,
              "Superlative"
              //,
              //"Bastow"
             // ,
            // "Bastow"//,
          //"Superlative"
         // ,"Superlative"
         // 
         // ,"London"
         // ,"Cambridge"
         // ,
         //"Bristol"
         //, "Pudsey", "Lincolnshire"
          //"Belfast", //"Glasgow",
          //"Londonderry",
           };

        foreach (string method in
         methods)
        {
            ++index;
            char letter = method[0];
            if (method == "Lincolnshire") letter = 'N';
            if (method == "Belfast") letter = 'F';
            Method methodObject;
            if ( method == "Bastow")
            {

                methodObject = new Method(method, "β", "X12-18", 8, p.AllowSingles);
            }
            else
            {
                var not2 = lib.GetNotation(method);
               //  Orm Embar S Major: b -38-1458-56-36.14-34.58.14-16.78
                //Yevaud S Major: g -38-1458-56-36.14-34.58.14-16.78
                //Tehanu S Major: d -58-14-56-36-12-58.34-16.78
                //Kalessin S Major: j -58-14-56-36-12-58.34-16.78
                //Bar Oth S Major: d -58-14-56-36-34-58.12-16.78
                //Orm Irian S Major: j -58-14-56-36-34-58.12-16.78   
                
                // 9-call 140 - var not2 = "-38-1458-56-36.14-34.58.14-16.78".Replace("-", "x") + "-12";
                // 12-call 152 - var not2 = "-38-1458-56-36.14-34.58.14-16.78".Replace("-", "x") + "-18";
                // 14-call 129 var not2 = "-58-14-56-36-12-58.34-16.78".Replace("-", "x") + "-12";
                // 19-call 127 var not2 = "-58-14-56-36-12-58.34-16.78".Replace("-", "x") + "-18";
                
                // 14-call 128 var not2 = "-58-14-56-36-34-58.12-16.78".Replace("-", "x") + "-12";
                // 19-call 124 var not2 = "-58-14-56-36-34-58.12-16.78".Replace("-", "x") + "-18";
                //  (136 with snap start)
                //
                // c.f. Glasgow 91
                // Bristol > 160
                // Superlative 134
             methodObject = new Method(method, letter.ToString(), not2, 8, p.AllowSingles);
            }
            if (method == "Bastow")
            {
                //methodObject.FirstLeadOnly();
                methodObject.LastLeadOnly();
            }
            p.AddMethod(methodObject);
        }
        if (p.Reverse || backstrokeStart)
        {
            var start = new Method("PlainLeadEnd", " EC ", null, 8, p.AllowSingles);
            //start.LastLeadOnly();
            p.AddMethod(start);
            p.FirstChoice = p.AllowSingles ? 3 : 2;
            //p.FirstChoice = p.AllowSingles ? 5 : 4;
            start.FirstLeadOnly();
        }

        if (methods.Contains("Bastow"))
        {
            // at least for Y and S then Bastow start seems to imply need to finish with bob
            p.FirstChoice = (p.AllowSingles ? 3 : 2) * (p.Methods.Count() - 1) + 1;
        }
       

        p.MusicalPreferences = new MusicalPreferences();
        p.MusicalPreferences.Init();
        _tables.Initialise(p);
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

    // best music found at a given number of leads
    int[] bestMusic;
    int[] bestQuality;
    int[] bestCalls;
    int bestTotalMusic;

    int totalMusic = 0;
    //int totalChoices = 0;

    long totalLeads;
    int maxLength;
    int totalCompositions;

    int maxLeads;

    bool bComposing;

    public void StartCompose(Action initFunction)
    {

      //compositionsWithMusicScore = new int[1 + _tables.maxMusic * _tables.MAX_LEADS];

      //DoComposeWrapper(null);

      bComposing = true;
      System.Threading.ThreadPool.QueueUserWorkItem((o) =>DoComposeWrapper(initFunction));
      _tables.readyToCreateComposition.Wait();


      maxLeads = _tables.MAX_LEADS;

      _composition = new Composition(_tables);
      _composition.Problem = problem;
      _composition.Reverse = problem.Reverse;

      bestMusic = new int[maxLeads * 32 + 1];
      bestQuality = new int[maxLeads * 32 + 1];
      bestCalls = new int[maxLeads * 32 + 1];

      int maxGroup = maxLeads * 1000 + maxLeads;
      bestMusic = new int[maxGroup];
      bestQuality = new int[maxGroup];
      bestCalls = new int[maxGroup];

      readyToCompose.Set();
    }

    public System.Threading.ManualResetEventSlim readyToCompose = new System.Threading.ManualResetEventSlim(false);
    
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
          //return "temp stubbed";
           var temp = _composition.Clone();
           return string.Join("", temp.choices.Select(c => c.ToString()));
           //return temp.ToString();
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
          readyToCompose.Wait();
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
      choices[0] = problem.FirstChoice;

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
        int snapFinish = -1;
        if (nextLead >= 0 && (snapFinish = _tables._methodsByChoice[choices[maxLeadIndex]].Lead(currentLead).ContainsRoundsAt) > 0)
        {
            Console.WriteLine("Found snap");
        }
        end = snapFinish > 0 || nextLead == start;
        //end = snapFinish > 0 ;//|| nextLead == start;

        // no snap version
        //end = nextLead == start;
        //if (nextLead >= 0 && _tables._methodsByChoice[0].Lead(nextLead).ContainsRoundsAt > 0)
        //{
        //  Console.WriteLine("Found snap");
        //}
        if (end
            // Force a minimum touch length
            && maxLeadIndex >= problem.MinLeads
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
         

          int firstUnprovenLead = -1; // no false check !!! TODO:
          var falseAt = _composition.RunsFalseAt5(ref firstUnprovenLead);
          int minBackTrack;
         // if (true /*falseAt < 0*/)
          
          if (falseAt < 0 )
          {
              //only want longest compositions
              //if (maxLeadIndex == maxLeads-1)
              {
                  totalMusic += _tables.music[currentLead, choices[maxLeadIndex]];
                  WriteComposition(maxLeadIndex);
                  totalMusic -= _tables.music[currentLead, choices[maxLeadIndex]];
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
          bool bDone = BackTrack(ref currentLead, choices, leads, ref maxLeadIndex, (int)minBackTrack);
          if (bDone)
          {
            return;
          }
        }
        else
        {

            // required for imbalance check
            _composition.maxLeadIndex = maxLeadIndex;

          // continue down tree if
          if (nextLead >= 0  // choice is allowed

            && maxLeadIndex < maxLeads - 1 // array length (else continue from rounds)
            // change of method only at a bob
/* Prime Cam/York/Super out of start
 *          && (maxLeadIndex != 1 || choices[maxLeadIndex] == 0)
            && (maxLeadIndex != 2 || choices[maxLeadIndex] == 2)
            //&& (maxLeadIndex != 3 || choices[maxLeadIndex] == 2)*/

           // looking likely for good music
           && totalMusic >= (maxLeadIndex  * bestTotalMusic)/maxLeads - problem.MusicDelta

            && IsNotTriviallyFalseOrRepetitive(nextLead, leads, choices, maxLeadIndex)

          //    && (maxLeadIndex == 0 ||
          //    (choices[maxLeadIndex - 1] / 2 == choices[maxLeadIndex] / 2)
          //    || ( choices[maxLeadIndex - 1] % 2 != 0
          //    && ( choices[maxLeadIndex - 1] / 2 != choices[maxLeadIndex] / 2)
          //    )
          //    )

          //&& _composition.Imbalance < 5
          && _composition.Calls <= problem.MaxCalls
          && _composition.Singles <= problem.MaxSingles
          && _composition.Calls - _composition.Singles <= problem.MaxBobs

         // && ( maxLeadIndex != 16 ||)
              // avoid lots of singles
         // && ( maxLeadIndex > maxLeads - 5 || choices[maxLeadIndex] != 2 )


            // this is expensive - better to work out once got something that comes round
            //&&  IsTrue(nextLead, leads, maxLeadIndex)  // next lead is true against what got so far

             && maxLeadIndex + _tables.leadsToEnd[nextLead] < maxLeads // could come round in time

            // constraint to go through Cambridge group sBIM finish
            //&& (nextLead == 3910 || maxLeadIndex != maxLeads - 5 ) // 63 - 5 = 58

            // try and get a largely tenors together compostion
            //&& ( maxLeadIndex > 58 || Row.FromNumber(nextLead).IsTenorsTogetherLeadEnd)

            //&& totalChoices < 30


           //&& totalMusic * maxLeads >= (80 * maxLeadIndex * bestTotalMusic) / 100 - 5

            // aiming for superlative score above 90)
            //&& totalMusic >= maxLeadIndex * ( 90.0/63 * 0.8) - 5
            //&& totalMusic >= maxLeadIndex * (260 / 63 * 0.8) - 5
            // && totalMusic >= maxLeadIndex * (700 / 63 * 0.8) - 5 // was 473
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
            {
                choices[maxLeadIndex] = 0;
            }
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
      

      while (choices[maxLeadIndex] == _tables.NO_CHOICES || maxLeadIndex > lastPossiblyTrueLead
          // backtrack if ends with a call
          //|| ( maxLeadIndex == 62 && choices[maxLeadIndex] > 0) 
          )
      {
        if (maxLeadIndex > 0)
        {
          --maxLeadIndex;
          currentLead = leads[maxLeadIndex];
          totalMusic -= _tables.music[currentLead, choices[maxLeadIndex]];
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
      int falseAt = _composition.RunsFalseAt5(ref firstUnprovenLead);
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
   
    private void WriteComposition(int noLeads)
    {
      lock (this)
      {
     
        ++totalCompositions;

        _composition.CalcStats();

        int totalScore = _composition.Score;// -_composition.Calls;
        var quality = _composition.COM;
        //quality = _composition.choices.Where((c, i) => i <= noLeads && c > 2).Count();
        var calls = bestMusic.Length-1- _composition.Calls;
        //_composition.CalcWraps();

       // compositionsWithMusicScore[totalScore]++;
        int changes = _composition.Changes;
        //var group = calls;
        var group = _composition.COM * 1000 + _composition.Calls; // just look for most music
        //if ( changes == 2016 )


        if (//true ||
      !(changes< problem.MinLength) &&
           !(changes > problem.MaxLength) &&
//_composition.COM> 10&&
 //_composition.Imbalance < 10&&
            // changes == 2015 - (6 * 7 * 32) &&
            // changes == maxLeads * 32 - 1 &&
            //changes % 32 == 31 &&
            // _composition.Calls < 9
            //(changes % 2 != 0 ) &&
            //changes == 2015 && 
            (
          totalScore > bestMusic[group] ||
          quality > bestQuality[group] 
          //||calls >=bestCalls[changes]
          )

        // &&    quality > bestQuality[group] 
            //totalScore >= 120 &&
            // _composition.Calls < 10&&
            // totalScore > 2&&
            // true
          )
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
            Console.WriteLine("Leads = " + (noLeads + 1) + " ( " + (noLeads + 1) * 32 + "  changes) Total music " + totalMusic);
            for (int i = 0; i <= noLeads; ++i)
            {
                Console.WriteLine(Row.FromNumber(_composition.leads[i]) + " " + "-BSZ"[_composition.choices[i]] + " " + _tables.music[_composition.leads[i], _composition.choices[i]]);
            }
        }
       
      }
    }
  }
}
