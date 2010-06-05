using System;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    class InputDialog : System.Windows.Forms.Form
    {
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMessage;

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
                this.txtMessage.UseSystemPasswordChar = value;
            }
        }

        public new int Width { get; set; }
        public new int Height { get; set; }
        public int Timeout { get; set; }

        public string Message { get; set; }

        public InputDialog()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 48);
            this.label1.TabIndex = 1;
            this.label1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(16, 104);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(96, 24);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            this.btnOK.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(152, 104);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 24);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(16, 72);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(232, 20);
            this.txtMessage.TabIndex = 0;
            this.txtMessage.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // 
            // DialogForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(266, 151);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputBoxDialog";
            this.Text = "Inputbox";
            this.ResumeLayout(false);

        }

        protected void btnOK_Click(object sender, System.EventArgs e)
        {
            Message = txtMessage.Text;
        }
    }
}
