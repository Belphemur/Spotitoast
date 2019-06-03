using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spotitoast.Banner.Client;
using Spotitoast.Logic.Business.Action;

namespace Spotitoast
{
    public partial class Form1 : Form
    {
        private delegate void SetString(string str);

        private IActionFactory _client;
        public Form1(IActionFactory spotifyClient)
        {
            _client = spotifyClient;
            BannerClient.Setup();
            InitializeComponent();
        }

        public void UpdateTrackLabel(string label)
        {
            BeginInvoke(new SetString(SetTrackLabel), label);

        }

        private void SetTrackLabel(string label)
        {
            this.trackLabel.Text = label;
        }

        private void LoveButton_Click(object sender, EventArgs e)
        {
            _client.Get(ActionFactory.PlayerAction.Like).Execute();
        }

        private void UnloveButton_Click(object sender, EventArgs e)
        {
            _client.Get(ActionFactory.PlayerAction.Dislike).Execute();
        }

        private void PlayPauseButton_Click(object sender, EventArgs e)
        {
            _client.Get(ActionFactory.PlayerAction.TogglePlayback).Execute();
        }
    }
}
