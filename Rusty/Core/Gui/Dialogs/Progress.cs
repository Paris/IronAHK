using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    class ProgressDialog : Form
    {
        private Container components;
        private ProgressBar mProgressBar;
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

        public int Value
        {
            get { return mProgressBar.Value; }
            set { mProgressBar.Value = value; }
        }

        public ProgressDialog()
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
            mProgressBar = new ProgressBar();

            SuspendLayout();
            // 
            // mMainText
            // 
            mMainText.Location = new Point(20, 20);
            mMainText.Name = "label1";
            mMainText.Size = new Size(240, 48);
            mMainText.TabIndex = 1;
            mMainText.Font = new Font(mMainText.Font.FontFamily, 12, FontStyle.Bold);
            mMainText.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;


            // 
            // Progress
            // 
            mProgressBar.Location = new Point(10, 70);
            mProgressBar.Name = "Progress";
            mProgressBar.Size = new Size(300, 24);
            mProgressBar.TabIndex = 3;
            mProgressBar.Value = 0;
            mProgressBar.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // 
            // mSubText
            // 
            mSubText.Location = new Point(20, 100);
            mSubText.Name = "txtMessage";
            mSubText.Size = new Size(232, 20);
            mSubText.TabIndex = 0;
            mSubText.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // 
            // DialogForm
            // 
            AutoScaleBaseSize = new Size(5, 13);
            ClientSize = new Size(320, 151);
            ControlBox = true;
            Controls.Add(mSubText);
            Controls.Add(mMainText);
            Controls.Add(mProgressBar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressDialog";
            Text = "Progress";
            ResumeLayout(true);
        }
    }
}
