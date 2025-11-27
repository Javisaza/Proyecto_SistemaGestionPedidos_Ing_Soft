using System;

namespace KITCH.Registro_e_inicio_de_sesion
{
    partial class PantallaCarga
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PantallaCarga));
            this.guna2ProgressBar1 = new Guna.UI2.WinForms.Guna2ProgressBar();
            this.timerCarga = new System.Windows.Forms.Timer(this.components);
            this.lblEstado = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.SuspendLayout();
            // 
            // guna2ProgressBar1
            // 
            this.guna2ProgressBar1.Location = new System.Drawing.Point(154, 134);
            this.guna2ProgressBar1.Name = "guna2ProgressBar1";
            this.guna2ProgressBar1.Size = new System.Drawing.Size(171, 26);
            this.guna2ProgressBar1.TabIndex = 0;
            this.guna2ProgressBar1.Text = "guna2ProgressBar1";
            this.guna2ProgressBar1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            // 
            // lblEstado
            // 
            this.lblEstado.BackColor = System.Drawing.Color.Transparent;
            this.lblEstado.Location = new System.Drawing.Point(199, 166);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(3, 2);
            this.lblEstado.TabIndex = 1;
            // 
            // PantallaCarga
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(482, 282);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.guna2ProgressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PantallaCarga";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PantallaCarga";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2ProgressBar guna2ProgressBar1;
        private System.Windows.Forms.Timer timerCarga;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblEstado;
    }
}