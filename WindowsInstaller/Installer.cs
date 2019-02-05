﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsInstaller
{
    public partial class Installer : Form
    {
        private static readonly bool StandaloneInstaller = true;
        private string Key;
        private byte phase;
        private int count;
        private Image LoadIcon = Properties.Resources.win_load;

        public Installer()
        {
            InitializeComponent();
            UniqueID.Location = new Point(0, (int)(this.Height * .6));
            StatusMessage.Location = new Point(0, UniqueID.Location.Y + 67);
            UniqueID.TextChanged += UniqueID_TextChanged;
            IntroTimer.Tick += IntroTimer_Tick;
            IntroTimer.Start();
        }

        private void UniqueID_TextChanged(object sender, EventArgs e)
        {
            if(UniqueID.MaskFull)
            {
                bool result = KeyMeetsCriteria(UniqueID.Text);
                if (result)
                {
                    StatusMessage.ForeColor = Color.White;
                    StatusMessage.Text = "";
                    UniqueID.ForeColor = Color.LightGreen;
                    UniqueID.ReadOnly = true;
                    StatusMessage.Focus();
                    phase = 3;
                    IntroTimer.Start();
                }
                else
                {
                    StatusMessage.ForeColor = Color.LightCoral;
                    StatusMessage.Text = "Invalid Unique Identifier.";
                }
            }
            else
            {
                StatusMessage.ForeColor = Color.White;
                StatusMessage.Text = "";
            }
        }

        /// <summary>
        /// Precheck for key validation
        /// </summary>
        /// <param name="key">The key to validate</param>
        /// <returns></returns>
        private bool KeyMeetsCriteria(string key)
        {
            int result = 0;
            foreach(char c in key)
            {
                result ^= c;
            }
            return (result % 16) == 0;
        }

        private void IntroTimer_Tick(object sender, EventArgs e)
        {
            switch(phase)
            {
                case 0:
                    IntroTimer.Stop();
                    IntroTimer.Interval = 10;
                    phase = 1;
                    IntroTimer.Start();
                    break;
                case 1:
                    count++;
                    WelcomeLabel.ForeColor = Color.FromArgb(255, (int)FloatLerp(255, 32, (float)count / 50), (int)FloatLerp(255, 32, (float)count / 50), (int)FloatLerp(255, 32, (float)count / 50));
                    if(count >= 50)
                    {
                        count = 0;
                        WelcomeLabel.Text = "Please enter your Unique ID";
                        UniqueID.Visible = true;
                        UniqueID.Focus();
                        phase = 2;
                    }
                    break;
                case 2:
                    count++;
                    WelcomeLabel.ForeColor = Color.FromArgb(255, (int)FloatLerp(32, 255, (float)count / 50), (int)FloatLerp(32, 255, (float)count / 50), (int)FloatLerp(32, 255, (float)count / 50));
                    UniqueID.ForeColor = WelcomeLabel.ForeColor;
                    if (count >= 50)
                    {
                        phase = 3;
                        count = 0;
                        IntroTimer.Stop();
                    }
                    break;
                case 3:
                    count++;
                    WelcomeLabel.ForeColor = Color.FromArgb(255, (int)FloatLerp(255, 32, (float)count / 50), (int)FloatLerp(255, 32, (float)count / 50), (int)FloatLerp(255, 32, (float)count / 50));
                    if (count >= 50)
                    {
                        count = 0;
                        WelcomeLabel.Text = "Just a second...";
                        phase = 4;
                    }
                    break;
                case 4:
                    count++;
                    WelcomeLabel.ForeColor = Color.FromArgb(255, (int)FloatLerp(32, 255, (float)count / 50), (int)FloatLerp(32, 255, (float)count / 50), (int)FloatLerp(32, 255, (float)count / 50));
                    if (count >= 50)
                    {
                        phase = 5;
                        count = 0;
                        IntroTimer.Stop();
                        ValidateID();
                    }
                    break;
                case 5:
                    count++;
                    WelcomeLabel.ForeColor = Color.FromArgb(255, (int)FloatLerp(255, 32, (float)count / 50), (int)FloatLerp(255, 32, (float)count / 50), (int)FloatLerp(255, 32, (float)count / 50));
                    UniqueID.ForeColor = Color.FromArgb(255, (int)FloatLerp(Color.LightGreen.R, 32, (float)count / 50), (int)FloatLerp(Color.LightGreen.G, 32, (float)count / 50), (int)FloatLerp(Color.LightGreen.B, 32, (float)count / 50));
                    if (count >= 50)
                    {
                        phase = 6;
                        count = 0;
                    }
                    break;
            }
        }

        private void ValidateID()
        {
            //for current, we are going to accept the key using offline installer mode
            if(StandaloneInstaller)
            {
                Key = UniqueID.Text;
                phase = 5;
                IntroTimer.Start();
            }

            //if id is good, start the installation
            //else, tell em its bad, clear it, and restart the process
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Linear float interpolation
        /// </summary>
        /// <param name="from">Starting Point</param>
        /// <param name="to">End Point</param>
        /// <param name="alpha">Alpha</param>
        /// <returns></returns>
        private float FloatLerp(float from, float to, float alpha)
        {
            return from * (1 - alpha) + to * alpha;
        }

        private void UniqueID_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
        }
    }
}
