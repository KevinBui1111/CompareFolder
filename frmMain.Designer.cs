namespace CompareFolder
{
    partial class frmMain
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDes = new System.Windows.Forms.TextBox();
            this.btnCompare = new System.Windows.Forms.Button();
            this.olvResult = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.olvResult)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source:";
            // 
            // txtSource
            // 
            this.txtSource.AllowDrop = true;
            this.txtSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSource.Location = new System.Drawing.Point(85, 6);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(300, 23);
            this.txtSource.TabIndex = 1;
            this.txtSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtSrc_DragDrop);
            this.txtSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtSrc_DragEnter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Destination:";
            // 
            // txtDes
            // 
            this.txtDes.AllowDrop = true;
            this.txtDes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDes.Location = new System.Drawing.Point(85, 35);
            this.txtDes.Name = "txtDes";
            this.txtDes.Size = new System.Drawing.Size(300, 23);
            this.txtDes.TabIndex = 1;
            this.txtDes.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtSrc_DragDrop);
            this.txtDes.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtSrc_DragEnter);
            // 
            // btnCompare
            // 
            this.btnCompare.Location = new System.Drawing.Point(85, 64);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(75, 27);
            this.btnCompare.TabIndex = 2;
            this.btnCompare.Text = "Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // olvResult
            // 
            this.olvResult.AllColumns.Add(this.olvColumn1);
            this.olvResult.AllColumns.Add(this.olvColumn2);
            this.olvResult.AllColumns.Add(this.olvColumn3);
            this.olvResult.AllColumns.Add(this.olvColumn4);
            this.olvResult.AllColumns.Add(this.olvColumn5);
            this.olvResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn4,
            this.olvColumn5});
            this.olvResult.FullRowSelect = true;
            this.olvResult.GridLines = true;
            this.olvResult.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.olvResult.Location = new System.Drawing.Point(12, 97);
            this.olvResult.Name = "olvResult";
            this.olvResult.Size = new System.Drawing.Size(373, 347);
            this.olvResult.TabIndex = 3;
            this.olvResult.UseCompatibleStateImageBehavior = false;
            this.olvResult.View = System.Windows.Forms.View.Details;
            this.olvResult.BeforeCreatingGroups += new System.EventHandler<BrightIdeasSoftware.CreateGroupsEventArgs>(this.olvResult_BeforeCreatingGroups);
            this.olvResult.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.olvResult_FormatRow);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "filename";
            this.olvColumn1.Text = "File name";
            this.olvColumn1.Width = 218;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Date";
            this.olvColumn2.DisplayIndex = 1;
            this.olvColumn2.IsVisible = false;
            this.olvColumn2.Text = "Modified Date";
            this.olvColumn2.Width = 121;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "folder";
            this.olvColumn3.DisplayIndex = 1;
            this.olvColumn3.IsVisible = false;
            this.olvColumn3.Text = "Folder";
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "operation";
            this.olvColumn4.Text = "Status";
            this.olvColumn4.Width = 76;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "action";
            this.olvColumn5.Text = "Action";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(166, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 27);
            this.button1.TabIndex = 2;
            this.button1.Text = "Synchronize...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnSyn_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 456);
            this.Controls.Add(this.olvResult);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.txtDes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = global::CompareFolder.Properties.Resources.folder;
            this.Name = "frmMain";
            this.Text = "Compare Folder v1.1 Build 25.02.2018";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.olvResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDes;
        private System.Windows.Forms.Button btnCompare;
        private BrightIdeasSoftware.ObjectListView olvResult;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private System.Windows.Forms.Button button1;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
    }
}

