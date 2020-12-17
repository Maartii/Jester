using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks; 
using System;

namespace Impostor.Plugins.Example.Handlers
{
    /// <summary>
    ///     A class that listens for two events.
    ///     It may be more but this is just an example.
    ///
    ///     Make sure your class implements <see cref="IEventListener"/>.
    /// </summary>


    public class GameEventListener : IEventListener
    {
        private readonly ILogger<ExamplePlugin> _logger;

        static System.Random rnd = new System.Random();

        struct JesterGame
        {
            public int JesterClientId;
            public bool JesterOn;
            public bool Jesterwin;
            public bool GameEnded;
            public bool CountingDown;
            public bool JesterInGame;
            public string Jestername;
        }
    
        Dictionary<string, JesterGame> JesterGames = new Dictionary<string, JesterGame>();

        public GameEventListener(ILogger<ExamplePlugin> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     An example event listener.
        /// </summary>
        /// <param name="e">
        ///     The event you want to listen for.
        /// </param>

        private async Task ServerSendChatToPlayerAsync(string text, IInnerPlayerControl player)
        {
            string playername = player.PlayerInfo.PlayerName;
            await player.SetNameAsync($"PrivateMsg").ConfigureAwait(false);
            await player.SendChatToPlayerAsync($"{text}", player).ConfigureAwait(false);
            await player.SetNameAsync(playername);
        }

        private async Task ServerSendChatAsync(string text, IInnerPlayerControl player)
        {
            string playername = player.PlayerInfo.PlayerName;
            await player.SetNameAsync($"PublicMsg").ConfigureAwait(false);
            await player.SendChatAsync($"{text}").ConfigureAwait(false);
            await player.SetNameAsync(playername);
        }  

        [EventListener]
        public void OnGameCreated(IGameCreatedEvent e)
        {
            JesterGame jgame = new JesterGame();
            jgame.JesterOn = true;
            jgame.Jesterwin = false;
            jgame.GameEnded = false;
            jgame.CountingDown = false;
            jgame.JesterInGame = false;
            JesterGames.Add(e.Game.Code, jgame);
        }

        [EventListener]
        public void OnSetStartCounter(IPlayerSetStartCounterEvent e)
        {
            if (e.SecondsLeft == 5)
            {
                JesterGame jgame = JesterGames[e.Game.Code];
                jgame.CountingDown = true;
                JesterGames[e.Game.Code] = jgame;

                _logger.LogInformation($"Countdown started.");
                if (JesterGames[e.Game.Code].JesterOn)
                {
                    Task.Run(async () => await AssignJester(e).ConfigureAwait(false));
                    foreach (var player in e.Game.Players)
                    {
                        Task.Run(async () => await MakePlayerLookAtChat(player).ConfigureAwait(false));
                    }
                }
            }
        }      

        private async Task AssignJester(IPlayerSetStartCounterEvent e)
        {
            List<IClientPlayer> gameplayers = new List<IClientPlayer>();
            foreach (var player in e.Game.Players)
            {
                gameplayers.Add(player);
            }
            int r = rnd.Next(gameplayers.Count);

            JesterGame jgame = JesterGames[e.Game.Code];
            jgame.JesterClientId = gameplayers[r].Client.Id;
            jgame.Jestername = gameplayers[r].Character.PlayerInfo.PlayerName;
            JesterGames[e.Game.Code] = jgame;

            await ServerSendChatToPlayerAsync($"You're the JESTER! (unless you'll be an imposter)", gameplayers[r].Character).ConfigureAwait(false);
            _logger.LogInformation($"- {JesterGames[e.Game.Code].Jestername} is probably the jester.");    
        }

        private async Task MakePlayerLookAtChat(IClientPlayer player)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            string playername = player.Character.PlayerInfo.PlayerName;
            await player.Character.SetNameAsync($"OPEN CHAT").ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
            await player.Character.SetNameAsync(playername).ConfigureAwait(false);
        }

        [EventListener]
        public void OnGamePlayerJoined(IGamePlayerJoinedEvent e)
        {
            if (JesterGames[e.Game.Code].CountingDown)
            {
                _logger.LogInformation($"Assigning Jester interrupted. {JesterGames[e.Game.Code].Jestername} is NOT the jester.");
                Task.Run(async () => await SorryNotJester(e.Game.GetClientPlayer(JesterGames[e.Game.Code].JesterClientId)).ConfigureAwait(false));

                JesterGame jgame = JesterGames[e.Game.Code];
                jgame.CountingDown = false;
                JesterGames[e.Game.Code] = jgame;
            }
        }

        [EventListener]
        public void OnGamePlayerLeft(IGamePlayerLeftEvent e)
        {
            if (JesterGames[e.Game.Code].CountingDown)
            {
                _logger.LogInformation($"Assigning Jester interrupted. {JesterGames[e.Game.Code].Jestername} is NOT the jester.");
                Task.Run(async () => await SorryNotJester(e.Game.GetClientPlayer(JesterGames[e.Game.Code].JesterClientId)).ConfigureAwait(false));

                JesterGame jgame = JesterGames[e.Game.Code];
                jgame.CountingDown = false;
                JesterGames[e.Game.Code] = jgame;
            }
        }

        private async Task SorryNotJester(IClientPlayer player)
        {
            await ServerSendChatToPlayerAsync($"Startup interrupted. You're NOT the Jester.", player.Character).ConfigureAwait(false);
        }        

        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            JesterGame jgame = JesterGames[e.Game.Code];
            jgame.GameEnded = false;
            jgame.CountingDown = false;
            jgame.JesterInGame = false;
            jgame.Jesterwin = false;
            JesterGames[e.Game.Code] = jgame;

            _logger.LogInformation($"Game is starting.");
            if (JesterGames[e.Game.Code].JesterOn)
            {
                Task.Run(async () => await InformJester(e).ConfigureAwait(false));
            }            
            // This prints out for all players if they are impostor or crewmate.
            foreach (var player in e.Game.Players)
            {
                if (JesterGames[e.Game.Code].JesterOn && (player.Character.PlayerInfo.HatId == 27 || player.Character.PlayerInfo.HatId == 84))
                {
                    Task.Run(async () => await OffWithYourHat(player).ConfigureAwait(false));
                }                
                var info = player.Character.PlayerInfo;
                var isImpostor = info.IsImpostor;
                if (isImpostor)
                {
                    _logger.LogInformation($"- {info.PlayerName} is an impostor.");
                }
                else
                {
                    _logger.LogInformation($"- {info.PlayerName} is a crewmate.");
                }
            }
        }

        private async Task InformJester(IGameStartedEvent e)
        {
            if (e.Game.GetClientPlayer(JesterGames[e.Game.Code].JesterClientId).Character.PlayerInfo.IsImpostor)
            {
                _logger.LogInformation($"- {e.Game.GetClientPlayer(JesterGames[e.Game.Code].JesterClientId).Character.PlayerInfo.PlayerName} isn't jester but impostor.");
                await ServerSendChatToPlayerAsync($"You happen to be IMPOSTER! No Jester this game.", e.Game.GetClientPlayer(JesterGames[e.Game.Code].JesterClientId).Character).ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation($"- {e.Game.GetClientPlayer(JesterGames[e.Game.Code].JesterClientId).Character.PlayerInfo.PlayerName} is indeed jester.");
                await ServerSendChatToPlayerAsync($"You're indeed the JESTER!", e.Game.GetClientPlayer(JesterGames[e.Game.Code].JesterClientId).Character).ConfigureAwait(false);

                JesterGame jgame = JesterGames[e.Game.Code];
                jgame.JesterInGame = true;
                JesterGames[e.Game.Code] = jgame;
            }
        }

        private async Task OffWithYourHat(IClientPlayer player)
        {
            await player.Character.SetHatAsync(HatType.NoHat).ConfigureAwait(false);
        }

        [EventListener]
        public void OnPlayerExiled(IPlayerExileEvent e)
        {
            if (JesterGames[e.Game.Code].JesterInGame && e.PlayerControl == e.Game.GetClientPlayer(JesterGames[e.Game.Code].JesterClientId).Character)
            {                                
                JesterGame jgame = JesterGames[e.Game.Code];
                jgame.Jesterwin = true;
                JesterGames[e.Game.Code] = jgame;

                _logger.LogInformation($"Jester has won!");
                Task.Run(async () => await TurnTheTables(e).ConfigureAwait(false));
            }
        }

        private async Task TurnTheTables(IPlayerExileEvent e)
        {
            await e.Game.GetClientPlayer(JesterGames[e.Game.Code].JesterClientId).Character.SetHatAsync(HatType.ElfHat).ConfigureAwait(false);
            foreach (var player in e.Game.Players)
            {
                if (player.Client.Id != JesterGames[e.Game.Code].JesterClientId)
                {
                    await player.Character.SetHatAsync(HatType.DumSticker).ConfigureAwait(false);
                }
            }
            foreach (var player in e.Game.Players)
            {
                if (!player.Character.PlayerInfo.IsDead && player.Character.PlayerInfo.IsImpostor)
                {
                    _logger.LogInformation($"- {player.Character.PlayerInfo.PlayerName} is murdered by plugin.");
                    await player.Character.SetMurderedByAsync(player);                          
                }
            }
                      
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            _logger.LogInformation($"Game has ended.");

            JesterGame jgame = JesterGames[e.Game.Code];
            jgame.GameEnded = true;
            JesterGames[e.Game.Code] = jgame;
        }

        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent e)
        {
            if (JesterGames[e.Game.Code].GameEnded && JesterGames[e.Game.Code].JesterInGame)
            {               
                Task.Run(async () => await JesterAnnouncement(e).ConfigureAwait(false));
            }
        }

        private async Task JesterAnnouncement(IPlayerSpawnedEvent e)
        {
            if (JesterGames[e.Game.Code].Jesterwin)
            {
                await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                await ServerSendChatToPlayerAsync($"{JesterGames[e.Game.Code].Jestername} was Jester and won by getting ejected!", e.PlayerControl).ConfigureAwait(false);
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                await ServerSendChatToPlayerAsync($"{JesterGames[e.Game.Code].Jestername} was Jester, but didn't get voted out.", e.PlayerControl).ConfigureAwait(false);                
            }
        }

        [EventListener]
        public void OnGameDestroyed(IGameDestroyedEvent e)
        {
            JesterGames.Remove(e.Game.Code);
        }

        [EventListener]
        public void OnPlayerChat(IPlayerChatEvent e)
        {
            _logger.LogInformation($"{e.PlayerControl.PlayerInfo.PlayerName} said {e.Message}");
            if (e.Game.GameState == GameStates.NotStarted && !JesterGames[e.Game.Code].CountingDown && e.Message.StartsWith("/"))
            {
                Task.Run(async () => await RunCommands(e).ConfigureAwait(false));
            }
        }

        private async Task RunCommands(IPlayerChatEvent e)
        {
            switch (e.Message.ToLowerInvariant())
            {
                case "/j on":
                case "/jester on":
                    if (e.ClientPlayer.IsHost)
                    {
                        JesterGame jgame = JesterGames[e.Game.Code];
                        jgame.JesterOn = true;
                        JesterGames[e.Game.Code] = jgame;

                        await ServerSendChatAsync("The Jester role is now on!", e.PlayerControl).ConfigureAwait(false);
                    }
                    else
                    {
                        await ServerSendChatAsync("You need to be host to change roles.", e.PlayerControl).ConfigureAwait(false);
                    }
                    break;
                case "/j off":
                case "/jester off":
                    if (e.ClientPlayer.IsHost)
                    {
                        JesterGame jgame = JesterGames[e.Game.Code];
                        jgame.JesterOn = false;
                        JesterGames[e.Game.Code] = jgame;

                        await ServerSendChatAsync("The Jester role is now off!", e.PlayerControl).ConfigureAwait(false);
                    }
                    else
                    {
                        await ServerSendChatAsync("You need to be host to change roles.", e.PlayerControl).ConfigureAwait(false);
                    }
                    break;
                case "/j help":
                case "/jester help":
                    await ServerSendChatAsync("When the special Jester role is on, one crewmate is Jester.", e.PlayerControl).ConfigureAwait(false);  
                    await ServerSendChatAsync("In addition to the ways in which a normal crewmate can win, the Jester can win by getting voted out.", e.PlayerControl).ConfigureAwait(false);
                    await ServerSendChatAsync("If this happens all the other players lose, so be careful who you vote during meetings!", e.PlayerControl).ConfigureAwait(false);
                    await ServerSendChatAsync("The host can turn the Jester role on and off by typing '/jester on' or '/jester off'.", e.PlayerControl).ConfigureAwait(false);
                    break;
                default:
                    await ServerSendChatAsync("Error. Possible commands are '/jester help', '/jester on', '/jester off'.", e.PlayerControl).ConfigureAwait(false);  
                    break;                                     
            }
        }
        
    }
}