using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AVRTimerCalculator
{
    public partial class Form1 : Form
    {
        double ClkFreq = 8000000;
        double ActualFreq = 0;
        double TotalTicks = 0;
        double Overflows = 0;
        double Remainder = 0;
        double RealTime = 0;

        public Form1()
        {
            InitializeComponent();

            TimerResDD.SelectedIndex = 0;
            PrescalerDD.SelectedIndex = 0;

            UpdateView();
        }

        private void UpdateView()
        {
            ClkFreqTB.Text = string.Format("{0}", ClkFreq);
            TotalTicksTB.Text = string.Format("{0}", TotalTicks);
            OverflowsTB.Text = string.Format("{0}", Overflows);
            RemainderTB.Text = string.Format("{0}", Remainder);
            RealTimeTB.Text = string.Format("{0:F9}", RealTime);
        }

        private void UseTotalTicksBut_Click(object sender, EventArgs e)
        {
            try
            {
                ClkFreq = Convert.ToDouble(ClkFreqTB.Text);
                TotalTicks = Convert.ToDouble(TotalTicksTB.Text);
            }
            catch
            {
                MessageBox.Show("Bad Input");
            }
            finally
            {
                double divider = 1;
                switch(PrescalerDD.Text)
                {
                    case "(1) Clk/1":
                        divider = 1;
                        break;
                    case "(2) Clk/8":
                        divider = 8;
                        break;
                    case "(3) Clk/64":
                        divider = 64;
                        break;
                    case "(4) Clk/256":
                        divider = 256;
                        break;
                    case "(5) Clk/1024":
                        divider = 1024;
                        break;
                }
                ActualFreq = ClkFreq / divider;
                if (TimerResDD.Text == "8 bit")
                {
                    Overflows = Math.Floor(TotalTicks / Math.Pow(2, 8));
                    Remainder = TotalTicks - (Overflows * Math.Pow(2, 8));
                }
                else if (TimerResDD.Text == "16 bit")
                {
                    Overflows = Math.Floor(TotalTicks / Math.Pow(2, 16));
                    Remainder = TotalTicks - (Overflows * Math.Pow(2, 16));
                }
                RealTime = TotalTicks / ActualFreq;
                UpdateView();
            }
        }

        private void UseOverflowRemainderBut_Click(object sender, EventArgs e)
        {
            try
            {
                ClkFreq = Convert.ToDouble(ClkFreqTB.Text);
                Overflows = Math.Floor(Convert.ToDouble(OverflowsTB.Text));
                Remainder = Convert.ToDouble(RemainderTB.Text);
            }
            catch
            {
                MessageBox.Show("Bad Input");
            }
            finally
            {
                double divider = 1;
                switch (PrescalerDD.Text)
                {
                    case "(1) Clk/1":
                        divider = 1;
                        break;
                    case "(2) Clk/8":
                        divider = 8;
                        break;
                    case "(3) Clk/64":
                        divider = 64;
                        break;
                    case "(4) Clk/256":
                        divider = 256;
                        break;
                    case "(5) Clk/1024":
                        divider = 1024;
                        break;
                }
                ActualFreq = ClkFreq / divider;
                if (TimerResDD.Text == "8 bit")
                {
                    TotalTicks = Overflows * Math.Pow(2, 8) + Remainder;
                }
                else if (TimerResDD.Text == "16 bit")
                {
                    TotalTicks = Overflows * Math.Pow(2, 16) + Remainder;
                }
                RealTime = TotalTicks / ActualFreq;
                UpdateView();
            }
        }

        private void UseRealTimeBut_Click(object sender, EventArgs e)
        {
            try
            {
                ClkFreq = Convert.ToDouble(ClkFreqTB.Text);
                RealTime = Convert.ToDouble(RealTimeTB.Text);
            }
            catch
            {
                MessageBox.Show("Bad Input");
                ClkFreq = 0;
                TotalTicks = 0;
            }
            finally
            {
                double divider = 1;
                switch (PrescalerDD.Text)
                {
                    case "(1) Clk/1":
                        divider = 1;
                        break;
                    case "(2) Clk/8":
                        divider = 8;
                        break;
                    case "(3) Clk/64":
                        divider = 64;
                        break;
                    case "(4) Clk/256":
                        divider = 256;
                        break;
                    case "(5) Clk/1024":
                        divider = 1024;
                        break;
                }
                ActualFreq = ClkFreq / divider;
                TotalTicks = RealTime * ActualFreq;
                if (TimerResDD.Text == "8 bit")
                {
                    Overflows = Math.Floor(TotalTicks / Math.Pow(2, 8));
                    Remainder = TotalTicks - (Overflows * Math.Pow(2, 8));
                }
                else if (TimerResDD.Text == "16 bit")
                {
                    Overflows = Math.Floor(TotalTicks / Math.Pow(2, 16));
                    Remainder = TotalTicks - (Overflows * Math.Pow(2, 16));
                }
                UpdateView();
            }
        }

        private void TimerResDD_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TimerResDD.Text == "8 bit")
            {
                Overflows = Math.Floor(TotalTicks / Math.Pow(2, 8));
                Remainder = TotalTicks - (Overflows * Math.Pow(2, 8));
            }
            else if (TimerResDD.Text == "16 bit")
            {
                Overflows = Math.Floor(TotalTicks / Math.Pow(2, 16));
                Remainder = TotalTicks - (Overflows * Math.Pow(2, 16));
            }
            UpdateView();
        }
    }
}
