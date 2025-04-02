
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
      this.btQuadList = new System.Windows.Forms.Button();
      this.btDistTable = new System.Windows.Forms.Button();
      this.txLat = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txLon = new System.Windows.Forms.TextBox();
      this.txMv = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.btCalcMvFromLatLon = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // RTB
      // 
      this.RTB.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RTB.Location = new System.Drawing.Point(12, 215);
      this.RTB.Name = "RTB";
      this.RTB.Size = new System.Drawing.Size(604, 496);
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
      this.button1.Text = "TEST equality";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // btMagVar
      // 
      this.btMagVar.Location = new System.Drawing.Point(418, 59);
      this.btMagVar.Name = "btMagVar";
      this.btMagVar.Size = new System.Drawing.Size(85, 32);
      this.btMagVar.TabIndex = 4;
      this.btMagVar.Text = "Calc MagVar";
      this.btMagVar.UseVisualStyleBackColor = true;
      this.btMagVar.Click += new System.EventHandler(this.btMagVar_Click);
      // 
      // txMVbearing
      // 
      this.txMVbearing.Location = new System.Drawing.Point(418, 12);
      this.txMVbearing.Name = "txMVbearing";
      this.txMVbearing.Size = new System.Drawing.Size(106, 20);
      this.txMVbearing.TabIndex = 5;
      this.txMVbearing.Text = "180";
      // 
      // lblMVresult
      // 
      this.lblMVresult.AutoSize = true;
      this.lblMVresult.Location = new System.Drawing.Point(424, 104);
      this.lblMVresult.Name = "lblMVresult";
      this.lblMVresult.Size = new System.Drawing.Size(35, 13);
      this.lblMVresult.TabIndex = 6;
      this.lblMVresult.Text = "label1";
      // 
      // cbxUseTable
      // 
      this.cbxUseTable.AutoSize = true;
      this.cbxUseTable.Location = new System.Drawing.Point(418, 38);
      this.cbxUseTable.Name = "cbxUseTable";
      this.cbxUseTable.Size = new System.Drawing.Size(98, 17);
      this.cbxUseTable.TabIndex = 7;
      this.cbxUseTable.Text = "Use UTM table";
      this.cbxUseTable.UseVisualStyleBackColor = true;
      // 
      // btQuadList
      // 
      this.btQuadList.Location = new System.Drawing.Point(268, 84);
      this.btQuadList.Name = "btQuadList";
      this.btQuadList.Size = new System.Drawing.Size(100, 32);
      this.btQuadList.TabIndex = 8;
      this.btQuadList.Text = "LatLon-Quad";
      this.btQuadList.UseVisualStyleBackColor = true;
      this.btQuadList.Click += new System.EventHandler(this.btQuadList_Click);
      // 
      // btDistTable
      // 
      this.btDistTable.Location = new System.Drawing.Point(544, 59);
      this.btDistTable.Name = "btDistTable";
      this.btDistTable.Size = new System.Drawing.Size(93, 39);
      this.btDistTable.TabIndex = 9;
      this.btDistTable.Text = "DistTable";
      this.btDistTable.UseVisualStyleBackColor = true;
      this.btDistTable.Click += new System.EventHandler(this.btDistTable_Click);
      // 
      // txLat
      // 
      this.txLat.Location = new System.Drawing.Point(45, 126);
      this.txLat.Name = "txLat";
      this.txLat.Size = new System.Drawing.Size(106, 20);
      this.txLat.TabIndex = 10;
      this.txLat.Text = "180";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 129);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(27, 13);
      this.label1.TabIndex = 11;
      this.label1.Text = "LAT";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 156);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(29, 13);
      this.label2.TabIndex = 13;
      this.label2.Text = "LON";
      // 
      // txLon
      // 
      this.txLon.Location = new System.Drawing.Point(45, 153);
      this.txLon.Name = "txLon";
      this.txLon.Size = new System.Drawing.Size(106, 20);
      this.txLon.TabIndex = 12;
      this.txLon.Text = "180";
      // 
      // txMv
      // 
      this.txMv.Location = new System.Drawing.Point(45, 179);
      this.txMv.Name = "txMv";
      this.txMv.Size = new System.Drawing.Size(106, 20);
      this.txMv.TabIndex = 14;
      this.txMv.Text = "180";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 182);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(27, 13);
      this.label3.TabIndex = 15;
      this.label3.Text = "MV°";
      // 
      // btCalcMvFromLatLon
      // 
      this.btCalcMvFromLatLon.Location = new System.Drawing.Point(157, 167);
      this.btCalcMvFromLatLon.Name = "btCalcMvFromLatLon";
      this.btCalcMvFromLatLon.Size = new System.Drawing.Size(85, 32);
      this.btCalcMvFromLatLon.TabIndex = 16;
      this.btCalcMvFromLatLon.Text = "Calc MagVar";
      this.btCalcMvFromLatLon.UseVisualStyleBackColor = true;
      this.btCalcMvFromLatLon.Click += new System.EventHandler(this.btCalcMvFromLatLon_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(649, 779);
      this.Controls.Add(this.btCalcMvFromLatLon);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.txMv);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.txLon);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txLat);
      this.Controls.Add(this.btDistTable);
      this.Controls.Add(this.btQuadList);
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
    private System.Windows.Forms.Button btQuadList;
    private System.Windows.Forms.Button btDistTable;
    private System.Windows.Forms.TextBox txLat;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txLon;
    private System.Windows.Forms.TextBox txMv;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button btCalcMvFromLatLon;
  }
}

