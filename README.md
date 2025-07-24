# Acorn

Acorn is a Discord bot I am developing mainly for my friends and I. It is coded in C# using the [DSharpPlus library](https://github.com/DSharpPlus/DSharpPlus), and designed to run on a Linux machine (specifically, a Lenovo ThinkCentre M710q mini-PC running Ubuntu, on my desk).

## Features
- **Remembering and returning quotes:** The original purpose for Acorn. Discord messages may be added as quotes and recalled at random. There are also features for returning a specific quote, returning a random quote by a specific person, and searching in plain-text. Image attachments are transcribed with [Tesseract](https://github.com/charlesw/tesseract).
- **Converting units:** Converts a value between two units, using the [UnitsNet library](https://github.com/angularsen/UnitsNet/).
- **Converting currencies:** Converts a value between two currencies, relying on the [Exchange API](https://github.com/fawazahmed0/exchange-api).
- **Shaking a magic 8 ball:** Returns one of the [standard responses](https://en.wikipedia.org/wiki/Magic_8_Ball#Possible_answers) of a magic 8 ball with some added consistency, thanks to seeds generated based on the asker's username and question.
- **Generating character sheets:** Returns a randomly-rolled Dungeons & Dragons character sheet.
- **Coin-flipping:** What it says on the tin.
- **Rolling dice:** Rolls a specified number of dice with the specified number of sides. Based on the command of the same purpose from Roll20 (though, not as in-depth).

## Usage and Redistribution
You may use Acorn's code, in part or in its entirety, in your own projects, but please include my name and a link to this repository as attribution. Since bot accounts are defined entirely on Discord's side, you can run this code as-is under a different bot account's name. Please note that some things, like the channel which debug messages are sent to, may be hardcoded and require forking and modification before use.

## Contribution
Contributions and feedback are welcome, but please keep in mind that I am creating Acorn for myself. If your contribution is rejected, you can always fork this repository and create your own version.
