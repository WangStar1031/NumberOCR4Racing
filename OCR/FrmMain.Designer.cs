using System.Windows.Forms;

namespace OCR
{
    partial class FrmMain
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
            this.pic_obj = new System.Windows.Forms.PictureBox();
            this.lbl_result = new System.Windows.Forms.Label();
            this.cmd_track = new System.Windows.Forms.ComboBox();
            this.txt_left = new System.Windows.Forms.TextBox();
            this.txt_top = new System.Windows.Forms.TextBox();
            this.txt_width = new System.Windows.Forms.TextBox();
            this.txt_height = new System.Windows.Forms.TextBox();
            this.btn_save = new System.Windows.Forms.Button();
            this.txt_origin = new System.Windows.Forms.TextBox();
            this.pic_pattern = new System.Windows.Forms.PictureBox();
            this.txt_oddsType = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pic_obj)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_pattern)).BeginInit();
            this.SuspendLayout();
            // 
            // pic_obj
            // 
            this.pic_obj.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pic_obj.Location = new System.Drawing.Point(0, 1);
            this.pic_obj.Margin = new System.Windows.Forms.Padding(0);
            this.pic_obj.Name = "pic_obj";
            this.pic_obj.Padding = new System.Windows.Forms.Padding(3);
            this.pic_obj.Size = new System.Drawing.Size(685, 160);
            this.pic_obj.TabIndex = 0;
            this.pic_obj.TabStop = false;
            // 
            // lbl_result
            // 
            this.lbl_result.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_result.AutoSize = true;
            this.lbl_result.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lbl_result.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbl_result.Location = new System.Drawing.Point(12, 176);
            this.lbl_result.Name = "lbl_result";
            this.lbl_result.Size = new System.Drawing.Size(0, 25);
            this.lbl_result.TabIndex = 1;
            // 
            // cmd_track
            // 
            this.cmd_track.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmd_track.BackColor = System.Drawing.SystemColors.Window;
            this.cmd_track.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmd_track.DropDownWidth = 200;
            this.cmd_track.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmd_track.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cmd_track.Location = new System.Drawing.Point(487, 164);
            this.cmd_track.Name = "cmd_track";
            this.cmd_track.Size = new System.Drawing.Size(198, 45);
            this.cmd_track.TabIndex = 2;
            this.cmd_track.SelectedIndexChanged += new System.EventHandler(this.cmd_track_SelectedIndexChanged);
            // 
            // txt_left
            // 
            this.txt_left.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_left.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_left.Location = new System.Drawing.Point(318, 170);
            this.txt_left.Name = "txt_left";
            this.txt_left.Size = new System.Drawing.Size(42, 31);
            this.txt_left.TabIndex = 3;
            this.txt_left.Text = "10";
            this.txt_left.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_top
            // 
            this.txt_top.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_top.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_top.Location = new System.Drawing.Point(360, 170);
            this.txt_top.Name = "txt_top";
            this.txt_top.Size = new System.Drawing.Size(42, 31);
            this.txt_top.TabIndex = 4;
            this.txt_top.Text = "10";
            this.txt_top.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_width
            // 
            this.txt_width.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_width.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_width.Location = new System.Drawing.Point(402, 170);
            this.txt_width.Name = "txt_width";
            this.txt_width.Size = new System.Drawing.Size(42, 31);
            this.txt_width.TabIndex = 5;
            this.txt_width.Text = "10";
            this.txt_width.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_height
            // 
            this.txt_height.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_height.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_height.Location = new System.Drawing.Point(444, 170);
            this.txt_height.Name = "txt_height";
            this.txt_height.Size = new System.Drawing.Size(42, 31);
            this.txt_height.TabIndex = 6;
            this.txt_height.Text = "10";
            this.txt_height.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btn_save
            // 
            this.btn_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save.Location = new System.Drawing.Point(654, 127);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(31, 31);
            this.btn_save.TabIndex = 7;
            this.btn_save.Text = "!";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // txt_origin
            // 
            this.txt_origin.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_origin.Location = new System.Drawing.Point(656, 1);
            this.txt_origin.Name = "txt_origin";
            this.txt_origin.Size = new System.Drawing.Size(29, 31);
            this.txt_origin.TabIndex = 8;
            // 
            // pic_pattern
            // 
            this.pic_pattern.Location = new System.Drawing.Point(660, 38);
            this.pic_pattern.Name = "pic_pattern";
            this.pic_pattern.Size = new System.Drawing.Size(20, 20);
            this.pic_pattern.TabIndex = 9;
            this.pic_pattern.TabStop = false;
            // 
            // txt_oddsType
            // 
            this.txt_oddsType.Location = new System.Drawing.Point(654, 101);
            this.txt_oddsType.Name = "txt_oddsType";
            this.txt_oddsType.Size = new System.Drawing.Size(18, 20);
            this.txt_oddsType.TabIndex = 10;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(684, 209);
            this.Controls.Add(this.txt_oddsType);
            this.Controls.Add(this.pic_pattern);
            this.Controls.Add(this.txt_origin);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.txt_height);
            this.Controls.Add(this.txt_width);
            this.Controls.Add(this.txt_top);
            this.Controls.Add(this.txt_left);
            this.Controls.Add(this.cmd_track);
            this.Controls.Add(this.lbl_result);
            this.Controls.Add(this.pic_obj);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Location = new System.Drawing.Point(0, 580);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "OCR";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pic_obj)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_pattern)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pic_obj;
        private Label lbl_result;
        private ComboBox cmd_track;
        private TextBox txt_left;
        private TextBox txt_top;
        private TextBox txt_width;
        private TextBox txt_height;
        private Button btn_save;
        private TextBox txt_origin;
        private PictureBox pic_pattern;
        private TextBox txt_oddsType;
    }
}

