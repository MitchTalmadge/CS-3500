﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SpaceWars.Properties;

namespace SpaceWars
{
    /// <inheritdoc />
    /// <summary>
    /// The game window for the space wars program, which is displayed once connection to a server has been established.
    /// </summary>
    /// <authors>Jiahui Chen, Mitch Talmadge</authors>
    public partial class GameForm : Form
    {
        /// <summary>
        /// The Space Wars instance being played.
        /// </summary>
        private readonly SpaceWars _spaceWars;

        /// <summary>
        /// The panel that the game ultimately takes place on. Ships, stars, etc. are drawn here.
        /// </summary>
        private WorldPanel _worldPanel;

        /// <summary>
        /// The panel that the players' scores appear on.
        /// </summary>
        private ScoreboardPanel _scoreboardPanel;

        /// <summary>
        /// The mp3 player for the background music
        /// </summary>
        private Mp3Player _mp3Player;

        /// <inheritdoc />
        /// <summary>
        /// Creates a new Game Form that is based on the given Space Wars instance.
        /// </summary>
        /// <param name="spaceWars">The connected Space Wars instance.</param>
        public GameForm(SpaceWars spaceWars)
        {
            _spaceWars = spaceWars;

            InitializeComponent();

            CreateWorldPanel();

            CreateScoreboardPanel();

            StartMusic();

            _spaceWars.OnGameComponentsUpdated += () =>
            {
                _worldPanel.DrawGameComponents(GetGameComponentsToDraw());
            };
        }

        private IEnumerable<GameComponent> GetGameComponentsToDraw()
        {
            foreach (var ship in _spaceWars.Ships)
            {
                yield return ship;
            }

            //TODO: Yield return other things in the order they should be drawn.
        }

        /// <summary>
        /// Creates the World Panel that the game is played on.
        /// </summary>
        private void CreateWorldPanel()
        {
            _worldPanel = new WorldPanel
            {
                Margin = new Padding(10),
                Location = new Point(10, 10),
                Size = new Size(_spaceWars.WorldSize, _spaceWars.WorldSize),
                Parent = _mainLayoutPanel
            };

            _mainLayoutPanel.SetCellPosition(_worldPanel, new TableLayoutPanelCellPosition(0, 0));
        }

        /// <summary>
        /// Creates the Scoreboard Panel that the players' scores appear on.
        /// </summary>
        private void CreateScoreboardPanel()
        {
            _scoreboardPanel = new ScoreboardPanel
            {
                Margin = new Padding(10),
                Location = new Point(10, 10),
                Dock = DockStyle.Fill,
                Parent = _mainLayoutPanel
            };

            _mainLayoutPanel.SetCellPosition(_scoreboardPanel, new TableLayoutPanelCellPosition(1, 0));
        }

        /// <summary>
        /// Plays the background music on a loop.
        /// </summary>
        private void StartMusic()
        {
            _mp3Player = new Mp3Player(Resources.game_music);
            _mp3Player.StartPlaying();
        }

        /// <summary>
        /// Stops the background music.
        /// </summary>
        private void StopMusic()
        {
            _mp3Player.StopPlaying();
        }

        /// <summary>
        /// Centers the window onscreen whenever the size is updated (which can only happen automatically, since the user cannot resize the window).
        /// </summary>
        private void GameForm_Resize(object sender, EventArgs e)
        {
            CenterToScreen();
        }

        /// <summary>
        /// Loads the main menu when the game window is closed.
        /// </summary>
        private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            OpenMainMenu();
        }

        /// <summary>
        /// Disconnects from the server and opens the main menu.
        /// </summary>
        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            OpenMainMenu();
        }

        /// <summary>
        /// Opens the Main Menu window and disposes this window.
        /// </summary>
        private void OpenMainMenu()
        {
            _spaceWars.Disconnect();
            StopMusic();

            new MainMenuForm().Show();
            Dispose();
        }
    }
}