using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _36Cube
{
    public class Form1 : Form
    {
        private const int rowCount = 6;
        private const int colCount = 6;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem drawTrialsToolStripMenuItem;
        private ToolStripMenuItem gOToolStripMenuItem;
        private Panel pnlBlocks;
        private ComboBox ddlSolutions;
        private Panel pnlSolutions;

        public List<Block[,]> Solutions = new List<Block[,]>();
        private ToolStripMenuItem sTOPToolStripMenuItem;

        private bool stop = false;

        public Form1()
        {
            InitializeComponent();

            this.ResizeEnd += Form1_ResizeEnd;
            this.FormClosing += (s, e) => { stop = true; };
            BuildLabels(rowCount, colCount);
        }

        #region loop
        private void FindSolution()
        {
            Birth(new Node());
        }

        public bool Birth(Node Input)
        {
            if (stop) { return false; }
            if (drawTrialsToolStripMenuItem.Checked) { DrawTrials(Input); }

            if (Input.isSolution) {
                Solutions.Add(Utility.GetBoardState(Input));
                this.BeginInvoke((MethodInvoker)delegate() { this.Text = "Solutions Found:" + Solutions.Count.ToString(); });
                ddlSolutions.BeginInvoke((MethodInvoker)delegate() { ddlSolutions.Items.Add("Solution:" + Solutions.Count); });

                return true; 
            }

            IEnumerable<Block> Blocks = Utility.GetNextBlockOptions(Input);
            foreach (Block B in Blocks)
            {
                Input.Children.Add(new Node(Input, B));
            }

            bool output = false;
            for(int i =0; i< Input.Children.Count; i++){
                Node N = Input.Children[i];

                output &= Birth(N);
                if (output) { return output; }
            }

            if (Input.Children.Count == 0 && !Input.isSolution) { Input.Dispose(); }

            return output;
        }
        #endregion

        #region ui stuff
        private void BuildLabels(int rows, int cols)
        {
            int lblWidth = pnlBlocks.Width / colCount;
            int lblHeight = pnlBlocks.Height / rowCount;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Label newLabel = new Label()
                    {
                        Text = string.Empty,
                        Size = new Size(lblWidth, lblHeight),
                        Location = new Point(j * lblWidth, i * lblHeight),
                        Name = string.Format("lbl{1}_{0}", i, j),
                        BorderStyle = BorderStyle.FixedSingle,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    newLabel.Click += newLabel_Click;
                    pnlBlocks.Controls.Add(newLabel);
                }
            }

            lblWidth = pnlSolutions.Width / colCount;
            lblHeight = pnlSolutions.Height / rowCount;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Label newLabel = new Label()
                    {
                        Text = string.Empty,
                        Size = new Size(lblWidth, lblHeight),
                        Location = new Point(j * lblWidth, i * lblHeight),
                        Name = string.Format("lbl{1}_{0}", i, j),
                        BorderStyle = BorderStyle.FixedSingle,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    newLabel.Click += newLabel_Click;
                    pnlSolutions.Controls.Add(newLabel);
                }
            }
        }
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            int lblWidth = pnlBlocks.Width / colCount;
            int lblHeight = pnlBlocks.Height / rowCount;

            foreach (Control C in pnlBlocks.Controls)
            {
                string[] t = C.Name.Replace("lbl", "").Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int x = int.Parse(t[0]) * lblWidth;
                int y = int.Parse(t[1]) * lblHeight;

                C.Location = new Point(x, y);
                C.Size = new Size(lblWidth, lblHeight);
            }

            lblWidth = pnlSolutions.Width / colCount;
            lblHeight = pnlSolutions.Height / rowCount;

            foreach (Control C in pnlSolutions.Controls)
            {
                if (!(C is Label)) { continue; }
                string[] t = C.Name.Replace("lbl", "").Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int x = int.Parse(t[0]) * lblWidth;
                int y = int.Parse(t[1]) * lblHeight;

                C.Location = new Point(x, y);
                C.Size = new Size(lblWidth, lblHeight);
            }
        }

        private void BindSolutions()
        {
            ddlSolutions.Items.Clear();
            for (int i = 0; i < Solutions.Count; i++)
            {
                ddlSolutions.Items.Add("Solution:" + i);
            }
        }
        private void DrawTrials(Node Input)
        {
            Block[,] bs = Utility.GetBoardState(Input);

            foreach (Control C in pnlBlocks.Controls)
            {
                string[] t = C.Name.Replace("lbl", "").Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int x = int.Parse(t[0]);
                int y = int.Parse(t[1]);

                C.BackColor = bs[x,y]._Color;
                C.BeginInvoke((MethodInvoker)delegate() { C.Text = bs[x,y]._Height.ToString();});
            }
           System.Threading.Thread.Sleep(16);
        }
        private void DrawSolution(Block[,] Input)
        {
            foreach (Control C in pnlSolutions.Controls)
            {
                string[] t = C.Name.Replace("lbl", "").Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int x = int.Parse(t[0]);
                int y = int.Parse(t[1]);

                C.BackColor = Input[x, y]._Color;
                C.BeginInvoke((MethodInvoker)delegate() { C.Text = Input[x, y]._Height.ToString(); });
            }
        }

        private void ddlSolutions_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawSolution(Solutions[ddlSolutions.SelectedIndex]);
        }
        private void sTOPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gOToolStripMenuItem.Enabled = true;
            sTOPToolStripMenuItem.Enabled = false;
            stop = true;
        }
        private void gOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Text = "Solutions Found:0";
            sTOPToolStripMenuItem.Enabled = true;
            gOToolStripMenuItem.Enabled = false;
            stop = false;
            Solutions.Clear();
            ddlSolutions.Items.Clear();

            DateTime Start = DateTime.Now;

            Task T = Task.Run(() =>
            {
                FindSolution();
                TimeSpan duration = DateTime.Now - Start;
                MessageBox.Show("RunTime: " + duration.ToString());

                BindSolutions();
                sTOPToolStripMenuItem.Enabled = false;
                gOToolStripMenuItem.Enabled = true;
            });
        }
        void newLabel_Click(object sender, EventArgs e)
        {
            MessageBox.Show((sender as Control).Name);
        }


        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pnlBlocks = new System.Windows.Forms.Panel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pnlSolutions = new System.Windows.Forms.Panel();
            this.ddlSolutions = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawTrialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sTOPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(284, 238);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pnlBlocks);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(276, 212);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Trials";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pnlBlocks
            // 
            this.pnlBlocks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBlocks.Location = new System.Drawing.Point(3, 3);
            this.pnlBlocks.Name = "pnlBlocks";
            this.pnlBlocks.Size = new System.Drawing.Size(270, 206);
            this.pnlBlocks.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pnlSolutions);
            this.tabPage2.Controls.Add(this.ddlSolutions);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(276, 212);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Solutions";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pnlSolutions
            // 
            this.pnlSolutions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSolutions.Location = new System.Drawing.Point(3, 24);
            this.pnlSolutions.Name = "pnlSolutions";
            this.pnlSolutions.Size = new System.Drawing.Size(270, 185);
            this.pnlSolutions.TabIndex = 1;
            // 
            // ddlSolutions
            // 
            this.ddlSolutions.Dock = System.Windows.Forms.DockStyle.Top;
            this.ddlSolutions.FormattingEnabled = true;
            this.ddlSolutions.Location = new System.Drawing.Point(3, 3);
            this.ddlSolutions.Name = "ddlSolutions";
            this.ddlSolutions.Size = new System.Drawing.Size(270, 21);
            this.ddlSolutions.TabIndex = 0;
            this.ddlSolutions.SelectedIndexChanged += new System.EventHandler(this.ddlSolutions_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(284, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gOToolStripMenuItem,
            this.sTOPToolStripMenuItem,
            this.drawTrialsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // gOToolStripMenuItem
            // 
            this.gOToolStripMenuItem.Name = "gOToolStripMenuItem";
            this.gOToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.gOToolStripMenuItem.Text = "GO";
            this.gOToolStripMenuItem.Click += new System.EventHandler(this.gOToolStripMenuItem_Click);
            // 
            // drawTrialsToolStripMenuItem
            // 
            this.drawTrialsToolStripMenuItem.Checked = true;
            this.drawTrialsToolStripMenuItem.CheckOnClick = true;
            this.drawTrialsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.drawTrialsToolStripMenuItem.Name = "drawTrialsToolStripMenuItem";
            this.drawTrialsToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.drawTrialsToolStripMenuItem.Text = "Draw Trials";
            // 
            // sTOPToolStripMenuItem
            // 
            this.sTOPToolStripMenuItem.Enabled = false;
            this.sTOPToolStripMenuItem.Name = "sTOPToolStripMenuItem";
            this.sTOPToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.sTOPToolStripMenuItem.Text = "STOP";
            this.sTOPToolStripMenuItem.Click += new System.EventHandler(this.sTOPToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


    }
}
