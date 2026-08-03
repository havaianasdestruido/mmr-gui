namespace mmr_gui;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.Label lblBinDir;
    private System.Windows.Forms.TextBox txtBinDir;
    private System.Windows.Forms.FlowLayoutPanel flowButtons;
    private System.Windows.Forms.Button btnRunAll;
    private System.Windows.Forms.Button btnWlidcli;
    private System.Windows.Forms.Button btnUxctl;
    private System.Windows.Forms.Button btnVideo;
    private System.Windows.Forms.Button btnPipe;
    private System.Windows.Forms.Button btnMM;
    private System.Windows.Forms.ListBox lstLog;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        lblBinDir = new System.Windows.Forms.Label();
        txtBinDir = new System.Windows.Forms.TextBox();
        flowButtons = new System.Windows.Forms.FlowLayoutPanel();
        btnRunAll = new System.Windows.Forms.Button();
        btnWlidcli = new System.Windows.Forms.Button();
        btnUxctl = new System.Windows.Forms.Button();
        btnVideo = new System.Windows.Forms.Button();
        btnPipe = new System.Windows.Forms.Button();
        btnMM = new System.Windows.Forms.Button();
        lstLog = new System.Windows.Forms.ListBox();
        flowButtons.SuspendLayout();
        SuspendLayout();
        // 
        // lblBinDir
        // 
        lblBinDir.AutoSize = true;
        lblBinDir.Location = new System.Drawing.Point(12, 16);
        lblBinDir.Name = "lblBinDir";
        lblBinDir.Size = new System.Drawing.Size(74, 15);
        lblBinDir.TabIndex = 0;
        lblBinDir.Text = "DLL directory:";
        // 
        // txtBinDir
        // 
        txtBinDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtBinDir.Location = new System.Drawing.Point(92, 12);
        txtBinDir.Name = "txtBinDir";
        txtBinDir.Size = new System.Drawing.Size(696, 23);
        txtBinDir.TabIndex = 1;
        txtBinDir.Text = @"C:\Users\mcmco\Desktop\WMMR\build_clean\bin\Debug";
        // 
        // flowButtons
        // 
        flowButtons.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        flowButtons.Controls.Add(btnRunAll);
        flowButtons.Controls.Add(btnWlidcli);
        flowButtons.Controls.Add(btnUxctl);
        flowButtons.Controls.Add(btnVideo);
        flowButtons.Controls.Add(btnPipe);
        flowButtons.Controls.Add(btnMM);
        flowButtons.Location = new System.Drawing.Point(12, 45);
        flowButtons.Name = "flowButtons";
        flowButtons.Size = new System.Drawing.Size(776, 34);
        flowButtons.TabIndex = 2;
        // 
        // btnRunAll
        // 
        btnRunAll.Location = new System.Drawing.Point(3, 3);
        btnRunAll.Name = "btnRunAll";
        btnRunAll.Size = new System.Drawing.Size(80, 28);
        btnRunAll.TabIndex = 0;
        btnRunAll.Text = "Run All";
        btnRunAll.UseVisualStyleBackColor = true;
        btnRunAll.Click += BtnRunAll_Click;
        // 
        // btnWlidcli
        // 
        btnWlidcli.Location = new System.Drawing.Point(89, 3);
        btnWlidcli.Name = "btnWlidcli";
        btnWlidcli.Size = new System.Drawing.Size(82, 28);
        btnWlidcli.TabIndex = 1;
        btnWlidcli.Text = "wlidcli.dll";
        btnWlidcli.UseVisualStyleBackColor = true;
        btnWlidcli.Click += BtnWlidcli_Click;
        // 
        // btnUxctl
        // 
        btnUxctl.Location = new System.Drawing.Point(177, 3);
        btnUxctl.Name = "btnUxctl";
        btnUxctl.Size = new System.Drawing.Size(78, 28);
        btnUxctl.TabIndex = 2;
        btnUxctl.Text = "uxctl.dll";
        btnUxctl.UseVisualStyleBackColor = true;
        btnUxctl.Click += BtnUxctl_Click;
        // 
        // btnVideo
        // 
        btnVideo.Location = new System.Drawing.Point(261, 3);
        btnVideo.Name = "btnVideo";
        btnVideo.Size = new System.Drawing.Size(104, 28);
        btnVideo.TabIndex = 3;
        btnVideo.Text = "WLXVideoTrim";
        btnVideo.UseVisualStyleBackColor = true;
        btnVideo.Click += BtnVideo_Click;
        // 
        // btnPipe
        // 
        btnPipe.Location = new System.Drawing.Point(371, 3);
        btnPipe.Name = "btnPipe";
        btnPipe.Size = new System.Drawing.Size(98, 28);
        btnPipe.TabIndex = 4;
        btnPipe.Text = "WLXPipetran";
        btnPipe.UseVisualStyleBackColor = true;
        btnPipe.Click += BtnPipe_Click;
        // 
        // btnMM
        // 
        btnMM.Location = new System.Drawing.Point(475, 3);
        btnMM.Name = "btnMM";
        btnMM.Size = new System.Drawing.Size(120, 28);
        btnMM.TabIndex = 5;
        btnMM.Text = "MovieMakerCore";
        btnMM.UseVisualStyleBackColor = true;
        btnMM.Click += BtnMM_Click;
        // 
        // lstLog
        // 
        lstLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lstLog.FormattingEnabled = true;
        lstLog.IntegralHeight = false;
        lstLog.ItemHeight = 15;
        lstLog.Location = new System.Drawing.Point(12, 86);
        lstLog.Name = "lstLog";
        lstLog.Size = new System.Drawing.Size(776, 352);
        lstLog.TabIndex = 3;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 450);
        Controls.Add(lstLog);
        Controls.Add(flowButtons);
        Controls.Add(txtBinDir);
        Controls.Add(lblBinDir);
        MinimumSize = new System.Drawing.Size(640, 320);
        Name = "Form1";
        Text = "WMMR 32-bit DLL Test Harness";
        flowButtons.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}
