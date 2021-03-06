# wowiebot

wowiebot is a self-hosted (meaning hosted by you, not by me) Twitch bot that you can download and use. It's easy to use and new features are being added all the time. You can use the wowiebot account, or a different account if you want!

wowiebot was developed using the framework [twitchat](https://github.com/chhopsky/twitchat) by the lovely [chhopsky](https://twitter.com/chhopsky).

## Getting Started

To use wowiebot, download the zip from the [latest release](https://github.com/scattertv/wowiebot/releases/latest). Extract the folder where you wish and run wowiebot.exe. Configuration files will be created in `AppData\Local\wowiebot`.

If you wish to use the wowiebot Twitch account, leave the "Log in as wowiebot" box checked. Otherwise, if you want to run the bot as my_awesome_bot or whatever other account you own, uncheck it and click Log In. You will need to provide your Oauth2 credentials; a link is provided to a page explaining how to do so. Once logged in, type the name of the channel you wish to connect to and click Connect as `<username>`.

## Configuration

Configuration in wowiebot is done through the Configure menu. The Export/Import buttons can export and import your settings to and from a json file for easy portability.

### Commands 

Several example commands are provided to demonstrate the various features of the bot, but you may delete them by clicking on the left column to select the entire row and press the Delete key. To write a new command, just start typing in the bottom row and a new row will be created. Quick add buttons are provided for most purposes; clicking these will add a default command which can then be edited.

In the message of a bot command, placeholders can be used. There are two main types of placeholders, static and dynamic. Static placeholders simply replace the placeholder with some text; the broadcaster's name, for example. Dynamic placeholders completely change the command's meaning and may not send any message at all.

* Enabled: This is used to enable or disable a command. Disabled commands will be ignored by the bot.
* Command: Comma-separated list of keywords that will trigger the command. The first value is the one that will appear in the list of commands, if applicable.
* Permissions: Comma-separated list of users that can trigger the command. Broadcaster always has permission. Can include $MOD to grant access to all mods. If left blank, all users can run the command.
* Message: This is the message that will be sent by the bot when the command is received. Placeholders such as $QUOTE can be used and will be replaced accordingly. Press the ? button or see below for a full list of placeholders.
* Show in commands list: This determines whether the command will be shown in the list generated by the $COMMANDS placeholder, generally seen as !help or !commands.

#### Placeholders

Static:

* $QUOTE: A random quote from the list of quotes
* $QNUM: The index of the quote selected
* $BROADCASTER: The username of the broadcaster
* $SENDER: The username of the user who sent the command
* $GAME: The game the broadcaster is currently playing
* $TITLE: The current title of the broadcaster's stream
* $UPHOURS: Whole number of hours the broadcaster has been live
* $UPMINUTES: Whole number of minutes the broadcaster has been live
* $8BALL: A random choice from the 8-Ball Choices
* $COMMANDS: A list of all commands with "Show in commands list" checked
* $QUEUETIME: The total length of all the videos in the Song Requests queue.

Dynamic: 

* $ADDQUOTE: Adds a quote according to the "Quote adding permissions" setting
* $VOTEYES: Adds 1 to the number of people voting to add the quote (only meaningful if voting quote permission is used)
* $CALCULATOR: Evaluates a mathematical expression
* $SONGREQ: Parses a Youtube link and adds it to the Song Requests queue.

### Text Strings 

Here all of the various text strings in wowiebot can be edited. Quotes, 8-Ball choices and periodic messages are lists of strings that can be edited here, one per line.

On the right side, Event Messages can be edited. These messages will be sent in chat in response to the event listed. $SENDER and $BROADCASTER can be used in all of them, and additional tokens are available as listed. To disable an event message, leave the box blank.

## Song Requests

wowiebot now supports song requests. To enable these, add a song request command and open the Song Requests window from the main launchpad. Chatters can use the command with a Youtube link or a search query to queue up videos to play in the window. When the video is over it will automatically play the next, unless Autoplay is unchecked. The "Play next/skip" button can be used to skip to the end of the current video, or to start the next if Autoplay is disabled. If this window is closed, the queue will be lost.

## Development

wowiebot is developed in C#. If you wish to develop for wowiebot, clone the repo and simply open the .sln in Visual Studio. The required NuGet packages should automatically be installed at this time.

## License

wowiebot is licensed under [WTFPL](http://www.wtfpl.net/).
