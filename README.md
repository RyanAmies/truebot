# TRUEbot

Discord bot.

## Commands

### Player

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
