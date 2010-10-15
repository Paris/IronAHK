using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    class SplashDialog : Form, IComplexDialoge
    {


        #region Controls

        private TableLayoutPanel layoutTable;
        private Label main, sub;
        private PictureBox pic;

        #endregion

        #region Constructor

        public SplashDialog() {
            InitializeComponent();
        }


        private void InitializeComponent() {

            #region Controls

            layoutTable = new TableLayoutPanel();

            main = new Label();
            sub = new Label();
            pic = new PictureBox();

            #endregion

            SuspendLayout();

            //
            // Layout Table
            //
            layoutTable.ColumnCount = 1;
            layoutTable.RowCount = 3;
            layoutTable.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
            layoutTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutTable.AutoSize = true;
            layoutTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;


            //
            // Main Text
            //
            main.Dock = DockStyle.Fill;
            main.Font = new Font(main.Font, FontStyle.Bold);
            main.TextAlign = ContentAlignment.MiddleCenter;
            main.Size = new Size(100, 20);
            main.AutoSize = true;
            main.Location = new Point(0, 30);

            //
            // Pictrue Control
            //
            pic.Dock = DockStyle.Fill;
            pic.SizeMode = PictureBoxSizeMode.AutoSize;
            pic.Size = new Size(100, 20);
            pic.AutoSize = true;
            pic.Location = new Point(0, 40);


            //
            // Sub Text
            //
            sub.Dock = DockStyle.Fill;
            sub.TextAlign = ContentAlignment.MiddleCenter;
            sub.Size = new Size(100, 20);
            sub.AutoSize = true;
            sub.Location = new Point(0, 150);


            layoutTable.Controls.Add(main, 0, 0);
            layoutTable.Controls.Add(pic, 0, 1);
            layoutTable.Controls.Add(sub, 0, 2);

            Controls.Add(layoutTable);


            this.Size = new System.Drawing.Size(500, 400);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = true;
            ShowInTaskbar = false;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;


            ResumeLayout(true);
        }
        #endregion

        #region IComplexDialogeProperties

        public string Title {
            get { return Text; }
            set { Text = value; }
        }

        public string Subtext {
            get { return sub.Text; }
            set {
                sub.Text = value;
                sub.Visible = !string.IsNullOrEmpty(sub.Text);
            }
        }

        public string MainText {
            get { return main.Text; }
            set {
                main.Text = value;
                main.Visible = !string.IsNullOrEmpty(main.Text);
            }
        }
        #endregion

        #region Public Properties

        public Image Image {
            get { return pic.Image; }
            set {
                pic.Image = value;
            }
        }

        #endregion

    }
}
