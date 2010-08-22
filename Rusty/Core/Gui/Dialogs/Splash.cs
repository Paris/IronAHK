using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    class SplashDialog : Form
    {
        Label main, sub;
        PictureBox pic;

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        public string Subtext
        {
            get { return sub.Text; }
            set
            {
                sub.Text = value;
                sub.Visible = !string.IsNullOrEmpty(sub.Text);
            }
        }

        public string MainText
        {
            get { return main.Text; }
            set
            {
                main.Text = value;
                main.Visible = !string.IsNullOrEmpty(main.Text);
            }
        }

        public string Image
        {
            get { return pic.ImageLocation; }
            set
            {
                pic.ImageLocation = value;
                pic.Load();
            }
        }

        public SplashDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            main = new Label();
            sub = new Label();
            pic = new PictureBox();

            main.Dock = DockStyle.Top;
            main.Font = new Font(main.Font, FontStyle.Bold);
            main.TextAlign = ContentAlignment.MiddleCenter;

            pic.Dock = DockStyle.Fill;
            pic.SizeMode = PictureBoxSizeMode.AutoSize;

            sub.Dock = DockStyle.Bottom;
            sub.TextAlign = ContentAlignment.MiddleCenter;

            Controls.Add(sub);
            Controls.Add(main);
            Controls.Add(pic);

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = true;
            AutoSize = true;
            ShowInTaskbar = false;

            ResumeLayout(true);
        }
    }
}
