using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
    int rot;

    Tables _tables;

    TimeSpan _timeToFind;


    int[] blankRowCount;// = new int[40320];
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

      blankRowCount = new int[40320];
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
        int method1 = _tables.methodIndexByChoice[choices[i]];
        Method m = _tables._methodsByChoice[choices[i]];
        Lead l = m.Lead(leadHead1);
        foreach (int r in l.RowsAsInts)
        {
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
      StringBuilder sb = new StringBuilder();
      {
        short thisLead = leads[0]; // rounds
        for (int i = 0; i <= maxLeadIndex; ++i)
        {
          int leadIndex = (i + rot) % (maxLeadIndex + 1);
          int choice = choices[leadIndex];
          Method method = _tables._methodsByChoice[choice];
          int methodIndex = _tables.methodIndexByChoice[choice];
          short nextLead = _tables.leadMapping[thisLead, choice];
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
          Method method = _tables._methodsByChoice[choice];
          int methodIndex = _tables.methodIndexByChoice[choice];
          short nextLead = _tables.leadMapping[thisLead, choice];
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

    public int Calls
    {
      get
      {
        int calls = 0;
        for (int i = 0; i <= this.maxLeadIndex; ++i)
        {
          if ( _tables.IsCall(choices[i]) ) ++calls;
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

    public int Music
    {
      get
      {
        int music = 0;
        foreach (KeyValuePair<Int16, int> lead in LeadHeadsAndChoices)
        {
          music += _tables.music[lead.Key,lead.Value];
        }
        return music;
      }
    }

    public int Quality
    {
      get
      {
         // return Score * 1000 / Changes;
          return Music * 1000 / (1+ Calls);
      }
    }

    public int COM
    {
      get
      {
        int COM = 0;
        int lead = rot;
        for (int i = 0; i < this.maxLeadIndex; ++i)
        {
          int methodIndex = _tables.methodIndexByChoice[choices[lead]];
          lead++;
          if (lead > maxLeadIndex) lead = 0;
          int nextMethodIndex = _tables.methodIndexByChoice[choices[lead]];
          if (methodIndex != nextMethodIndex) ++COM;
        }
        return COM;
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
        foreach (KeyValuePair<Int16, int> lead in LeadHeadsAndChoices )
        {
          foreach (Row r in _tables.Rows(lead.Key,lead.Value))
          {
            yield return r;
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
