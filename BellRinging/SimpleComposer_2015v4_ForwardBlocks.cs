using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace BellRinging
{
    public class SimpleComposer_2015v4_ForwardBlocks
    {
        Tables _tables = new Tables();
        ICompositionReceiver _receiver;
        SharedStats sharedStats = new SharedStats();

        public ICompositionReceiver Receiver { get { return _receiver; } set { _receiver = value; } }

        public void StartCompose(Action initFunction)
        {

            //compositionsWithMusicScore = new int[1 + _tables.maxMusic * _tables.MAX_LEADS];

            //DoComposeWrapper(null);

            System.Threading.ThreadPool.QueueUserWorkItem((o) => DoComposeWrapper(initFunction));
            _tables.readyToCompose.Wait();

            for (int firstChoice = 0; firstChoice < _tables.NO_CHOICES; ++firstChoice)
            {
                var solver = new Solver(sharedStats, _tables, problem, _receiver, firstChoice);
                var thread = new System.Threading.Thread(() => {
                    try
                    {
                        lock (activeSolvers)
                        {
                            activeSolvers.Add(solver);
                        }
                        solver.DoComposeWrapper();
                    }
                    finally
                    {
                        lock ( activeSolvers)
                        {
                            activeSolvers.Remove(solver);
                        }
                    }
                });
                thread.Name = "Solver#" + firstChoice;
                thread.IsBackground = true;
                thread.Start();
            }
        }


        System.Collections.Generic.HashSet<Solver> activeSolvers = new System.Collections.Generic.HashSet<Solver>();

        public string DescribeState()
        {
            lock (activeSolvers)
            {
                return string.Join(" ", activeSolvers.Select(x => x.Id).OrderBy(x => x));
            }
        }

        void DoComposeWrapper(Action initFunction)
        {
            try
            {
                initFunction();
                //Console.WriteLine("Composition complete in " + (DateTime.UtcNow - start));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        Problem problem;
        public void Initialise(string method2)
        {
            MethodLibrary lib = new MethodLibrary();
            int blockLength = 14;
            int parts = 3;
            int l = 32 * parts * blockLength; // 2016;// -10 * 32;
            sharedStats.bestTotalMusic = 0; // start looking for some music
            //Problem p = new Problem()
            //{
            //    TenorsTogether = false,
            //    AllowSingles = true,
            //    MinLeads = (int) (l/32-1),
            //    MaxLeads = l/32 ,
            //    MinLength = l,
            //    MaxLength = l,5
            //    Reverse = false,
            //    BlockLength = blockLength,
            //    VariableHunt = true,
            //    MusicDelta = 10,
            //    RotateCompositions = false
            //};

            Problem p = new Problem()
            {
                TenorsTogether = true,
                AllowSingles = false,
                MinLeads = (l / 32) - 1,
                MaxLeads = l / 32,
                MinLength = l / 2,
                MaxLength = l,
                Reverse = false,
                BlockLength = blockLength,
                VariableHunt = false,
                MusicDelta = 50,
                RotateCompositions = true,
                ExcludeUnrungMethodsFromBalance = false
            };

            /*
             1,792 Spliced Surprise Major (8 methods)
    2345678   W   H
    42356         -   B.
    35426     -   -   RC.L.
    3527486   -       YSNP.
    seven part 
    Bristol, Cambridge, Lincolnshire (N), London, Pudsey, Rutland, Superlative, Yorkshire
             */
            var vhCall = p.VariableHunt ? "34" : null;
            this.problem = p;
            int index = 0;
            foreach (string method in
              new string[] { 
             
              
              
             "Rutland"
              , "London"
              , "Superlative"
              , "Pudsey"
           , "Cambridge"
             , "Lincolnshire", "Yorkshire" , "Bristol"
              /*
             // "Lessness",
             //"Lindum",
           // "Uxbridge", 
            "London",
           //  "Cassiobury",
             //"Preston",
             "Superlative",
             //"Cornwall", 
             "Yorkshire",
            "Cambridge" , 
        "Rutland",    
        "Lincolnshire",
        "Glasgow" ,
        //,"Ashtead",
              "Pudsey",
              "Bristol"
         //     ,
              
         
         
       
              //,
              //"Bastow"
             // "Superlative",
            // "Bastow"//,"Bastow"
          
         // ,"Superlative"
         // 
         // ,"London"
         // ,"Cambridge"
         // ,"Bristol"
         //, "Pudsey", "Lincolnshire"
         // ,"Rutland"
          //"Belfast", //"Glasgow",
          //"Londonderry",*/
           })
            {
                ++index;
                char letter = method[0];
                if (method == "Lincolnshire") letter = 'N';
                if (method == "Belfast") letter = 'F';
                if (method == "Cassiobury") letter = 'O';
                if (method == "Cornwall") letter = 'E';
                if (method == "Lindum") letter = 'M';
                if (method == "Wembley") letter = 'X';
                if (method == "Cray") letter = 'K';
                if (method == "Preston") letter = 'H';
                Method methodObject;
                if (method == "Bastow")
                {

                    methodObject = new Method(method, "β", "X12-18", 8, p.AllowSingles, vhCall);
                }
                else
                {

                    methodObject = new Method(method, letter.ToString(), lib.GetNotation(method), 8, p.AllowSingles, vhCall);
                }
                if (method == "Bastow")
                {
                    //methodObject.FirstLeadOnly();
                    methodObject.LastLeadOnly();
                }
                p.AddMethod(methodObject);
            }
            if (p.Reverse)
            {
                var start = new Method("Null", " EC ", null, 8, p.AllowSingles);
                //start.LastLeadOnly();
                p.AddMethod(start);
                p.FirstChoice = 3;
                start.FirstLeadOnly();
            }

            // at least for Y and S then Bastow start seems to imply need to finish with bob
            //p.FirstChoice = (p.AllowSingles ? 3 : 2) * (p.Methods.Count() - 1) +1;
            Task[] initErs = p.Methods.Select(m => new Task(() =>
            {
                var x = m.AllLeads.Count();
            })).ToArray();
            foreach (var t in initErs)
            {
                t.Start();
            }
            Task.WaitAll(initErs);
            p.MusicalPreferences = new MusicalPreferences();
            p.MusicalPreferences.Init();
            _tables.Initialise(p);

            if (false)
            {
                var ape = "12345678";
                //for (int i = 0; i < 7; ++i)
                //{
                //    ape = ape.Substring(1) + ape[0];
                //    allowedPartEnds.Add(Row.FromString(ape).ToNumber());
                //}
                for (int i = 1; i < 8; ++i)
                {
                    ape = ape.Substring(1) + ape[0];
                    // 8-part
                    if (i == 1 || i == 3 || i == 5 || i == 7)
                    // 4 - part

                    // if (i == 2 || i == 6)
                    {
                       problem.allowedPartEnds.Add(Row.FromString(ape).ToNumber());
                    }
                }
            }
            //allowedPartEnds.Add(Row.FromString("42316857").ToNumber());

            /*
             // cyclic part ends
             problem.allowedPartEnds.Add(Row.FromString("13456782").ToNumber());
             problem.allowedPartEnds.Add(Row.FromString("14567823").ToNumber());
             problem.allowedPartEnds.Add(Row.FromString("15678234").ToNumber());
             problem.allowedPartEnds.Add(Row.FromString("16782345").ToNumber());
             problem.allowedPartEnds.Add(Row.FromString("17823456").ToNumber());
             problem.allowedPartEnds.Add(Row.FromString("18234567").ToNumber());

             // plain bob part ends
             problem.allowedPartEnds.Add(Row.FromString("13527486").ToNumber());
             problem.allowedPartEnds.Add(Row.FromString("14263857").ToNumber());
             problem.allowedPartEnds.Add(Row.FromString("15738264").ToNumber());
             problem.allowedPartEnds.Add(Row.FromString("16482735").ToNumber());
             problem.allowedPartEnds.Add(Row.FromString("17856342").ToNumber());
             problem.allowedPartEnds.Add(Row.FromString("18674523").ToNumber());  
             */


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

        class SharedStats
        {            
          public volatile int bestTotalMusic;
        }

        class Solver
        {
            SharedStats sharedStats;
            Tables _tables;
            Problem problem;
            ICompositionReceiver _receiver;
            int firstChoice;

            public int Id
            {
                get
                {
                    return firstChoice;
                }
            }
            public Solver(SharedStats sharedStats, Tables tables, Problem problem, ICompositionReceiver receiver, int firstChoice)
            {
                this.problem = problem;
                this._tables = tables;
                this.sharedStats = sharedStats;
                this._receiver = receiver;
                this.firstChoice = firstChoice;


                bComposing = true;
                _tables.readyToCreateComposition.Wait();


                maxLeads = _tables.MAX_LEADS;

                _composition = new Composition(_tables);
                _composition.Problem = problem;
                _composition.Reverse = problem.Reverse;

                bestMusic = new int[maxLeads * 32 + 1];
                bestQuality = new int[maxLeads * 32 + 1];
                bestCalls = new int[maxLeads * 32 + 1];

                int maxGroup = maxLeads * 1000 + maxLeads + 1;
                bestMusic = new int[maxGroup];
                bestQuality = new int[maxGroup];
                bestCalls = new int[maxGroup];

            }

            // best music found at a given number of leads
            int[] bestMusic;
            int[] bestQuality;
            int[] bestCalls;

            int totalMusic = 0;
            //int totalChoices = 0;

            long totalLeads;
            int maxLength;
            int totalCompositions;

            int maxLeads;

            bool bComposing;


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
                    return string.Join(".", temp.choices.Select(c => c.ToString()));
                    //return temp.ToString();
                    //return minBackTrackPoint.ToString() + "/" + timesToPoint.ToString();
                }
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

            public void DoComposeWrapper()
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
                int regenPointer = -1;

                int[] choices = _composition.choices;
                int[] leads = _composition.leads;

                // lead is the lead just rung, call is the call made from that lead
                choices[0] = firstChoice;
                if ( problem.FirstChoice != 0 )
                {
                    throw new Exception("First choice for problem not supported");
                }

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
                    var nextLead = _tables.leadMapping[currentLead, choices[maxLeadIndex]];


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
                        int minBackTrack = maxLeadIndex;
                        // if (true /*falseAt < 0*/)

                        if (falseAt < 0)
                        {
                            totalMusic += _tables.music[currentLead, choices[maxLeadIndex]];
                            WriteComposition(maxLeadIndex, ref  minBackTrack);
                            totalMusic -= _tables.music[currentLead, choices[maxLeadIndex]];
                        }
                        else
                        {
                            minBackTrack = falseAt;
                        }



                        // falseAt is the first lead that was false - therefore we made either a false choice of method
                        // at that lead or a false call at the previous lead
                        bool bDone = BackTrack(ref currentLead, choices, leads, ref maxLeadIndex, (int)minBackTrack);
                        regenPointer = 0;
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


                         // looking likely for good music
                            //  && totalMusic >= (maxLeadIndex  * bestTotalMusic)/maxLeads - problem.MusicDelta

                          && IsNotTriviallyFalseOrRepetitive(nextLead, leads, choices, maxLeadIndex)

                            //&& (maxLeadIndex == 0 ||
                            //(choices[maxLeadIndex - 1] / 2 == choices[maxLeadIndex] / 2)
                            //|| (choices[maxLeadIndex - 1] % 2 != 0
                            //&& (choices[maxLeadIndex - 1] / 2 != choices[maxLeadIndex] / 2)
                            //)
                            //)

                            // cannot check part ends here because part ends are not preserved by rotation
                            // nice part ends
                            //&& ( (maxLeadIndex != problem.BlockLength-1) || allowedPartEnds.Contains(nextLead)) 

                            // use all the methods
                        && (maxLeadIndex > problem.BlockLength || _composition.Imbalance < 3)


                        && (maxLeadIndex > problem.BlockLength || _composition.Calls <5)

                        && (maxLeadIndex > problem.BlockLength || _composition.COM < Math.Min(9,1 + 9 *maxLeadIndex/problem.BlockLength))

                        && (maxLeadIndex != problem.BlockLength - 1 || 
                          problem.allowedPartEnds.Count == 0 || problem.allowedPartEnds.Contains(nextLead))

                       // && _composition.Calls < 21
                            // avoid lots of singles
                            //&& ( maxLeadIndex > maxLeads - 5 || choices[maxLeadIndex] != 2 )


                          // this is expensive - better to work out once got something that comes round
                            //&&  IsTrue(nextLead, leads, maxLeadIndex)  // next lead is true against what got so far

                          // obsoleted by allowed part ends for cyclic composition
                            // && maxLeadIndex + _tables.leadsToEnd[nextLead] < maxLeads // could come round in time

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
                            if (maxLeadIndex < problem.BlockLength)
                            {
                                if (problem.RotateCompositions && regenPointer >= 0)
                                {
                                    choices[maxLeadIndex] = choices[regenPointer];
                                    ++regenPointer;
                                }
                                else
                                {
                                    choices[maxLeadIndex] = 0;
                                }
                            }
                            else
                            {
                                choices[maxLeadIndex] = choices[maxLeadIndex - problem.BlockLength];
                            }
                            leads[maxLeadIndex] = currentLead;

                            if (maxLeadIndex < firstUnprovenLead) throw new Exception("Change before lwm");
                        }
                        else
                        {
                            bool bDone = BackTrack(ref currentLead, choices, leads, ref maxLeadIndex, int.MaxValue);
                            regenPointer = 0;
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
                //if (maxLeadIndex < problem.BlockLength)
                {
                    ++choices[maxLeadIndex];
                }


                while (choices[maxLeadIndex] == _tables.NO_CHOICES || maxLeadIndex > lastPossiblyTrueLead
                    || maxLeadIndex >= problem.BlockLength
                    // backtrack if ends with a call
                    //|| ( maxLeadIndex == 62 && choices[maxLeadIndex] > 0) 
                    )
                {
                    // first choice handled by multiple solvers
                    //if (maxLeadIndex > 0)
                    if (maxLeadIndex > 1)
                    {
                        --maxLeadIndex;
                        currentLead = leads[maxLeadIndex];
                        totalMusic -= _tables.music[currentLead, choices[maxLeadIndex]];
                        //totalChoices -= choices[maxLeadIndex];
                        //if (maxLeadIndex < problem.BlockLength)
                        {
                            ++choices[maxLeadIndex];
                        }


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

            private void WriteComposition(int noLeads, ref int minBackTrack)
            {
                lock (this)
                {
                    bool proven = true;

                    var rotMax = 1;
                    if (problem.BlockLength > 0 && problem.RotateCompositions)
                    {
                        rotMax = Math.Min(problem.BlockLength, _composition.maxLeadIndex);
                    }
                    for (_composition.rot = 0; _composition.rot < rotMax; ++_composition.rot)
                    {
                        ++totalCompositions;

                        _composition.CalcStats();
                        if (maxLeads <= problem.BlockLength || problem.allowedPartEnds.Count == 0 || 
                            problem.allowedPartEnds.Contains(_composition._partEnd))
                        {
                            int totalScore = _composition.Score;// -_composition.Calls;
                            var quality = _composition.COM;
                            //quality = _composition.choices.Where((c, i) => i <= noLeads && c > 2).Count();
                            var calls = bestMusic.Length - 1 - _composition.Calls;
                            //_composition.CalcWraps();

                            // compositionsWithMusicScore[totalScore]++;
                            int changes = _composition.Changes;
                            //var group = calls;
                            var group = _composition.COM * 1000 + _composition.Calls; // just look for most music
                            //if ( changes == 2016 )


                            if (//true ||
                          !(changes < problem.MinLength) &&
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
                                if (!proven)
                                {

                                    var falseAt = _composition.RunsFalseAt5(ref firstUnprovenLead);
                                    if (falseAt >= 0)
                                    {
                                        minBackTrack = falseAt;
                                        return;
                                    }
                                    proven = true;
                                }
                                if (_receiver != null)
                                {
                                    _composition.TimeToFind = DateTime.UtcNow - _startTime;
                                    _receiver.AddComposition(_composition.Clone());
                                }

                                if (totalScore > bestMusic[group])
                                {
                                    bestMusic[group] = totalScore;
                                }
                                lock (sharedStats)
                                {
                                    if (totalScore > sharedStats.bestTotalMusic)
                                    {
                                        sharedStats.bestTotalMusic = totalScore;
                                    }
                                }
                                if (quality > bestQuality[group])
                                {
                                    bestQuality[group] = quality;
                                }
                                if (calls > bestCalls[group])
                                {
                                    bestCalls[group] = calls;
                                }
                                //return;
                                //Console.WriteLine("Leads = " + (noLeads + 1) + " ( " + (noLeads + 1) * 32 + "  changes) Total music " + totalMusic);
                                //for (int i = 0; i <= noLeads; ++i)
                                //{
                                //    Console.WriteLine(Row.FromNumber(_composition.leads[i]) + " " + "-BSZ"[_composition.choices[i]] + " " + _tables.music[_composition.leads[i], _composition.choices[i]]);
                                //}
                            }
                        }
                    }
                    _composition.rot = 0;
                }
            }
        }
    }
}
