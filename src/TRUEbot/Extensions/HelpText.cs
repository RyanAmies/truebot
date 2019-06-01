using System;
using System.Collections.Generic;
using System.Text;

namespace TRUEbot.Extensions
{
    public static class HelpText
    {
        public static string Text = @"# TRUEbot

This is the offial TRUE discord bot ran by InsaneRhino. If you have any issues PM him.

## Commands

### Player

`!player {name}` searches for a player with the given name

`!player add {name}` adds a player with the given name

`!player add {name} {alliance}` adds a player with the given name and alliance

`!player add {name} {alliance} {location}` adds a player with the given name, alliance and location

`!player rename {currentName} {newName}` renames the current player to the new, provided, name

`!player assign {playerName} {allianceName}` assigns a player to the alliance

`!player spot {playerName} {location}` updates the player's location to the provided location

`!player delete {playerName}` deletes the player from the database, this cannot be undone

### Alliances

`!alliance {name}` gets all players part of the alliance

### Location

`!location {name}` gets all players in the location

`!spot {playerName} {locationName}` alias to the player command, updates the location of the player";


         public static string Text2 = @"### Stats

`!stats` gets all the players the user has reported

`!stats {user}` gets all the players a given user has reported

### Kills

`!kill log {victim}` logs the kill of a victim

`!kill log {victim} {power}` logs the kill of a victim, and the power of the destroyed ship. {power} must be a number, with no commas (,)

`!kill alliancestats {alliance}` gets all the kills for an alliance in the last 1 day

`!kill alliancestats {alliance} {days}` gets all the kills for an alliance in the last {day} days

`!kill killerstats` gets all the kills for the user in the last 1 day

`!kill killerstats {player}` gets all the kills for a user in the last 1 day

`!kill killerstats {player} {days}` gets all the kills for a player in the last {day} days

`!kill victimstats {player}` gets all the kills for a player in the last 1 day

`!kill victimstats {player} {days}` gets all the kills for a player in the last {day} days

`!kill toppower` get the top 10 killers for the last day ordered by power destroyed

`!kill toppower {days}` get the top 10 killers over a period of {days} ordered by power destroyed

`!kill topkills` get the top 10 killers for the last day ordered by power destroyed

`!kill topkills {days}` get the top 10 killers over a period of {days} ordered by power destroyed";
    }
}
