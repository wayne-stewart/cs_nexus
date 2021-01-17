
namespace AlphaSQL
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.master_split_container = new System.Windows.Forms.SplitContainer();
            this.left_split_container = new System.Windows.Forms.SplitContainer();
            this.file_list_box = new System.Windows.Forms.CheckedListBox();
            this.master_right_split_container = new System.Windows.Forms.SplitContainer();
            this.center_split_container = new System.Windows.Forms.SplitContainer();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.right_split_container = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.master_split_container)).BeginInit();
            this.master_split_container.Panel1.SuspendLayout();
            this.master_split_container.Panel2.SuspendLayout();
            this.master_split_container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.left_split_container)).BeginInit();
            this.left_split_container.Panel1.SuspendLayout();
            this.left_split_container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.master_right_split_container)).BeginInit();
            this.master_right_split_container.Panel1.SuspendLayout();
            this.master_right_split_container.Panel2.SuspendLayout();
            this.master_right_split_container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.center_split_container)).BeginInit();
            this.center_split_container.Panel1.SuspendLayout();
            this.center_split_container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.right_split_container)).BeginInit();
            this.right_split_container.SuspendLayout();
            this.SuspendLayout();
            // 
            // master_split_container
            // 
            this.master_split_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.master_split_container.Location = new System.Drawing.Point(0, 0);
            this.master_split_container.Name = "master_split_container";
            // 
            // master_split_container.Panel1
            // 
            this.master_split_container.Panel1.Controls.Add(this.left_split_container);
            // 
            // master_split_container.Panel2
            // 
            this.master_split_container.Panel2.Controls.Add(this.master_right_split_container);
            this.master_split_container.Size = new System.Drawing.Size(1507, 820);
            this.master_split_container.SplitterDistance = 408;
            this.master_split_container.TabIndex = 0;
            // 
            // left_split_container
            // 
            this.left_split_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.left_split_container.Location = new System.Drawing.Point(0, 0);
            this.left_split_container.Name = "left_split_container";
            this.left_split_container.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // left_split_container.Panel1
            // 
            this.left_split_container.Panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.left_split_container.Panel1.Controls.Add(this.file_list_box);
            // 
            // left_split_container.Panel2
            // 
            this.left_split_container.Panel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.left_split_container.Size = new System.Drawing.Size(408, 820);
            this.left_split_container.SplitterDistance = 410;
            this.left_split_container.TabIndex = 0;
            // 
            // file_list_box
            // 
            this.file_list_box.CheckOnClick = true;
            this.file_list_box.FormattingEnabled = true;
            this.file_list_box.Location = new System.Drawing.Point(0, 46);
            this.file_list_box.Margin = new System.Windows.Forms.Padding(0);
            this.file_list_box.Name = "file_list_box";
            this.file_list_box.Size = new System.Drawing.Size(408, 364);
            this.file_list_box.TabIndex = 0;
            // 
            // master_right_split_container
            // 
            this.master_right_split_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.master_right_split_container.Location = new System.Drawing.Point(0, 0);
            this.master_right_split_container.Name = "master_right_split_container";
            // 
            // master_right_split_container.Panel1
            // 
            this.master_right_split_container.Panel1.Controls.Add(this.center_split_container);
            // 
            // master_right_split_container.Panel2
            // 
            this.master_right_split_container.Panel2.Controls.Add(this.right_split_container);
            this.master_right_split_container.Size = new System.Drawing.Size(1095, 820);
            this.master_right_split_container.SplitterDistance = 701;
            this.master_right_split_container.TabIndex = 0;
            // 
            // center_split_container
            // 
            this.center_split_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.center_split_container.Location = new System.Drawing.Point(0, 0);
            this.center_split_container.Name = "center_split_container";
            this.center_split_container.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // center_split_container.Panel1
            // 
            this.center_split_container.Panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.center_split_container.Panel1.Controls.Add(this.textBox2);
            this.center_split_container.Panel1.Controls.Add(this.textBox1);
            // 
            // center_split_container.Panel2
            // 
            this.center_split_container.Panel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.center_split_container.Size = new System.Drawing.Size(701, 820);
            this.center_split_container.SplitterDistance = 600;
            this.center_split_container.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox2.Location = new System.Drawing.Point(0, 0);
            this.textBox2.Margin = new System.Windows.Forms.Padding(0);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(701, 23);
            this.textBox2.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(0, 22);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(701, 578);
            this.textBox1.TabIndex = 0;
            // 
            // right_split_container
            // 
            this.right_split_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.right_split_container.Location = new System.Drawing.Point(0, 0);
            this.right_split_container.Name = "right_split_container";
            this.right_split_container.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // right_split_container.Panel1
            // 
            this.right_split_container.Panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            // 
            // right_split_container.Panel2
            // 
            this.right_split_container.Panel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.right_split_container.Size = new System.Drawing.Size(390, 820);
            this.right_split_container.SplitterDistance = 400;
            this.right_split_container.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1507, 820);
            this.Controls.Add(this.master_split_container);
            this.Name = "MainForm";
            this.Text = "Alpha SQL";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.master_split_container.Panel1.ResumeLayout(false);
            this.master_split_container.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.master_split_container)).EndInit();
            this.master_split_container.ResumeLayout(false);
            this.left_split_container.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.left_split_container)).EndInit();
            this.left_split_container.ResumeLayout(false);
            this.master_right_split_container.Panel1.ResumeLayout(false);
            this.master_right_split_container.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.master_right_split_container)).EndInit();
            this.master_right_split_container.ResumeLayout(false);
            this.center_split_container.Panel1.ResumeLayout(false);
            this.center_split_container.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.center_split_container)).EndInit();
            this.center_split_container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.right_split_container)).EndInit();
            this.right_split_container.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer master_split_container;
        private System.Windows.Forms.SplitContainer left_split_container;
        private System.Windows.Forms.SplitContainer master_right_split_container;
        private System.Windows.Forms.SplitContainer center_split_container;
        private System.Windows.Forms.SplitContainer right_split_container;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckedListBox file_list_box;
    }
}

