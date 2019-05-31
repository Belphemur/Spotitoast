/********************************************************************
* Copyright (C) 2015-2017 Antoine Aflalo
*
* This program is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; either version 2
* of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
********************************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spotitoast.Banner.Model;
using Timer = System.Windows.Forms.Timer;

namespace Spotitoast.Banner.Display
{
    /// <summary>
    /// This class implements the UI form used to show a Banner notification.
    /// </summary>
    public partial class BannerForm : Form
    {
        private Timer _timerHide;
        private bool _hiding;


        /// <summary>
        /// Constructor for the <see cref="BannerForm"/> class
        /// </summary>
        public BannerForm()
        {
            InitializeComponent();

            MouseEnter += (sender, args) => { _timerHide?.Stop(); };

            MouseLeave += (sender, args) =>
            {
                if (ClientRectangle.Contains(PointToClient(Control.MousePosition))) return;
                TimerHide_Tick(this, EventArgs.Empty);
            };

            Location = new Point(50, 60);
        }

        protected override bool ShowWithoutActivation => true;

        /// <summary>
        /// Override the parameters used to create the window handle.
        /// Ensure that the window will be top-most and do not activate or take focus.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var @params = base.CreateParams;
                @params.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                @params.ExStyle |= 0x00000008; // WS_EX_TOPMOST
                return @params;
            }
        }

        /// <summary>
        /// Called internally to configure pass notification parameters
        /// </summary>
        /// <param name="data">The configuration data to setup the notification UI</param>
        internal void SetData(BannerData data)
        {
            if (_timerHide == null)
            {
                _timerHide = new Timer {Interval = Convert.ToInt32(data.TimeOnScreen.TotalMilliseconds)};
                _timerHide.Tick += TimerHide_Tick;
            }
            else
            {
                _timerHide.Enabled = false;
            }

            if (data.Image != null)
            {
                pbxLogo.Image = data.Image;
            }


            _hiding = false;
            Opacity = .8;
            lblTop.Text = data.Title;
            lblTitle.Text = data.Text;
            lblSubtitle.Text = data.SubText;

            _timerHide.Enabled = true;

            Show();
        }


        /// <summary>
        /// Event handler for the "hiding" timer.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Arguments of the event</param>
        private void TimerHide_Tick(object sender, EventArgs e)
        {
            _hiding = true;
            _timerHide.Enabled = false;
            FadeOut();
        }

        /// <summary>
        /// Implements an "fadeout" animation while hiding the window.
        /// In the end of the animation the form is self disposed.
        /// <remarks>The animation is canceled if the method <see cref="SetData"/> is called along the animation.</remarks>
        /// </summary>
        private async void FadeOut()
        {
            while (Opacity > 0.0)
            {
                await Task.Delay(50);

                if (!_hiding)
                    break;
                Opacity -= 0.05;
            }

            if (_hiding)
            {
                Dispose();
            }
        }
    }
}