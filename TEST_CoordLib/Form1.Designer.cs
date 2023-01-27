
namespace TEST_CoordLib
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing )
    {
      if (disposing && (components != null)) {
        components.Dispose( );
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent( )
    {
      this.RTB = new System.Windows.Forms.RichTextBox();
      this.btLookup1 = new System.Windows.Forms.Button();
      this.btLookup2 = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.btMagVar = new System.Windows.Forms.Button();
      this.txMVbearing = new System.Windows.Forms.TextBox();
      this.lblMVresult = new System.Windows.Forms.Label();
      this.cbxUseTable = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // RTB
      // 
      this.RTB.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RTB.Location = new System.Drawing.Point(12, 102);
      this.RTB.Name = "RTB";
      this.RTB.Size = new System.Drawing.Size(357, 665);
      this.RTB.TabIndex = 0;
      this.RTB.Text = "";
      // 
      // btLookup1
      // 
      this.btLookup1.Location = new System.Drawing.Point(12, 12);
      this.btLookup1.Name = "btLookup1";
      this.btLookup1.Size = new System.Drawing.Size(101, 46);
      this.btLookup1.TabIndex = 1;
      this.btLookup1.Text = "Load";
      this.btLookup1.UseVisualStyleBackColor = true;
      this.btLookup1.Click += new System.EventHandler(this.btLookup1_Click);
      // 
      // btLookup2
      // 
      this.btLookup2.Location = new System.Drawing.Point(148, 12);
      this.btLookup2.Name = "btLookup2";
      this.btLookup2.Size = new System.Drawing.Size(101, 46);
      this.btLookup2.TabIndex = 2;
      this.btLookup2.Text = "QLookup 2";
      this.btLookup2.UseVisualStyleBackColor = true;
      this.btLookup2.Click += new System.EventHandler(this.btLookup2_Click);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(268, 12);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(101, 46);
      this.button1.TabIndex = 3;
      this.button1.Text = "TEST";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // btMagVar
      // 
      this.btMagVar.Location = new System.Drawing.Point(446, 137);
      this.btMagVar.Name = "btMagVar";
      this.btMagVar.Size = new System.Drawing.Size(85, 32);
      this.btMagVar.TabIndex = 4;
      this.btMagVar.Text = "Calc MagVar";
      this.btMagVar.UseVisualStyleBackColor = true;
      this.btMagVar.Click += new System.EventHandler(this.btMagVar_Click);
      // 
      // txMVbearing
      // 
      this.txMVbearing.Location = new System.Drawing.Point(446, 90);
      this.txMVbearing.Name = "txMVbearing";
      this.txMVbearing.Size = new System.Drawing.Size(106, 20);
      this.txMVbearing.TabIndex = 5;
      this.txMVbearing.Text = "180";
      // 
      // lblMVresult
      // 
      this.lblMVresult.AutoSize = true;
      this.lblMVresult.Location = new System.Drawing.Point(452, 182);
      this.lblMVresult.Name = "lblMVresult";
      this.lblMVresult.Size = new System.Drawing.Size(35, 13);
      this.lblMVresult.TabIndex = 6;
      this.lblMVresult.Text = "label1";
      // 
      // cbxUseTable
      // 
      this.cbxUseTable.AutoSize = true;
      this.cbxUseTable.Location = new System.Drawing.Point(446, 116);
      this.cbxUseTable.Name = "cbxUseTable";
      this.cbxUseTable.Size = new System.Drawing.Size(98, 17);
      this.cbxUseTable.TabIndex = 7;
      this.cbxUseTable.Text = "Use UTM table";
      this.cbxUseTable.UseVisualStyleBackColor = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(649, 779);
      this.Controls.Add(this.cbxUseTable);
      this.Controls.Add(this.lblMVresult);
      this.Controls.Add(this.txMVbearing);
      this.Controls.Add(this.btMagVar);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.btLookup2);
      this.Controls.Add(this.btLookup1);
      this.Controls.Add(this.RTB);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.RichTextBox RTB;
    private System.Windows.Forms.Button btLookup1;
    private System.Windows.Forms.Button btLookup2;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button btMagVar;
    private System.Windows.Forms.TextBox txMVbearing;
    private System.Windows.Forms.Label lblMVresult;
    private System.Windows.Forms.CheckBox cbxUseTable;
  }
}

