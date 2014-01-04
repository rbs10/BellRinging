namespace GUI
{
  partial class LeadView
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.leadViewControl1 = new GUI.LeadViewControl();
      this.button1 = new System.Windows.Forms.Button();
      this.methodSelectorComboBox = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // leadViewControl1
      // 
      this.leadViewControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.leadViewControl1.Location = new System.Drawing.Point(0, 0);
      this.leadViewControl1.Name = "leadViewControl1";
      this.leadViewControl1.Size = new System.Drawing.Size(1083, 664);
      this.leadViewControl1.TabIndex = 0;
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.Location = new System.Drawing.Point(1007, 13);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 1;
      this.button1.Text = "Show Music";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // methodSelectorComboBox
      // 
      this.methodSelectorComboBox.FormattingEnabled = true;
      this.methodSelectorComboBox.Items.AddRange(new object[] {
            "Cambridge",
            "Superlative",
            "Lincolnshire",
            "Yorkshire",
            "Pudsey",
            "Bristol",
            "Rutland",
            "London",
            "Belfast",
            "Glasgow",
            "Lessness"});
      this.methodSelectorComboBox.Location = new System.Drawing.Point(920, 62);
      this.methodSelectorComboBox.Name = "methodSelectorComboBox";
      this.methodSelectorComboBox.Size = new System.Drawing.Size(121, 21);
      this.methodSelectorComboBox.TabIndex = 2;
      this.methodSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.methodSelectorComboBox_SelectedIndexChanged);
      // 
      // LeadView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1083, 664);
      this.Controls.Add(this.methodSelectorComboBox);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.leadViewControl1);
      this.Name = "LeadView";
      this.Text = "LeadView";
      this.ResumeLayout(false);

    }

    #endregion

    private LeadViewControl leadViewControl1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.ComboBox methodSelectorComboBox;
  }
}