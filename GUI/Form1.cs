using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BellRinging;
using System.Linq;
using System.IO;

namespace GUI
{
  public partial class Form1 : Form, ICompositionReceiver
  {
    ListViewColumnSorter lvwColumnSorter;
    MusicalPreferences _musicalPreferences = new MusicalPreferences();
    string method;


   //SimpleComposer_2015v4_ForwardBlocks _composer = new SimpleComposer_2015v4_ForwardBlocks();
   SimpleComposer_2016_2015_SingleMethodReverse _composer = new SimpleComposer_2016_2015_SingleMethodReverse();

    public Form1(string method)
    {
        this.method = method;
      InitializeComponent();
      lvwColumnSorter = new ListViewColumnSorter();
      this.listView1.ListViewItemSorter = lvwColumnSorter;
      _musicalPreferences.Init();


      _composer.Receiver = this;
      _composer.StartCompose(() => _composer.Initialise(method));

        
    }


    //void InitComposer(string method)
    //{
    //   // _composer = new SimpleComposer();
    //    //_composer.Initialise("X18X18X18x18-12"); // PLAIN BOB?
    //     // _composer.Initialise("Glasgow","36X56.14.58X58.36X14X38.16X16.38-18"); // glasgow
    //   // _composer.Initialise("Lessness","X38X14X56X16X12X58X14X58-12"); // lessness
    //   //_composer.Initialise("X58X14.58X58.36.14X14.58X14X18-18"); // bristol
    //    //_composer.Initialise("Yorkshire"); // yorkshire
    //   // _composer.InitialiseWithSnapStart("Yorkshire"); // yorkshire
    //   // _composer.InitialiseWithSnapStart(method); 
    //   //_composer.Initialise(method);
    //    //_composer.InitialiseStandardMethods();
    //    // _composer.Initialise("X38X14X58X16X14X38X34X18-12"); // rutland
    //   //_composer.Initialise("Cambridge","X38X14X1258X36X14X58X16X78-12");// cambridge
    //    // _composer.Initialise("X36X14X58X36X14X58X36X78-12"); // superlative

    //    //_composer.Initialise("36X56.14.58X58.36X14X38.16X16.38-18", // glasgow
    //    //  "X38X14X58X16X12X38X14X78-12"); // yorkshire

    //   ;

    //    //_composer.Initialise("Dover");
    //    //_composer.Initialise("Venusium");

    //    //_composer.Initialise("Cambridge", "C", "X38X14X1258X36X14X58X16X78-12", "Bastow", "B", "X12-12");
    ////    _composer.Initialise("Yorkshire", "Y", "X38X14X58X16X12X38X14X78-12", "Bastow", "B", "X12-12");
    // //   _composer.Initialise("Ely", "E", "X38X14X56X16.34X14.58.14X14.58-12", "Bastow", "B", "X12-12");
    // //   _composer.Initialise("Bristol", "B", "X58X14.58X58.36.14X14.58X14X18-18", "Bastow", "b", "X12-12");
    //  //  _composer.Initialise("Little Bob", "X18X14-12");
      
    //}

    private void goButton_Click(object sender, EventArgs e)
    {

      //  _composer = new SimpleComposer_LPS_QP_2015();
      //_composer.StartCompose( () => InitComposer() );
    }

    #region ICompositionReceiver Members

    delegate void AddCompositionDelegate(Composition c);

    public void AddComposition(Composition c)
    {
      try
      {
        if (InvokeRequired)
        {
          Invoke(new AddCompositionDelegate(this.AddComposition), c);
        }
        else
        {
          ListViewItem i = new ListViewItem();
          i.Tag = c;
          i.SubItems[0] = new ListViewItem.ListViewSubItem(i, c.Changes.ToString());
          i.SubItems.Add( new ListViewItem.ListViewSubItem(i, c.Calls.ToString()));
           i.SubItems.Add( new ListViewItem.ListViewSubItem(i, c.Music.ToString()));
            i.SubItems.Add(  new ListViewItem.ListViewSubItem(i, c.COM.ToString()));
         i.SubItems.Add( new ListViewItem.ListViewSubItem(i, c.Score.ToString()));
         i.SubItems.Add(new ListViewItem.ListViewSubItem(i, c.Quality.ToString()));
         i.SubItems.Add(new ListViewItem.ListViewSubItem(i, c.TimeToFind.TotalMinutes.ToString()));
         i.SubItems.Add(new ListViewItem.ListViewSubItem(i, c.ToString()));
          //i.Text = (c.maxLeadIndex + 1).ToString();
          listView1.Items.Add(i);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }


    #endregion

    Composition _selectedComposition;

    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {
      string text = "";
      foreach (ListViewItem item in listView1.SelectedItems)
      {
          _selectedComposition = item.Tag as Composition;
          if (leadView != null)
          {
              leadView.Composition = _selectedComposition;
          }
          text = _selectedComposition.WriteDetails();
      }
      this.compositionTextBox.Text = text;
    }

    private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
    {

      // Determine if clicked column is already the column that is being sorted.
      if (e.Column == lvwColumnSorter.SortColumn)
      {
        // Reverse the current sort direction for this column.
        if (lvwColumnSorter.Order == SortOrder.Ascending)
        {
          lvwColumnSorter.Order = SortOrder.Descending;
        }
        else
        {
          lvwColumnSorter.Order = SortOrder.Ascending;
        }
      }
      else
      {
        // Set the column number that is to be sorted; default to ascending.
        lvwColumnSorter.SortColumn = e.Column;
        lvwColumnSorter.Order = SortOrder.Ascending;
      }

      // Perform the sort with these new sort options.
      this.listView1.Sort();
    }

    long lastLeadCount = 0;

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (_composer != null)
      {
        //long count = _composer.TotalLeads;
        //leadsCheckedTextBox.Text = _composer.TotalLeads.ToString();
        //leadsPerSecondTextBox.Text = (count - lastLeadCount).ToString(); // c.LeadsPerSecond.ToString();
        //lastLeadCount = count;
        //minBackTrackTextBox.Text = _composer.MinBackTrackDescription;

          minBackTrackTextBox.Text = _composer.DescribeState();
      }
    }

    LeadView leadView;

    static SaveFileDialog sfd;
    private void button1_Click(object sender, EventArgs e)
    {
     try
     {
         if ( sfd == null )
         {
             sfd = new SaveFileDialog();
             sfd.DefaultExt = ".txt";
         }
         if ( sfd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK )
         {
             using ( var sw = new StreamWriter(sfd.FileName) )
             {
                 sw.WriteLine("{0} changes", _selectedComposition.Changes);
                 sw.WriteLine("{0} music", _selectedComposition.Music);
                 sw.WriteLine();
                 sw.WriteLine(compositionTextBox.Text);
             }
         }
     }
        catch ( Exception ex )
     {
         MessageBox.Show(ex.Message);
     }
    }
  }
}