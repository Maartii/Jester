# Jester
*Jester* is a plugin for the Among Us private server with the name [Impostor](https://github.com/Impostor/Impostor) that adds a new role to the game Among Us. The plugin will add a special **Jester role** to the game. The idea behind this Jester role is that if this Jester gets **voted out** during a meeting they **win**, and all other players **lose**. At the start of each game, one player will be assigned to this role. 

This adds a special twist to the usual dynamic of Among Us and you now have to be extra careful who you vote. Your very suspicious crewmate standing on a body is probably the imposter... unless the **Jester** is among us. 

## How it works

- When the hosts clicks the 'start' button in the lobby, every player has to **open the ingame chat** to see whether they are the Jester.

  <details> 
  <summary><i>Screenshot</i></summary>
   <img src="https://github.com/Maartii/Jester/blob/main/Screenshots/JesterAnnouncement.png" width="500"> 
</details>

- If a player that gets the 'Youre the Jester'-message happens to be Imposter, there will be *no Jester* that game.
- If a player that gets the 'Youre the Jester'-message happens to be a crewmate, that player will have the special Jester role that game!
- The Jester is also a crewmate and wins when the crewmates completed all their tasks (including the Jester).
- The Jester has an additional win-condition: if the Jester get voted out, the Jester will win and all other players will lose.

  <details> 
  <summary><i>Screenshot of Jester victory</i></summary>
   <img src="https://github.com/Maartii/Jester/blob/main/Screenshots/JesterWin.png" width="500"> 
</details>

- After a game, all players receive a chat message telling them which player has been the Jester (and whether the Jester won that game).

  <details> 
  <summary><i>Screenshot</i></summary>
   <img src="https://github.com/Maartii/Jester/blob/main/Screenshots/AfterGame.png" width="500"> 
</details>

- In the Among Us lobby, the host can use the `/jester on` and `/jester off` commands in the chat to turn the jester role on and off. Addtionally, any player can write `/help` to get an explanation about the Jester role.

## Installation

1. Set up an [Impostor server](https://github.com/Impostor/Impostor) by following the instructions on their Github page.
2. Find the [latest release](https://github.com/Maartii/Jester/releases) of the plugin. 
3. Drop the Jester.dll file in the `plugin` folder of your Impostor server.
4. To play on your private server, see the instructions on the [Impostor](https://github.com/Impostor/Impostor) page.
5. Don't forget to open chat before you start a game! :wink:

## Credits

- All the developers and contributors to the [Impostor](https://github.com/Impostor/Impostor) project.
- And a huge thanks to all of my friends who helped test this plugin! 

## Donate

If you enjoy the plugin and want to donate some money, you can. I appreciate it! :)

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=ZPDMYAHEHSZAY&currency_code=EUR)
