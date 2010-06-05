using System;
using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    class SplashDialog : System.Windows.Forms.Form
    {
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.PictureBox mPictureBox;
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

        public string ImageLocation
        {
            get { return this.mPictureBox.ImageLocation; }
            set { this.mPictureBox.ImageLocation = value; }
        }

        public SplashDialog()
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
            this.mPictureBox = new System.Windows.Forms.PictureBox();

            this.SuspendLayout();
            // 
            // mMainText
            // 
            this.mMainText.Location = new System.Drawing.Point(20, 0);
            this.mMainText.Name = "label1";
            this.mMainText.Size = new System.Drawing.Size(240, 1);
            this.mMainText.TabIndex = 1;
            this.mMainText.Font = new Font(this.mMainText.Font.FontFamily, 12, FontStyle.Bold);
            this.mMainText.AutoSize = true;
            this.mMainText.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;


            // 
            // Splash
            // 
            this.mPictureBox.Location = new System.Drawing.Point(0, 1);
            this.mPictureBox.Name = "Progress";
            this.mPictureBox.Size = new System.Drawing.Size(300, 300);
            this.mPictureBox.TabIndex = 3;
            this.mPictureBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            this.mPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            this.mPictureBox.ClientSizeChanged += new EventHandler(mPictureBox_ClientSizeChanged);
            // 
            // mSubText
            // 
            this.mSubText.Location = new System.Drawing.Point(20, 301);
            this.mSubText.Name = "txtMessage";
            this.mSubText.Size = new System.Drawing.Size(240, 1);
            this.mMainText.AutoSize = true;
            this.mSubText.TabIndex = 0;
            this.mSubText.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // 
            // DialogForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(300, 350);
            this.ClientSize = new System.Drawing.Size(300, 350);

            this.Controls.Add(this.mSubText);
            this.Controls.Add(this.mMainText);
            this.Controls.Add(this.mPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = true;
            this.Name = "ProgressDialog";
            this.Text = "Progress";
            this.AutoSize = true;
            this.ResumeLayout(true);

        }

        private void mPictureBox_ClientSizeChanged(object sender, EventArgs e)
        {
            this.Location = new Point((Screen.PrimaryScreen.Bounds.Width - this.Size.Width) / 2, (Screen.PrimaryScreen.Bounds.Height - this.Size.Height) / 2);
        }
    }
}
