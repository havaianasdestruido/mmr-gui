namespace mmr_gui;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.Label lblBinDir;
    private System.Windows.Forms.TextBox txtBinDir;
    private System.Windows.Forms.Button btnBrowse;
    private System.Windows.Forms.FlowLayoutPanel flowButtons;
    private System.Windows.Forms.Button btnRunAll;
    private System.Windows.Forms.Button btnRunSelected;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnExportJunit;
    private System.Windows.Forms.Button btnExportJson;
    private System.Windows.Forms.CheckBox chkFailuresOnly;
    private System.Windows.Forms.Label lblDlls;
    private System.Windows.Forms.ListView lstDlls;
    private System.Windows.Forms.Label lblResults;
    private System.Windows.Forms.ListView lvResults;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.ProgressBar prgBar;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        lblBinDir = new System.Windows.Forms.Label();
        txtBinDir = new System.Windows.Forms.TextBox();
        btnBrowse = new System.Windows.Forms.Button();
        flowButtons = new System.Windows.Forms.FlowLayoutPanel();
        btnRunAll = new System.Windows.Forms.Button();
        btnRunSelected = new System.Windows.Forms.Button();
        btnCancel = new System.Windows.Forms.Button();
        btnExportJunit = new System.Windows.Forms.Button();
        btnExportJson = new System.Windows.Forms.Button();
        chkFailuresOnly = new System.Windows.Forms.CheckBox();
        lblDlls = new System.Windows.Forms.Label();
        lstDlls = new System.Windows.Forms.ListView();
        lblResults = new System.Windows.Forms.Label();
        lvResults = new System.Windows.Forms.ListView();
        lblStatus = new System.Windows.Forms.Label();
        prgBar = new System.Windows.Forms.ProgressBar();
        flowButtons.SuspendLayout();
        SuspendLayout();
        // 
        // lblBinDir
        // 
        lblBinDir.AutoSize = true;
        lblBinDir.Location = new System.Drawing.Point(12, 16);
        lblBinDir.Name = "lblBinDir";
        lblBinDir.Size = new System.Drawing.Size(82, 15);
        lblBinDir.TabIndex = 0;
        lblBinDir.Text = "DLL directory:";
        // 
        // txtBinDir
        // 
        txtBinDir.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        txtBinDir.Location = new System.Drawing.Point(98, 12);
        txtBinDir.Name = "txtBinDir";
        txtBinDir.Size = new System.Drawing.Size(726, 23);
        txtBinDir.TabIndex = 1;
        txtBinDir.Text = @"C:\Users\mcmco\Desktop\WMMR\build_clean\bin\Debug";
        // 
        // btnBrowse
        // 
        btnBrowse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        btnBrowse.Location = new System.Drawing.Point(832, 10);
        btnBrowse.Name = "btnBrowse";
        btnBrowse.Size = new System.Drawing.Size(92, 28);
        btnBrowse.TabIndex = 2;
        btnBrowse.Text = "Browse...";
        btnBrowse.UseVisualStyleBackColor = true;
        btnBrowse.Click += BtnBrowse_Click;
        // 
        // flowButtons
        // 
        flowButtons.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        flowButtons.Controls.Add(btnRunAll);
        flowButtons.Controls.Add(btnRunSelected);
        flowButtons.Controls.Add(btnCancel);
        flowButtons.Controls.Add(btnExportJunit);
        flowButtons.Controls.Add(btnExportJson);
        flowButtons.Controls.Add(chkFailuresOnly);
        flowButtons.Location = new System.Drawing.Point(12, 48);
        flowButtons.Name = "flowButtons";
        flowButtons.Size = new System.Drawing.Size(912, 34);
        flowButtons.TabIndex = 3;
        // 
        // btnRunAll
        // 
        btnRunAll.Location = new System.Drawing.Point(3, 3);
        btnRunAll.Name = "btnRunAll";
        btnRunAll.Size = new System.Drawing.Size(90, 28);
        btnRunAll.TabIndex = 0;
        btnRunAll.Text = "Run All";
        btnRunAll.UseVisualStyleBackColor = true;
        btnRunAll.Click += BtnRunAll_Click;
        // 
        // btnRunSelected
        // 
        btnRunSelected.Location = new System.Drawing.Point(99, 3);
        btnRunSelected.Name = "btnRunSelected";
        btnRunSelected.Size = new System.Drawing.Size(100, 28);
        btnRunSelected.TabIndex = 1;
        btnRunSelected.Text = "Run Selected";
        btnRunSelected.UseVisualStyleBackColor = true;
        btnRunSelected.Click += BtnRunSelected_Click;
        // 
        // btnCancel
        // 
        btnCancel.Enabled = false;
        btnCancel.Location = new System.Drawing.Point(205, 3);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new System.Drawing.Size(76, 28);
        btnCancel.TabIndex = 2;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += BtnCancel_Click;
        // 
        // btnExportJunit
        // 
        btnExportJunit.Enabled = false;
        btnExportJunit.Location = new System.Drawing.Point(287, 3);
        btnExportJunit.Name = "btnExportJunit";
        btnExportJunit.Size = new System.Drawing.Size(96, 28);
        btnExportJunit.TabIndex = 3;
        btnExportJunit.Text = "Export JUnit";
        btnExportJunit.UseVisualStyleBackColor = true;
        btnExportJunit.Click += BtnExportJunit_Click;
        // 
        // btnExportJson
        // 
        btnExportJson.Enabled = false;
        btnExportJson.Location = new System.Drawing.Point(389, 3);
        btnExportJson.Name = "btnExportJson";
        btnExportJson.Size = new System.Drawing.Size(96, 28);
        btnExportJson.TabIndex = 4;
        btnExportJson.Text = "Export JSON";
        btnExportJson.UseVisualStyleBackColor = true;
        btnExportJson.Click += BtnExportJson_Click;
        // 
        // chkFailuresOnly
        // 
        chkFailuresOnly.AutoSize = true;
        chkFailuresOnly.Location = new System.Drawing.Point(491, 8);
        chkFailuresOnly.Name = "chkFailuresOnly";
        chkFailuresOnly.Size = new System.Drawing.Size(110, 19);
        chkFailuresOnly.TabIndex = 5;
        chkFailuresOnly.Text = "Failures only";
        chkFailuresOnly.UseVisualStyleBackColor = true;
        chkFailuresOnly.CheckedChanged += ChkFailuresOnly_CheckedChanged;
        // 
        // lblDlls
        // 
        lblDlls.Location = new System.Drawing.Point(12, 92);
        lblDlls.Name = "lblDlls";
        lblDlls.Size = new System.Drawing.Size(240, 15);
        lblDlls.TabIndex = 4;
        lblDlls.Text = "DLLs";
        // 
        // lstDlls
        // 
        lstDlls.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        lstDlls.CheckBoxes = true;
        lstDlls.FullRowSelect = true;
        lstDlls.GridLines = true;
        lstDlls.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
        lstDlls.Location = new System.Drawing.Point(12, 110);
        lstDlls.Name = "lstDlls";
        lstDlls.Size = new System.Drawing.Size(240, 430);
        lstDlls.TabIndex = 5;
        lstDlls.UseCompatibleStateImageBehavior = false;
        lstDlls.View = System.Windows.Forms.View.Details;
        lstDlls.Columns.Add("DLL", 150);
        lstDlls.Columns.Add("Tests", 45);
        lstDlls.Columns.Add("Passed", 45);
        // 
        // lblResults
        // 
        lblResults.Location = new System.Drawing.Point(260, 92);
        lblResults.Name = "lblResults";
        lblResults.Size = new System.Drawing.Size(668, 15);
        lblResults.TabIndex = 6;
        lblResults.Text = "Results";
        // 
        // lvResults
        // 
        lvResults.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        lvResults.FullRowSelect = true;
        lvResults.GridLines = true;
        lvResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
        lvResults.Location = new System.Drawing.Point(260, 110);
        lvResults.Name = "lvResults";
        lvResults.Size = new System.Drawing.Size(668, 430);
        lvResults.TabIndex = 7;
        lvResults.UseCompatibleStateImageBehavior = false;
        lvResults.View = System.Windows.Forms.View.Details;
        lvResults.Columns.Add("Check", 240);
        lvResults.Columns.Add("DLL", 130);
        lvResults.Columns.Add("Group", 120);
        lvResults.Columns.Add("Result", 55);
        lvResults.Columns.Add("ms", 45);
        lvResults.Columns.Add("Detail", 200);
        // 
        // lblStatus
        // 
        lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        lblStatus.AutoSize = true;
        lblStatus.Location = new System.Drawing.Point(12, 548);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new System.Drawing.Size(400, 15);
        lblStatus.TabIndex = 8;
        lblStatus.Text = "Ready";
        // 
        // prgBar
        // 
        prgBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        prgBar.Location = new System.Drawing.Point(580, 546);
        prgBar.Name = "prgBar";
        prgBar.Size = new System.Drawing.Size(348, 16);
        prgBar.TabIndex = 9;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(940, 572);
        Controls.Add(prgBar);
        Controls.Add(lblStatus);
        Controls.Add(lvResults);
        Controls.Add(lblResults);
        Controls.Add(lstDlls);
        Controls.Add(lblDlls);
        Controls.Add(flowButtons);
        Controls.Add(btnBrowse);
        Controls.Add(txtBinDir);
        Controls.Add(lblBinDir);
        MinimumSize = new System.Drawing.Size(760, 440);
        Name = "Form1";
        Text = "WMMR 32-bit DLL Suite Browser";
        flowButtons.ResumeLayout(false);
        flowButtons.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}
