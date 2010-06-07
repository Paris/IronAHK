using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    class SplashDialog : Form
    {
        private Container components;
        private PictureBox mPictureBox;
        private Label mSubText;
        private Label mMainText;

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        public string SubText
        {
            get { return mSubText.Text; }
            set { mSubText.Text = value; }
        }

        public string MainText
        {
            get { return mMainText.Text; }
            set { mMainText.Text = value; }
        }

        public string ImageLocation
        {
            get { return mPictureBox.ImageLocation; }
            set { mPictureBox.ImageLocation = value; }
        }

        public SplashDialog()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
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
            mSubText = new Label();
            mMainText = new Label();
            mPictureBox = new PictureBox();

            SuspendLayout();
            // 
            // mMainText
            // 
            mMainText.Location = new Point(20, 0);
            mMainText.Name = "label1";
            mMainText.Size = new Size(240, 1);
            mMainText.TabIndex = 1;
            mMainText.Font = new Font(mMainText.Font.FontFamily, 12, FontStyle.Bold);
            mMainText.AutoSize = true;
            mMainText.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;


            // 
            // Splash
            // 
            mPictureBox.Location = new Point(0, 1);
            mPictureBox.Name = "Progress";
            mPictureBox.Size = new Size(300, 300);
            mPictureBox.TabIndex = 3;
            mPictureBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            mPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            mPictureBox.ClientSizeChanged += mPictureBox_ClientSizeChanged;
            // 
            // mSubText
            // 
            mSubText.Location = new Point(20, 301);
            mSubText.Name = "txtMessage";
            mSubText.Size = new Size(240, 1);
            mMainText.AutoSize = true;
            mSubText.TabIndex = 0;
            mSubText.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // 
            // DialogForm
            // 
            AutoScaleBaseSize = new Size(300, 350);
            ClientSize = new Size(300, 350);

            Controls.Add(mSubText);
            Controls.Add(mMainText);
            Controls.Add(mPictureBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = true;
            Name = "ProgressDialog";
            Text = "Progress";
            AutoSize = true;
            ResumeLayout(true);

        }

        private void mPictureBox_ClientSizeChanged(object sender, EventArgs e)
        {
            Location = new Point((Screen.PrimaryScreen.Bounds.Width - Size.Width) / 2, (Screen.PrimaryScreen.Bounds.Height - Size.Height) / 2);
        }
    }
}
