# Note: Due to time restraints I'm no longer maintaining this plugin. JohnTranQUT will probably maintain his fork [here](https://github.com/JohnTranQUT/Jester) and might update it to the new Impostor version.

# Jester

<img align="right" src="https://github.com/Maartii/Jester/blob/main/Screenshots/MiniJesterBG.png">

**If you're having issues, please try updating to the latest [experimental build](https://ci.appveyor.com/project/Impostor/Impostor/branch/dev/artifacts) of the Impostor server and the latest version of this plugin!**

*Jester* is a plugin for the Among Us private server called [Impostor](https://github.com/Impostor/Impostor) that adds a new role to the game Among Us. The plugin will add a special **Jester role** to the game. The idea behind this Jester role is that if this Jester gets **voted out** during a meeting they **win**, and all other players **lose**. At the start of each game, one player will be assigned this role. 

This adds a special twist to the usual dynamic of Among Us and you now have to be extra careful who you vote. Your very suspicious crewmate standing on a body is probably the imposter... unless the **Jester** is among us. 

## Installation

1. Set up an [Impostor server](https://github.com/Impostor/Impostor) by following the instructions on their Github page. **For now, please use the latest [experimental build](https://ci.appveyor.com/project/Impostor/Impostor/branch/dev/artifacts).**
2. Find the [latest release](https://github.com/Maartii/Jester/releases) of the plugin. 
3. Drop the Jester.dll file in the `plugins` folder of your Impostor server.
4. To play with your client on your private server, see the instructions on the [Impostor](https://github.com/Impostor/Impostor) page.
5. Don't forget to open chat before you start a game! :wink:

## How it works

- When the hosts clicks the 'start' button in the lobby, every player has to **open the ingame chat** to see whether they could become the Jester. 

  <details> 
  <summary><i>Screenshot of the message (click to open)</i></summary>
   <img src="https://github.com/Maartii/Jester/blob/main/Screenshots/JesterAnnouncement.png" width="500"> 
</details>

- If a player that gets the *"You're the Jester"*-message happens to be Imposter, there will be no Jester that game.
- If a player that gets the *"You're the Jester"*-message happens to be a **crewmate**, that player will have the **special Jester role** that game!
- You will also get a **confirmation message** during the **first meeting**. So if you're unsure if you're the Jester you can always press the emergency button.

  <details> 
  <summary><i>Screenshot of confirmation message (click to open)</i></summary>
   <img src="https://github.com/Maartii/Jester/blob/main/Screenshots/JesterConfirmation.png" width="500"> 
</details>

- The Jester is also a crewmate and wins when the crewmates completed all their tasks (including the Jester).
- The Jester has an **additional win-condition**: if the Jester gets **voted out**, the **Jester will win** and all other players will lose.
- When the Jester wins, they are shown in the victory screen with a **Elf Hat**. The other players are shown with a **Dum-sticker** to make clear that they have lost.

  <details> 
  <summary><i>Screenshot of this special Jester victory screen (click to open)</i></summary>
   <img src="https://github.com/Maartii/Jester/blob/main/Screenshots/JesterWin.png" width="500"> 
</details>

- In the Among Us lobby, the host can use the `/jester on` and `/jester off` commands in the chat to turn the jester role on and off. Additionally, any player can write `/jester help` to get an explanation about the Jester role.

## Credits

- All the developers and contributors to the [Impostor](https://github.com/Impostor/Impostor) project.
- My good friend Theo who helped writing this readme.
- And a huge thanks to all of my friends who helped test this plugin! 
