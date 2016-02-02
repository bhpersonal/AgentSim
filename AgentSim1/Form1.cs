using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgentSim1
{
    public partial class Form1 : Form
    {
        private Sim _sim;

        public Form1()
        {
            InitializeComponent();

            button1_Click(null, null);
            panel1.SetDoubleBuffered();
        }

        private Dictionary<CellState, Color> Colors = new Dictionary<CellState, Color>
        {
            {  CellState.Off, Color.Red },
            {  CellState.On, Color.Blue },
            {  CellState.Agent, Color.Gray },
        };

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (_sim == null)
            {
                e.Graphics.FillRectangle(Brushes.ForestGreen, e.Graphics.ClipBounds);
                return;
            }

            e.Graphics.ScaleTransform(
                (float)panel1.Width / (float)_sim.Cells.GetLength(0),
                (float)panel1.Height / (float)_sim.Cells.GetLength(1));
            
            for (var x = 0; x < _sim.Cells.GetLength(0); x++)
            {
                for (var y = 0; y < _sim.Cells.GetLength(1); y++)
                {
                    var cell = _sim.Cells[x, y];
                    var color = Colors[cell];

                    var turnsOld = _sim.Turn - _sim.CellsLastChange[x, y];

                    const int decay = 25;
                    if (turnsOld > decay) turnsOld = decay;
                    var colorStrength = 1f - (turnsOld / (float)decay)* .85f;

                    color = Color.FromArgb((int)((float)color.R * colorStrength), (int)((float)color.G * colorStrength), (int)((float)color.B * colorStrength));

                    e.Graphics.FillRectangle(new SolidBrush(color), new Rectangle(x, y, 1, 1));
                } 
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _sim.DoTurn();
            panel1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _sim = SimFactory.CreateRandomSim();
            panel1.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_sim == null) return;
            _sim.DoTurn();
            panel1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
                timer1.Stop();
            else
                timer1.Start();


        }

        private void button4_Click(object sender, EventArgs e)
        {
            _sim.DoTurn();
            panel1.Refresh();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            timer1.Interval = 1000 / hScrollBar1.Value;
        }
    }
}
