using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BellRinging;

namespace GUI
{
  public partial class LeadViewControl : UserControl
  {
    Dictionary<int, Control> leadToLeadControl = new Dictionary<int, Control>();
    Dictionary<Control, int> leadControlToLead = new Dictionary<Control, int>();
    public LeadViewControl()
    {
      InitializeComponent();

      int rows = 60;
      int cols = 5040 / rows;
      int colSpacing = Width / (cols+11); // 84 columns - split into groups of 7 - 12 columns of said
      int rowSpacing = Height / rows;
      int i = 0;
      Row rounds = new Row(8);
      //int[] leadEndOrder = new int[] { 8, 5, 2, 6, 7, 3, 4 };
      int[] leadEndOrder = new int[] { 8, 7, 5, 3, 2, 4, 6 };
      // 1xxxxx78 1xxxx7x8 ... - 6 sets of 120
      for (int pos7rel8 = 0; pos7rel8 < 6; ++pos7rel8)
      {
        for (int p6 = 0; p6 < 5; ++p6)
        {
          for (int p5 = 0; p5 < 4; ++p5)
          {
            for (int p4 = 0; p4 < 3; ++p4)
            {
              for (int p3 = 0; p3 < 2; ++p3)
              {
                // no freedom left for two
                // rotations for a lead
                for (int lead = 0; lead < 7; ++lead)
                {
                  int pos8 = leadEndOrder[lead];
                  int[] mapping = new int[8];
                  char[] rowChar = new char[8];
                  rowChar[0] = '1';

                  // treble maps to 0
                  for (int j = 1; j < 8; ++j) { mapping[j] = -1; }
                  mapping[pos8 - 1] = 8 - 1; // tenor position

                  Behind(mapping,8, 7, pos7rel8);
                  Ahead(mapping, 8, 6, p6);
                  Behind(mapping, 7, 5, p5);
                  Ahead(mapping, 6, 4, p4);
                  Behind(mapping, 5, 3, p3);
                  Behind(mapping, 3, 2, 0);

                  Row r = rounds.Apply(mapping);

                  //for (int i = 0; i < 5040; ++i)
                  {
                    CheckBox c = new CheckBox();
                    c.CheckedChanged += new EventHandler(c_CheckedChanged);
                    c.FlatStyle = FlatStyle.Flat;
                    int col = i % cols;
                    int row = i / cols + pos7rel8;
                    c.Left = (col + (col / 7)) * colSpacing;
                    c.Top = row * rowSpacing;
                    c.Width = colSpacing;
                    c.Height = rowSpacing;
                    Controls.Add(c);
                    int num = r.ToNumber();
                    leadToLeadControl.Add(num, c);
                    leadControlToLead.Add(c, num);
                    toolTip1.SetToolTip(c, r.ToString());
                  }

                  ++i;
                }
              }
            }
          }
        }
     
      }
    }

    int[] aheadPlaces = new int[] { -1, 3, 5, 2, 7, 4, 8, 6 };
    int[] behindPlaces = new int[] { -1, 4, 2, 6, 3, 8, 5, 7 };

    private void Ahead(int[] mapping, int refBell, int bellToPlace, int relPlaces)
    {
      RelativeMapping(mapping, refBell, bellToPlace, relPlaces, aheadPlaces);
    }

    private void Behind(int[] mapping, int refBell, int bellToPlace, int relPlaces)
    {
      RelativeMapping(mapping, refBell, bellToPlace, relPlaces, behindPlaces);
    }

    private void RelativeMapping(int[] mapping, int refBell, int bellToPlace, int relPlaces, int [] relMapping)
    {
      int placeOfRefBell = -1;
      for (int i = 0; i < mapping.Length; ++i)
      {
        if (mapping[i] == refBell - 1)
        {
          placeOfRefBell = i;
          break;
        }
      }
      int place = relMapping[placeOfRefBell] - 1;
      while ( relPlaces > 0  )
      {
        while (mapping[place] >= 0) { place = relMapping[place] - 1; };
        place = relMapping[place] - 1;
        --relPlaces;
      }
      while (mapping[place] >= 0) { place = relMapping[place] - 1; };

      // mapping is the slot to fill the position from when generating a new row
      mapping[place] = bellToPlace - 1;
    }


    void c_CheckedChanged(object sender, EventArgs e)
    {
      CheckBox c = sender as CheckBox;
      if (c.Checked)
      {
        int lead = leadControlToLead[c];
        foreach (short falseLead in _tables.falseLeadHeads[0, 0, lead])
        {
          if (falseLead > 0)
          {
            leadToLeadControl[falseLead].Enabled = false;
          }
        }
      }
    }

    Tables _tables;
    Composition _composition;
    List<Control> _compositionControls = new List<Control>();
    int _compositionControlsIndex = 0;

    public SimpleComposer Composer
    {
      set
      {
       _tables = value.Tables;
       foreach (Lead l in _tables._methodsByChoice[0].AllLeads)
        {
          Control c = leadToLeadControl[l.ToNumber()];
          switch (l.CalcWraps())
          {
            case 0: 
              break;
            case 1: 
              c.BackColor = Color.Red; 
              break;
            default:
              throw new Exception("Too many wraps!");

          }
        }

      }
    }

    public Composition Composition
    {
      set
      {
        _compositionControls.Clear();
        _composition = value;
        foreach (CheckBox c in leadToLeadControl.Values)
        {
          c.Checked = false;
          c.Enabled = true;
          c.ForeColor = Color.Black;
        }
        foreach (KeyValuePair<Int16,int> lh in _composition.LeadHeadsAndChoices)
        {
          Control ctrl = leadToLeadControl[(Int16)(lh.Key)];
          _compositionControls.Add(ctrl);
          (ctrl as CheckBox).Checked = true;
        }
      }
    }


    private void timer1_Tick(object sender, EventArgs e)
    {
      if (_compositionControls.Count > 0)
      {
        _compositionControlsIndex++;
        _compositionControlsIndex = _compositionControlsIndex % _compositionControls.Count;
        Control ctrl = _compositionControls[_compositionControlsIndex];
        ctrl.ForeColor = ctrl.ForeColor == Color.White ? Color.Black : Color.White;
      }

    }

    public void ShowMusic()
    {
      int maxMusic = 0;
      foreach (KeyValuePair<int, Control> kvp in leadToLeadControl)
      {
        int lead = (int) kvp.Key;
        int music = _tables.music[lead, 0];
        maxMusic = Math.Max(maxMusic,music);
      }
      foreach (KeyValuePair<int, Control> kvp in leadToLeadControl)
      {
        int lead = (int) kvp.Key;
        int music = _tables.music[lead, 0];
        kvp.Value.BackColor = Mix(Color.Red, Color.Blue, music, maxMusic);
      }
    }

    private Color Mix(Color color, Color color_2, float val, float maxVal)
    {
      return Color.FromArgb(
        Mix(color.R, color_2.R, val, maxVal),
        Mix(color.G, color_2.G, val, maxVal),
        Mix(color.B, color_2.B, val, maxVal));
    }

     private int Mix(int i1, int i2, float val, float maxVal)
    {
       return i2 + (int) Math.Round((i1-i2) * val/maxVal,0);
    }

  }
}
