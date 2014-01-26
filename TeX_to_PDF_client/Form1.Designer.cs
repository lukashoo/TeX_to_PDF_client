namespace TeX_to_PDF_client
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
            this.button_choose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_path = new System.Windows.Forms.TextBox();
            this.textBox_filename = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_fname = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button_ss = new System.Windows.Forms.Button();
            this.button_send = new System.Windows.Forms.Button();
            this.button_delete = new System.Windows.Forms.Button();
            this.button_get = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_choose
            // 
            this.button_choose.Location = new System.Drawing.Point(49, 116);
            this.button_choose.Name = "button_choose";
            this.button_choose.Size = new System.Drawing.Size(70, 23);
            this.button_choose.TabIndex = 0;
            this.button_choose.Text = "Choose File";
            this.button_choose.UseVisualStyleBackColor = true;
            this.button_choose.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_path);
            this.groupBox1.Controls.Add(this.textBox_filename);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button_choose);
            this.groupBox1.Location = new System.Drawing.Point(12, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(166, 145);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Send File";
            // 
            // textBox_path
            // 
            this.textBox_path.Location = new System.Drawing.Point(76, 77);
            this.textBox_path.Name = "textBox_path";
            this.textBox_path.ReadOnly = true;
            this.textBox_path.Size = new System.Drawing.Size(84, 20);
            this.textBox_path.TabIndex = 3;
            // 
            // textBox_filename
            // 
            this.textBox_filename.Location = new System.Drawing.Point(76, 51);
            this.textBox_filename.Name = "textBox_filename";
            this.textBox_filename.ReadOnly = true;
            this.textBox_filename.Size = new System.Drawing.Size(84, 20);
            this.textBox_filename.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "From path:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File to Send:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_fname);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(184, 40);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(163, 145);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Manage files on server";
            // 
            // textBox_fname
            // 
            this.textBox_fname.Location = new System.Drawing.Point(64, 64);
            this.textBox_fname.Name = "textBox_fname";
            this.textBox_fname.Size = new System.Drawing.Size(93, 20);
            this.textBox_fname.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Filename:";
            // 
            // button_ss
            // 
            this.button_ss.Enabled = false;
            this.button_ss.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button_ss.Location = new System.Drawing.Point(19, 199);
            this.button_ss.Name = "button_ss";
            this.button_ss.Size = new System.Drawing.Size(70, 23);
            this.button_ss.TabIndex = 3;
            this.button_ss.Text = "Send + Save";
            this.button_ss.UseVisualStyleBackColor = true;
            this.button_ss.Click += new System.EventHandler(this.button_ss_Click);
            // 
            // button_send
            // 
            this.button_send.Enabled = false;
            this.button_send.Location = new System.Drawing.Point(99, 199);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(70, 23);
            this.button_send.TabIndex = 4;
            this.button_send.Text = "Send";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // button_delete
            // 
            this.button_delete.Location = new System.Drawing.Point(271, 199);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(70, 23);
            this.button_delete.TabIndex = 6;
            this.button_delete.Text = "Delete";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // button_get
            // 
            this.button_get.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button_get.Location = new System.Drawing.Point(191, 199);
            this.button_get.Name = "button_get";
            this.button_get.Size = new System.Drawing.Size(70, 23);
            this.button_get.TabIndex = 5;
            this.button_get.Text = "Get";
            this.button_get.UseVisualStyleBackColor = true;
            this.button_get.Click += new System.EventHandler(this.button_get_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 261);
            this.Controls.Add(this.button_delete);
            this.Controls.Add(this.button_get);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.button_ss);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_choose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_path;
        private System.Windows.Forms.TextBox textBox_filename;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_ss;
        private System.Windows.Forms.Button button_send;
        private System.Windows.Forms.TextBox textBox_fname;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Button button_get;



    }
}

