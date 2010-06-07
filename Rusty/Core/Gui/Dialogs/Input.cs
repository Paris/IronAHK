using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    class InputDialog : Form
    {
        private Container components;
        private Button btnCancel;
        private Button btnOK;
        private Label label1;
        private TextBox txtMessage;

        private bool _Hide;

        public string Title { get; set; }

        public string Prompt { get; set; }

        public string Default { get; set; }

        public new bool Hide
        {
            get { return _Hide; }
            set
            {
                _Hide = value;
                txtMessage.UseSystemPasswordChar = value;
            }
        }

        public new int Width { get; set; }
        public new int Height { get; set; }
        public int Timeout { get; set; }

        public string Message { get; set; }

        public InputDialog()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
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
            label1 = new Label();
            btnOK = new Button();
            btnCancel = new Button();
            txtMessage = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(20, 20);
            label1.Name = "label1";
            label1.Size = new Size(240, 48);
            label1.TabIndex = 1;
            label1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(16, 104);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(96, 24);
            btnOK.TabIndex = 2;
            btnOK.Text = "OK";
            btnOK.Click += btnOK_Click;
            btnOK.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(152, 104);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(96, 24);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(16, 72);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(232, 20);
            txtMessage.TabIndex = 0;
            txtMessage.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // 
            // DialogForm
            // 
            AutoScaleBaseSize = new Size(5, 13);
            ClientSize = new Size(266, 151);
            ControlBox = false;
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(label1);
            Controls.Add(txtMessage);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputBoxDialog";
            Text = "Inputbox";
            ResumeLayout(false);

        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            Message = txtMessage.Text;
        }
    }
}
