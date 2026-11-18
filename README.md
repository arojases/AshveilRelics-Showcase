# Ashveil Relics

**A dark fantasy 2D action RPG prototype — Unity 6 · C# · Windows · Demo v1.1**

[**Download Demo v1.1 for Windows on itch.io**](https://arojases.itch.io/ashveil-relics)

## The game

A peaceful village faces an ancient darkness. Accept the *Echoes of Darkness* quest, explore a dungeon defended by skeletons, survive an orc ambush, and face **Lucifer Skeleton Ancient** in a two-phase boss battle. Return to the village to complete the story.

**Ashveil Relics is a short, complete portfolio prototype**, rather than a full-length commercial RPG.

## Gameplay features

- Eight-direction movement, responsive melee combat, and directional dodge with temporary invulnerability
- Skeleton and orc enemy varieties with visible weapons and readable attack warnings
- Dungeon encounters, NPC dialogue, quest tracking, and scene transitions
- **v1.1:** Three new village storytellers (villager, guard and wizard) explaining the dungeon threat
- **v1.1:** A small subterranean cave entrance with a quest-activated door, solid rock collision and a front-facing interaction zone (press **E** to enter)
- Two-phase final boss featuring three attacks and a boss health bar
- Animated main menu, defeat/retry flow, victory screen, and narrative epilogue
- Scene-based soundtrack, contextual sound effects, and music transitions

## What's new in Demo v1.1

**A Village Worth Saving** makes the village feel more alive and the start of the adventure clearer:

- **Villager Woman:** describes the villagers' fear as monsters approach the town.
- **Village Guard:** recounts the failed expedition into the dungeon.
- **Village Wizard:** introduces the ancient seal and the threat of Lucifer Skeleton Ancient.
- **Subterranean cave entrance:** an embedded wooden door opens after accepting *Echoes of Darkness*. Cave boundaries prevent walking through rock, and a small interaction area allows entry only by pressing **E** in front of the door. Once the main quest is complete, the entrance closes.

The rest of the combat, boss encounter and narrative ending are preserved. [Download the updated Windows build](https://arojases.itch.io/ashveil-relics).

## Screenshots

### Main menu
![Ashveil Relics main menu](https://img.itch.zone/aW1hZ2UvNTM5Njg5Ni8zMjQ5OTQ1NC5qcGc=/original/V%2FYNyU.jpg)

### Village
![Village scene](https://img.itch.zone/aW1hZ2UvNTM5Njg5Ni8zMjQ5OTQ2MC5qcGc=/original/p47aqR.jpg)

### Dungeon exploration
![Dungeon exploration](https://img.itch.zone/aW1hZ2UvNTM5Njg5Ni8zMjQ5OTQ1Ni5qcGc=/original/glFw9o.jpg)

### Orc ambush
![Orc ambush](https://img.itch.zone/aW1hZ2UvNTM5Njg5Ni8zMjQ5OTQ1NS5qcGc=/original/HP5TCw.jpg)

### Final boss — Lucifer Skeleton Ancient
![Lucifer boss fight](https://img.itch.zone/aW1hZ2UvNTM5Njg5Ni8zMjQ5OTQ1Ny5qcGc=/original/jalOqg.jpg)

### Story ending
![End of the adventure](https://img.itch.zone/aW1hZ2UvNTM5Njg5Ni8zMjQ5OTQ1OS5qcGc=/original/XQG9GX.jpg)

[View all screenshots and download the game on itch.io](https://arojases.itch.io/ashveil-relics).

## Controls

| Action | Control |
| --- | --- |
| Move | WASD / Arrow Keys |
| Attack | Left Mouse Button / J |
| Dodge | Space |
| Interact | E |

## Play the demo

1. Visit [Ashveil Relics on itch.io](https://arojases.itch.io/ashveil-relics).
2. Download the Windows x64 ZIP and extract all its contents.
3. Run `AshveilRelics.exe` alongside its data files and DLLs.
4. Talk to the villager to start the quest.

**Unity Editor is not required** to play the build.

## Technical overview

| System | Approach |
| --- | --- |
| Engine | Unity 6, 2D top-down |
| Language | C# |
| Character | Melee timing, attack buffering, directional dodge, collision checks |
| Enemy AI | Pursuit, warnings, attack timing, damage feedback |
| Final boss | State-driven behavior, three attacks, health-based phase transition |
| Progression | Quest journal, dialogue, encounters, scene transitions |
| Village v1.1 | Three narrative NPCs, quest-activated underground entrance, collider-based interaction |
| Presentation | TextMeshPro, transitions, reusable audio manager |

See [Architecture](docs/ARCHITECTURE.md) and the [C# code examples](src/README.md).

## About this repository

This is a **public technical showcase**, **not the full Unity project**. The full project remains private because it includes licensed third-party pixel art. The playable Windows build is distributed on itch.io.

Selected first-party C# scripts are included under `src/` to demonstrate implementation. These depend on other Unity components and **are not a standalone compilable game**.

## Development

**Developer:** Ariel Rojas  
**Technology:** Unity 6 / C#  
**Status:** Published Windows demo v1.1

OpenAI Codex and other generative AI tools assisted with coding, implementation, and debugging. Third-party art, sound effects, and music are credited to their original creators.

## Credits

**Art**
- Pixel Crawler — Anokolisa
- Lucifer Skeleton Ancient — Foozle

**Sound effects**
- Kenney — RPG Audio, Impact Sounds, Interface Sounds ([kenney.nl](https://kenney.nl))

**Music**
- [Whispers in the Fog](https://opengameart.org/content/whispers-in-the-fog) — Ruhinre
- [Medieval: The Bard's Tale](https://opengameart.org/content/medieval-the-bards-tale) — RandomMind
- [Dungeon Ambience](https://opengameart.org/content/dungeon-ambience) — yd
- [Heartfelt Battle](https://opengameart.org/content/heartfelt-battle-loopable-fantasy-stringspianohorn) — request
- [Epic Boss Battle](https://opengameart.org/content/boss-battle-music) — Juhani Junkala / SubspaceAudio

See [Third-party assets and distribution](docs/ASSET_CREDITS.md). No original art or audio files are distributed in this repository.
