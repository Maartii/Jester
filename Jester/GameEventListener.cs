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

        public IClientPlayer Jester;

        public bool JesterOn = true;

        public bool Jesterwin = false;

        public bool GameEnded = false;

        public bool CountingDown = false;

        public bool JesterInGame = false;

        public string Jestername;
    
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
        public void OnSetStartCounter(IPlayerSetStartCounterEvent e)
        {
            if (e.SecondsLeft == 5)
            {
                CountingDown = true;
                _logger.LogInformation($"Countdown started.");
                if (JesterOn)
                {
                    List<IClientPlayer> gameplayers = new List<IClientPlayer>();
                    foreach (var player in e.Game.Players)
                    {
                        gameplayers.Add(player);
                    }
                    Task.Run(async () => await AssignJester(gameplayers).ConfigureAwait(false));
                    foreach (var player in e.Game.Players)
                    {
                        Task.Run(async () => await MakePlayerLookAtChat(player).ConfigureAwait(false));
                    }
                }
            }
        }

        private async Task MakePlayerLookAtChat(IClientPlayer player)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            string playername = player.Character.PlayerInfo.PlayerName;
            await player.Character.SetNameAsync($"OPEN CHAT").ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
            await player.Character.SetNameAsync(playername).ConfigureAwait(false);
        }      

        private async Task AssignJester(List<IClientPlayer> gameplayers)
        {
            int r = rnd.Next(gameplayers.Count);
            Jester = gameplayers[r];
            Jestername = gameplayers[r].Character.PlayerInfo.PlayerName;
            await ServerSendChatToPlayerAsync($"You're the JESTER! (unless you'll be an imposter)", gameplayers[r].Character).ConfigureAwait(false);
            _logger.LogInformation($"- {gameplayers[r].Character.PlayerInfo.PlayerName} is probably the jester.");    
        }

        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            GameEnded = false;
            CountingDown = false;
            JesterInGame = false;
            _logger.LogInformation($"Game is starting.");
            Jesterwin = false;
            if (JesterOn)
            {
                Task.Run(async () => await InformJester().ConfigureAwait(false));
            }            
            // This prints out for all players if they are impostor or crewmate.
            foreach (var player in e.Game.Players)
            {
                if (JesterOn && (player.Character.PlayerInfo.HatId == 27 || player.Character.PlayerInfo.HatId == 84))
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

        private async Task InformJester()
        {
            if (Jester.Character.PlayerInfo.IsImpostor)
            {
                _logger.LogInformation($"- {Jester.Character.PlayerInfo.PlayerName} isn't jester but impostor.");
                await ServerSendChatToPlayerAsync($"You happen to be IMPOSTER! No Jester this game.", Jester.Character).ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation($"- {Jester.Character.PlayerInfo.PlayerName} is indeed jester.");
                await ServerSendChatToPlayerAsync($"You're indeed the JESTER!", Jester.Character).ConfigureAwait(false);
                JesterInGame = true;
            }
        }

        private async Task OffWithYourHat(IClientPlayer player)
        {
            await player.Character.SetHatAsync(HatType.NoHat).ConfigureAwait(false);
        }

        [EventListener]
        public void OnPlayerExiled(IPlayerExileEvent e)
        {
            if (JesterInGame && e.PlayerControl == Jester.Character)
            {                                
                Jesterwin = true;
                _logger.LogInformation($"Jester has won!");
                Task.Run(async () => await TurnTheTables(e).ConfigureAwait(false));
            }
        }

        private async Task TurnTheTables(IPlayerExileEvent e)
        {
            await Jester.Character.SetHatAsync(HatType.ElfHat).ConfigureAwait(false);
            foreach (var player in e.Game.Players)
            {
                if (player != Jester)
                {
                    await player.Character.SetHatAsync(HatType.DumSticker).ConfigureAwait(false);
                }
            }
            foreach (var player in e.Game.Players)
            {
                if (!player.Character.PlayerInfo.IsDead && player.Character.PlayerInfo.IsImpostor)
                {
                    _logger.LogInformation($"- {player.Character.PlayerInfo.PlayerName} is murdered by plugin.");
                    await player.Character.SetMurderedAsync().ConfigureAwait(false);                              
                }
            }
                      
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            _logger.LogInformation($"Game has ended.");
            GameEnded = true;
        }

        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent e)
        {
            if (GameEnded && JesterInGame)
            {               
                Task.Run(async () => await JesterAnnouncement(e).ConfigureAwait(false));
            }
        }

        private async Task JesterAnnouncement(IPlayerSpawnedEvent e)
        {
            if (Jesterwin)
            {
                await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                await ServerSendChatToPlayerAsync($"{Jestername} was Jester and won by getting ejected!", e.PlayerControl).ConfigureAwait(false);
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                await ServerSendChatToPlayerAsync($"{Jestername} was Jester, but didn't get voted out.", e.PlayerControl).ConfigureAwait(false);                
            }
        }

        [EventListener]
        public void OnPlayerChat(IPlayerChatEvent e)
        {
            _logger.LogInformation($"{e.PlayerControl.PlayerInfo.PlayerName} said {e.Message}");
            if (e.Game.GameState == GameStates.NotStarted && !CountingDown && e.Message.StartsWith("/"))
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
                        JesterOn = true;
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
                        JesterOn = false;
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