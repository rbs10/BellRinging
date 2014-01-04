using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BellRinging;

namespace GUI
{
  public partial class LeadView : Form
  {
    public LeadView()
    {
      InitializeComponent();
    }

    public SimpleComposer Composer
    {
      set
      {
        this.leadViewControl1.Composer = value;
      }
    }

    public Composition Composition
    {
      set
      {
        this.leadViewControl1.Composition = value;
      }
    }
    private void button1_Click(object sender, EventArgs e)
    {
      leadViewControl1.ShowMusic();
    }

    private void methodSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      string method = methodSelectorComboBox.SelectedItem.ToString();

      SimpleComposer c = new SimpleComposer();
      c.Initialise(method,new MethodLibrary().GetNotation(method));
      Composer = c;
      leadViewControl1.ShowMusic();
    }
  }
}