using System;
using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    class ProgressDialog : System.Windows.Forms.Form
    {
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.ProgressBar mProgressBar;
        private System.Windows.Forms.Label mSubText;
        private System.Windows.Forms.Label mMainText;

        public string Title
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        public string SubText
        {
            get { return mSubText.Text; }
            set { this.mSubText.Text = value; }
        }

        public string MainText
        {
            get { return mMainText.Text; }
            set { this.mMainText.Text = value; }
        }

        public int Value
        {
            get { return this.mProgressBar.Value; }
            set { this.mProgressBar.Value = value; }
        }

        public ProgressDialog()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mSubText = new System.Windows.Forms.Label();
            this.mMainText = new System.Windows.Forms.Label();
            this.mProgressBar = new System.Windows.Forms.ProgressBar();

            this.SuspendLayout();
            // 
            // mMainText
            // 
            this.mMainText.Location = new System.Drawing.Point(20, 20);
            this.mMainText.Name = "label1";
            this.mMainText.Size = new System.Drawing.Size(240, 48);
            this.mMainText.TabIndex = 1;
            this.mMainText.Font = new Font(this.mMainText.Font.FontFamily, 12, FontStyle.Bold);
            this.mMainText.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;


            // 
            // Progress
            // 
            this.mProgressBar.Location = new System.Drawing.Point(10, 70);
            this.mProgressBar.Name = "Progress";
            this.mProgressBar.Size = new System.Drawing.Size(300, 24);
            this.mProgressBar.TabIndex = 3;
            this.mProgressBar.Value = 0;
            this.mProgressBar.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // 
            // mSubText
            // 
            this.mSubText.Location = new System.Drawing.Point(20, 100);
            this.mSubText.Name = "txtMessage";
            this.mSubText.Size = new System.Drawing.Size(232, 20);
            this.mSubText.TabIndex = 0;
            this.mSubText.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // 
            // DialogForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(320, 151);
            this.ControlBox = true;
            this.Controls.Add(this.mSubText);
            this.Controls.Add(this.mMainText);
            this.Controls.Add(this.mProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressDialog";
            this.Text = "Progress";
            this.ResumeLayout(true);
        }
    }
}
