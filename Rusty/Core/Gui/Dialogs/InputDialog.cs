using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using IronAHK.Rusty.Common;

namespace IronAHK.Rusty
{
    class InputDialog : Form
    {
        #region Fields

        private Button btnCancel;
        private Button btnOK;
        private Label prompt;
        private TextBox txtMessage;

        private bool _hide = false;

        #endregion

        #region ctor

        public InputDialog() {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
        }

        #region InitializeComponent

        private void InitializeComponent() {
            prompt = new Label();
            btnOK = new Button();
            btnCancel = new Button();
            txtMessage = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            prompt.Location = new Point(20, 20);
            prompt.Size = new Size(240, 48);
            prompt.TabIndex = 1;
            prompt.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(16, 104);
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
            btnCancel.Size = new Size(96, 24);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(16, 72);
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
            Controls.Add(prompt);
            Controls.Add(txtMessage);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Text = "Inputbox";
            ResumeLayout(false);
        }

        #endregion

        #endregion

        #region Properties

        public string Title {
            get { return this.Text; }
            set {
                GuiInvoker.SetText(this, value);
            }
        }

        public string Prompt {
            get { return prompt.Text; } 
            set {
                GuiInvoker.SetText(prompt, value);
            } 
        }

        public string Default { get; set; }

        public new bool Hide
        {
            get { return _hide; }
            set
            {
                _hide = value;
                txtMessage.UseSystemPasswordChar = value;
            }
        }

        public new int Width { get; set; }
        public new int Height { get; set; }
        public int Timeout { get; set; }

        public string Message {
            get { return txtMessage.Text; }
            set { GuiInvoker.SetText(txtMessage, value); }
        }

        #endregion

        protected void btnOK_Click(object sender, EventArgs e)
        {
            Message = txtMessage.Text;
        }
    }
}
