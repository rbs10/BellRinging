using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace BellRinging
{
  public class Composition
  {
    // arrays - public for speed of access in main composing loop

    // the call made at the end of each lead
    // and the method the lead was rung in
    public int[] choices;

    // the row rung at the start of each lead
    public short[] leads;

    // the index of the final lead in the composition
    public short maxLeadIndex;

    // the rotation to be applied to the tables when interpreting the composition
    public int rot;

    Tables _tables;

    TimeSpan _timeToFind;


    int[] rowCount;// = new int[40320];
    int[] toClear;// = new int[5040];

  

    public Composition(bool supportFalsenessCheck)
    {
      if (supportFalsenessCheck)
      {
        InitFalsenessCheckSupport();
      }
    }

    private void InitFalsenessCheckSupport()
    {
        rowCount = new int[40320];
      toClear = new int[5040];
    }

    public Composition(Tables t)
    {
      _tables = t;
      int maxLeads = t.MAX_LEADS;
      choices = new int[maxLeads];
      leads = new short[maxLeads];
      InitFalsenessCheckSupport();
    }

    internal Composition Clone()
    {
      Composition clone = new Composition(false);
      clone.choices = this.choices.Clone() as int[];
      clone._tables = this._tables;
      clone.rot = this.rot;
      clone.maxLeadIndex = this.maxLeadIndex;
      clone.leads = this.leads.Clone() as short[];
      clone._timeToFind = _timeToFind;
      return clone;
    }

    private Composition()
    {
    }


    public string ToLongString(bool spaceOutBackstrokes)
    {
      StringBuilder sb = new StringBuilder();
      string roundsString = Row.RoundsString();
      bool isBackstroke = true; // start with final rounds
      foreach (Row r in Rows)
      {
        foreach (char bellChar in r.ToString())
        {
          int index = roundsString.IndexOf(bellChar);
          sb.Append((char)('1' + index));
        }
        if (isBackstroke && spaceOutBackstrokes)
        {
          sb.Append(" ");
        }
        isBackstroke = !isBackstroke;
      }
      return sb.ToString();
    }

    Regex r1 = new Regex("12345678");

    public int CalcWraps()
    {
      string s = ToLongString(true);
      int count = 0;
      return r1.Matches(s).Count;
      //return r1.Match(s).Captures.Count;
    }

    public void Beep()
    {
      string roundsString = Row.RoundsString();
      int duration = 35 * 60 * 1000 / 1260/8;
      bool isBackstroke = true; // start with final rounds
      foreach (Row r in Rows)
      {
        foreach (char bellChar in r.ToString())
        {
          int index = roundsString.IndexOf(bellChar);
          
          int freq = (int)( 400 * Math.Pow(2.0,index / 7.0));
          Console.Beep(freq, duration);
          System.Threading.Thread.Sleep(duration/20);
          
        }
        if (isBackstroke)
        {
          System.Threading.Thread.Sleep(duration);
        }
        isBackstroke = !isBackstroke;
      }
    }

    int oldIndex;

    public short RunsFalseAt5(ref short firstUnprovenLead)
    {
        unsafe
        {
            //return -1;
            fixed (int* pRowCount = rowCount)
            {
                //blankRowCount.CopyTo(rowCount, 0);

                int newIndex = oldIndex + 1;
                if (newIndex == int.MaxValue)
                {
                    for (int i = 0; i < rowCount.Length; ++i)
                    {
                        rowCount[i] = 0;
                    }
                    oldIndex = 0;
                    newIndex = 1;
                }

                var methodsByChoice = _tables._methodsByChoice;
                var choices = this.choices;
                oldIndex = newIndex;
                for (short i = 0; i <= maxLeadIndex; ++i)
                {
                    short leadHead1 = leads[i];
                    //int method1 = _tables.methodIndexByChoice[choices[i]];
                    Method m = methodsByChoice[choices[i]];
                    Lead l = m.Lead(leadHead1);
                    //foreach (int r in l.RowsAsInts)
                    var rows = l.RowsAsInts;
                    fixed (int* rowP = rows)
                    {
                        for (int j = 0; j < rows.Length; ++j)
                        {
                            var r = *(rowP + j);

                            var thisRowCountP = pRowCount + r;
                            if (*thisRowCountP != newIndex)
                            {
                                *thisRowCountP = newIndex;
                            }
                            else
                            {
                                // false - but started in rounds so if this is the end OK - otherwise too early and false
                                if (r == 0 && (i > 0 || j > 0) && i == maxLeadIndex) break;

                                return i;
                            }
                        }
                    }
                }
                return -1;
            }
        }
    }


    public short RunsFalseAt4(ref short firstUnprovenLead)
    {
        unsafe
        {
            //return -1;

            //blankRowCount.CopyTo(rowCount, 0);

            int newIndex = oldIndex + 1;
            if (newIndex == int.MaxValue)
            {
                for (int i = 0; i < rowCount.Length; ++i)
                {
                    rowCount[i] = 0;
                }
                oldIndex = 0;
                newIndex = 1;
            }

            var methodsByChoice = _tables._methodsByChoice;
            var choices = this.choices;
            oldIndex = newIndex;
            for (short i = 0; i <= maxLeadIndex; ++i)
            {
                short leadHead1 = leads[i];
                //int method1 = _tables.methodIndexByChoice[choices[i]];
                Method m = methodsByChoice[choices[i]];
                Lead l = m.Lead(leadHead1);
                //foreach (int r in l.RowsAsInts)
                var rows = l.RowsAsInts;
                fixed (int* rowP = rows)
                {
                    for (int j = 0; j < rows.Length; ++j)
                    {
                        var r = *(rowP + j);


                        if (rowCount[r] != newIndex)
                        {
                            rowCount[r] = newIndex;
                        }
                        else
                        {
                            // false - but started in rounds so if this is the end OK - otherwise too early and false
                            if (r == 0 && (i > 0 || j > 0) && i == maxLeadIndex) break;

                            return i;
                        }
                    }
                }
            }
            return -1;
        }
    }

    public short RunsFalseAt3(ref short firstUnprovenLead)
    {
        //return -1;

        //blankRowCount.CopyTo(rowCount, 0);

        int newIndex = oldIndex + 1;
        if (newIndex == int.MaxValue)
        {
            for (int i = 0; i < rowCount.Length; ++i)
            {
                rowCount[i] = 0;
            }
            oldIndex = 0;
            newIndex = 1;
        }

        var methodsByChoice = _tables._methodsByChoice;
        var choices = this.choices;
        oldIndex = newIndex;
        for (short i = 0; i <= maxLeadIndex; ++i)
        {
            short leadHead1 = leads[i];
            //int method1 = _tables.methodIndexByChoice[choices[i]];
            Method m =methodsByChoice[choices[i]];
            Lead l = m.Lead(leadHead1);
            //foreach (int r in l.RowsAsInts)
            var rows = l.RowsAsInts;
            for (int j = 0; j < rows.Length; ++j)
            {
                var r = rows[j];


                if (rowCount[r] != newIndex)
                {
                    rowCount[r] = newIndex;
                }
                else
                {
                    // false - but started in rounds so if this is the end OK - otherwise too early and false
                    if (r == 0 && (i > 0 || j > 0) && i == maxLeadIndex) break;

                    return i;
                }
            }
        }
        return -1;
    }


    /// <summary>
    /// Return where the composition runs false or -1 if the composition is true
    /// </summary>
    /// <returns></returns>
    public short RunsFalseAt2(ref short firstUnprovenLead)
    {
      //return -1;

      //blankRowCount.CopyTo(rowCount, 0);

        int newIndex = oldIndex + 1;
        if ( newIndex == int.MaxValue )
        {
           for ( int i = 0; i < rowCount.Length;++i )
           {
               rowCount[i] = 0;
           }
           oldIndex = 0;
           newIndex = 1;
        }

        oldIndex = newIndex;
      for (short i = 0; i <= maxLeadIndex; ++i)
      {
        short leadHead1 = leads[i];
        //int method1 = _tables.methodIndexByChoice[choices[i]];
        Method m = _tables._methodsByChoice[choices[i]];
        Lead l = m.Lead(leadHead1);
        //foreach (int r in l.RowsAsInts)
        var rows = l.RowsAsInts;
        for ( int j = 0; j < rows.Length; ++j )
        {
          var r = rows[j];
         

          if (rowCount[r] != newIndex)
          {
              rowCount[r] = newIndex;
          }
          else
          { 
            // false - but started in rounds so if this is the end OK - otherwise too early and false
            if (r == 0 && (i > 0 || j > 0) && i == maxLeadIndex) break;

            return i;
          }
        }
      }
      return -1;
    }

    /// <summary>
    /// Return where the composition runs false or -1 if the composition is true
    /// </summary>
    /// <returns></returns>
    public short RunsFalseAt(ref short firstUnprovenLead)
    {
        //return -1;

        //blankRowCount.CopyTo(rowCount, 0);

        int c = 0;
        for (short i = 0; i <= maxLeadIndex; ++i)
        {
            short leadHead1 = leads[i];
            //int method1 = _tables.methodIndexByChoice[choices[i]];
            Method m = _tables._methodsByChoice[choices[i]];
            Lead l = m.Lead(leadHead1);
            //foreach (int r in l.RowsAsInts)
            var rows = l.RowsAsInts;
            for (int j = 0; j < rows.Length; ++j)
            {
                var r = rows[j];
                // has come round
                if (r == 0 && c > 0 && i == maxLeadIndex) break;

                if (rowCount[r] == 0)
                {
                    ++rowCount[r];
                    toClear[c] = r;
                    ++c;
                }
                else
                {
                    //var fakse = Row.FromNumber(r).ToString();
                    for (int ltc = 0; ltc < c; ++ltc) rowCount[toClear[ltc]] = 0;
                    return i;
                }
            }
        }
        for (int ltc = 0; ltc < c; ++ltc) rowCount[toClear[ltc]] = 0;
        return -1;
    }



    /// <summary>
    /// Return where the composition runs false or -1 if the composition is true
    /// </summary>
    /// <returns></returns>
    /// <remarks>Version using falseness tables</remarks>
    public short RunsFalseAt1(ref short firstUnprovenLead)
    {
      //return -1;

      for (short i = 0; i <= maxLeadIndex; ++i)
      {
        short leadHead1 = leads[i];
        int method1 = _tables.methodIndexByChoice[choices[i]];
        // for each lead that we have not checked this lead against already
        for (short i2 = Math.Max(firstUnprovenLead,(short)(i + 1)); i2 <= maxLeadIndex; ++i2)
        {
          short leadHead2 = leads[i2];

          //// plain bob - just check lead ends (only works if in course - else must at least check both ends of the lead!
          //if (leadHead1 == leadHead2)
          //{
          //  firstUnprovenLead = Math.Max(firstUnprovenLead, (short)(i + 1));
          //  return i2;
          //}

          int method2 = _tables.methodIndexByChoice[choices[i2]];
          short[] leadHeads = _tables.falseLeadHeads[method1,method2,leadHead1];

          foreach ( short falseLeadHead in leadHeads )
          {
            if (leadHead2 == falseLeadHead)
            {
              //if (firstUnprovenLead > 0)
              //{
              //   short i0 =0;
              //  short refRes = RunsFalseAt(ref i0);
              //  if (refRes != i2)
              //  {

              //    short i00=0;
              //   refRes = RunsFalseAt(ref i00);
              //    throw new Exception("Error in falseness checking");
              //  }
              //}
              // the top of the pair which is false - therefore we must
              // change at least this to have a chance of being true
              //
              // we do not know about combinations beyond i from this search although we may
              // do from a previous search
              firstUnprovenLead = Math.Max(firstUnprovenLead,(short)(i+1));
              return i2;
            }
          }
        }
      }

      //if (firstUnprovenLead > 0)
      //{

      //  short i0=0;
      //          short refRes = RunsFalseAt(ref i0);
      //  if (refRes >= 0)
      //  {
      //    throw new Exception("Error in falseness checking");
      //  }
      //}
      // whole is true therefore no need to check except for backtracking
      firstUnprovenLead = (short)(maxLeadIndex+1);
      return -1;
    }

    public override string ToString()
    {
        if (_tables._methodsByChoice == null) return "Initialising";

        if ( _tables.problem.Reverse )
        {
            return ToStringReverse();
        }
    //    {
    //    StringBuilder sb1 = new StringBuilder();
    //    {
    //        // handle case of peak at composition before tables initialiased
    //        short thisLead = leads[0]; // rounds
    //        for (int i = 0; i <= maxLeadIndex; ++i)
    //        {
    //            int leadIndex = (i + rot) % (maxLeadIndex + 1);

    //            int choice = choices[leadIndex];
    //            sb1.Append(choice.ToString());
    //        }
    //    }
    //    return sb1.ToString();
    //}

        StringBuilder sb = new StringBuilder();
        {
            // handle case of peak at composition before tables initialiased
            short thisLead = leads[0]; // rounds
            for (int i = 0; i <= maxLeadIndex; ++i)
            {
                int leadIndex = (i + rot) % (maxLeadIndex + 1);

                int choice = choices[leadIndex];


                // handle snapshots of in progress touches
                if (choice >= _tables._methodsByChoice.Length) break;

                //sb.Append(choice.ToString());


                Method method = _tables._methodsByChoice[choice];
                int methodIndex = _tables.methodIndexByChoice[choice];
                short nextLead = _tables.leadMapping[thisLead, choice];
                if (nextLead >= 0)
                {
                    Row r = Row.FromNumber(nextLead);
                    //sb.Append(methodIndex);
                    //sb.Append(method.Letter);
                    int call = _tables.CallIndex(choice);

                    //sb.Append(call > 0 ? "B" : "P");
                    if (call > 0)
                    {
                        char callName = "?IB4VMWH"[r.ToString().IndexOf('8')];
                        //if (sb.Length > 0)
                        //{
                        //  sb.Append(" ");
                        //}
                        if (call == 2)
                        {
                            sb.Append("s");
                        }
                        //else
                        //{
                        //  sb.Append("-");
                        //}
                        sb.Append(callName);
                    }
                }
                else
                {
                    sb.Append(".");
                    break;
                }
                thisLead = nextLead;
            }
        }
        sb.Append(" ");
        while (sb.Length < 6) sb.Append(" ");
        {
            short thisLead = leads[0]; // rounds
            for (int i = 0; i <= maxLeadIndex; ++i)
            {
                int leadIndex = (i + rot) % (maxLeadIndex + 1);
                int choice = choices[leadIndex];

                // handle snapshots of in progress touches
                if (choice >= _tables._methodsByChoice.Length) break;

                Method method = _tables._methodsByChoice[choice];
                int methodIndex = _tables.methodIndexByChoice[choice];
                short nextLead = _tables.leadMapping[thisLead, choice];
                // handle snapshots of in progress touches
                if (nextLead < 0) break;
                Row r = Row.FromNumber(nextLead);
                //sb.Append(methodIndex);
                sb.Append(method.Letter);
                int call = _tables.CallIndex(choice);

                //sb.Append(call > 0 ? "B" : "P");
                if (call > 0)
                {
                    //char callName = "?IB4VMWH"[r.ToString().IndexOf('8')];
                    //if (sb.Length > 0)
                    //{
                    //  sb.Append(" ");
                    //}
                    if (call == 2)
                    {
                        sb.Append("s");
                    }
                    else
                    {
                        sb.Append("-");
                    }
                    //sb.Append(callName);
                }
                thisLead = nextLead;
            }
        }
        return sb.ToString();
    }
    public  string ToStringReverse()
    {
        if (rot != 0) throw new NotSupportedException();
       
        StringBuilder sb = new StringBuilder();
        try
        {
            {
                // handle case of peak at composition before tables initialiased
                //short thisLead = leads[maxLeadIndex]; // rounds
                for (int i = maxLeadIndex; i >= 0; --i)
                {
                    int leadIndex = (i + rot) % (maxLeadIndex + 1);

                    int choice = choices[leadIndex];
                    int lead = leads[leadIndex];

                    Method method = _tables._methodsByChoice[choice];
                    int methodIndex = _tables.methodIndexByChoice[choice];
                    //short nextLead = _tables.reverseLeadMapping[thisLead, choice];

                    Lead l = method.Lead(lead);

                    // handle snapshots of in progress touches
                    if (choice >= _tables._methodsByChoice.Length) break;

                    //sb.Append(choice.ToString());

                    var thisLead = lead;

                    if (true)
                    //|| nextLead >= 0)
                    {
                        Row r = Row.FromNumber(thisLead);
                        //var lead = method.Lead(thisLead);
                        //var firstRow = lead.RowsAsInts.First();
                        //Row fr = Row.FromNumber(firstRow);
                        //sb.Append(methodIndex);
                        //sb.Append(method.Letter);
                        int call = _tables.CallIndex(choice);

                        //sb.Append(call > 0 ? "B" : "P");
                        if (call > 0)
                        {
                            char callName = "?IB4VMWH"[
                                  Row.FromNumber(l.RowsAsInts.Last()).ToString().IndexOf('8')];
                            //if (sb.Length > 0)
                            //{
                            //  sb.Append(" ");
                            //}
                            if (call == 2)
                            {
                                sb.Append("s");
                            }
                            //else
                            //{
                            //  sb.Append("-");
                            //}
                            sb.Append(callName);
                        }
                    }
                    else
                    {
                        sb.Append(".");
                        break;
                    }
                    //thisLead = nextLead;
                    //if ( thisLead < 0 )
                    //{
                    //    break;
                    //}
                }
            }
            sb.Append(" ");
            while (sb.Length < 6) sb.Append(" ");
            {
                short thisLead = leads[maxLeadIndex]; // rounds
                for (int i = maxLeadIndex; i >= 0; --i)
                {
                    int leadIndex = (i + rot) % (maxLeadIndex + 1);
                    int choice = choices[leadIndex];

                    // handle snapshots of in progress touches
                    if (choice >= _tables._methodsByChoice.Length) break;

                    Method method = _tables._methodsByChoice[choice];
                    int methodIndex = _tables.methodIndexByChoice[choice];

                    //sb.Append(methodIndex);
                    int call = _tables.CallIndex(choice);

                    //sb.Append(call > 0 ? "B" : "P");
                    if (call > 0)
                    {
                        //char callName = "?IB4VMWH"[r.ToString().IndexOf('8')];
                        //if (sb.Length > 0)
                        //{
                        //  sb.Append(" ");
                        //}
                        if (call == 2)
                        {
                            sb.Append("s");
                        }
                        else
                        {
                            sb.Append("-");
                        }
                        //sb.Append(callName);
                    }
                    if (method.Name != "Null")
                    {
                        sb.Append(method.Letter);
                    }
                }
            }
        }
        catch ( Exception  )
        {
            // can happen for in progress snapshots

        }
        return sb.ToString();
    }

    public int Calls
    {
      get
      {
        int calls = 0;
        for (int i = 0; i <= this.maxLeadIndex; ++i)
        {
          if ( _tables.IsCall(choices[i]) ) ++calls;

          //var grp = choices[i] % 3;
          //if (grp == 1) ++calls;
          //if (grp == 2) calls += 1000;
        }
        return calls;
      }
    }

    public int Changes
    {
      get
      {
        int length = 0;

        foreach (KeyValuePair<Int16, int> lead in LeadHeadsAndChoices)
        {
          Lead leadObject = _tables._methodsByChoice[lead.Value].Lead(lead.Key);
          if (leadObject.ContainsRoundsAt > 0)
          {
            length += leadObject.ContainsRoundsAt;
          }
          else
          {
            length += leadObject.Length;
          }
        }
        return length;
      }
    }

    public int Score
    {
      get
      {
        //return Music;
        //return Music - Calls;
        return Music; // CalcWraps();
        //CalcWraps();// 100 - Calls;
      }
    }

    public int _music;
    public int _calls;
    public int _com;
    public int _quality;
    public int _changes;
    public int _centreOfMusic;

      public void CalcStats()
    {
        _music = 0;
        _calls = 0;
        _com = 0;
        _changes = 0;
        _centreOfMusic = 0;
        float q = 0;
        int _centreOfMusicDenominator = 0;

          var musicTable = _tables.music;
          var leadMapping = _tables.leadMapping;
          var choices = this.choices;

        int leadIndex = rot;
        Int16 lead = new Row(8).ToNumberExTreble();
          float w = 1;
        for (int i = 0; i <= this.maxLeadIndex; ++i)
        {
            w= w * 0.98f;
            var thisMusic = musicTable[lead, choices[leadIndex]];
            _music += thisMusic;
            q += thisMusic * w; 
            _centreOfMusic += musicTable[lead, choices[leadIndex]] * i;
            _centreOfMusicDenominator += i;
            lead = leadMapping[lead, choices[leadIndex]];

            // COM1
            int methodIndex = _tables.methodIndexByChoice[choices[leadIndex]];

            //move on
            leadIndex++;
            if (leadIndex > maxLeadIndex) leadIndex = 0;

            // calls
            if (_tables.IsCall(choices[i])) ++_calls;           

            // COM2
            int nextMethodIndex = _tables.methodIndexByChoice[choices[leadIndex]];
            if (methodIndex != nextMethodIndex) ++_com;
        }
        var denom = _music*maxLeadIndex;
        if (denom == 0) denom = 1;
        _centreOfMusic = (100 * _centreOfMusic) / denom;
        //if (_calls > 0)
        //{
        //    _quality = _music / _calls;
        //}
          // reverse composition so earlier is better
        //_quality = 100 - _centreOfMusic;
        _quality = (int)(100 * q);
    }
    public int Music
    {
      get
      {
          EnsureStats();
        return _music;
      }
    }

    public int Imbalance
    {
        get
        {
            Dictionary<Method, int> counts = new Dictionary<Method, int>();
            int ret = 0;
            int leadIndex = rot;
            Int16 lead = new Row(8).ToNumberExTreble();
            foreach ( var method in _tables._methodsByChoice)
            {
                if (method.Name != "Null")
                {
                    counts[method] = 0;
                }
            }
            for (int i = 0; i <= this.maxLeadIndex; ++i)
            {
               
                var method = _tables._methodsByChoice[choices[leadIndex]];
                if (method.Name != "Null")
                {
                        ++counts[method];
                    
                   
                    int imBAl = counts.Values.Max() - counts.Values.Min();
                    if (imBAl > ret)
                    {
                        ret = imBAl;
                    }
                }
                leadIndex++;
                if (leadIndex > maxLeadIndex) leadIndex = 0;
            }
            return ret;
        }
    }

      public int CalcQuality(int music)
    {
        return music * 1000 / (1 + Calls);
    }
    public int Quality
    {
      get
      {
          EnsureStats();
          return _quality;
         // return Score * 1000 / Changes;
         // return CalcQuality(Music);
      }
    }

    bool doneStats;
      void EnsureStats()
    {
          if ( !doneStats )
          {
              CalcStats();
              doneStats = true;
          }
    }

    public int COM
    {
      get
      {
          EnsureStats();
          return _com; 
      }
    }

    public IEnumerable<KeyValuePair<Int16, int>> LeadHeadsAndChoices
    {
      get
      {
        int leadIndex = rot;
        Int16 lead = new Row(8).ToNumberExTreble();
        for (int i = 0; i <= this.maxLeadIndex; ++i)
        {
          yield return new KeyValuePair<Int16, int>(lead, choices[leadIndex]);
          lead = _tables.leadMapping[lead, choices[leadIndex]];
          leadIndex++;
          if (leadIndex > maxLeadIndex) leadIndex = 0;
        }

      }
    }

    public IEnumerable<Row> Rows
    {
      get
      {
          if ( _tables.problem.Reverse )
          {
              foreach (KeyValuePair<Int16, int> lead in LeadHeadsAndChoices.Reverse())
              {
                  foreach (Row r in _tables.Rows(lead.Key, lead.Value).Reverse())
                  {
                      yield return r;
                  }
              }
          }
          else
          {

        foreach (KeyValuePair<Int16, int> lead in LeadHeadsAndChoices )
        {
          foreach (Row r in _tables.Rows(lead.Key,lead.Value))
          {
            yield return r;
          }
        }
          }
      }
    }

    public IEnumerable<KeyValuePair<Row, string>> GetMusic(MusicalPreferences preferences)
    {
      int i = 0;
      foreach (Row r in Rows)
      {
        i++;
        foreach (string s in preferences.EnumerateMusic(r))
        {
          yield return new KeyValuePair<Row, string>(r, s);
        }
      }
      Console.WriteLine(i);
    }

    public IEnumerable<KeyValuePair<Row, IMusicalChange>> GetMusicalChanges(MusicalPreferences preferences)
    {
        foreach (Row r in Rows)
        {
            foreach (var musicalChange in preferences.EnumerateMusicalChanges(r))
            {
                yield return new KeyValuePair<Row, IMusicalChange>(r, musicalChange);
            }
        }
    }
    public TimeSpan TimeToFind
    {
      get
      {
        return _timeToFind;
      }
      set
      {
        _timeToFind = value;
      }
    }
  }
}
