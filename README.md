# TRUEbot

Discord bot.

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

`!spot {playerName} {locationName}` alias to the player command, updates the location of the player

### Stats

`!stats` gets all the players the user has reported

`!stats {user}` gets all the players a given user has reported

### Hits

`!hits` gets all the outstanding hits on players

`!hit add {player}` adds a hit to the given player. Player must be added first

`!hit add {player} {reason}` adds a hit to the given player with the given reason. Player must be added first

`!hit stats` get all the completed hits the user has completed

`!hit stats {user}` gets all the completed hits a given user has completed