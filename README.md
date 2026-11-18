# Ashveil Relics

**A dark fantasy 2D action RPG prototype — Unity 6 · C# · Windows**

[**Download the playable Windows demo on itch.io**](https://arojases.itch.io/ashveil-relics)

## The game

A peaceful village faces an ancient darkness. Accept the *Echoes of Darkness* quest, explore a dungeon defended by skeletons, survive an orc ambush, and face **Lucifer Skeleton Ancient** in a two-phase boss battle. Return to the village to complete the story.

**Ashveil Relics is a short, complete portfolio prototype**, rather than a full-length commercial RPG.

## Gameplay features

- Eight-direction movement, responsive melee combat, and directional dodge with temporary invulnerability
- Skeleton and orc enemy varieties with visible weapons and readable attack warnings
- Dungeon encounters, NPC dialogue, quest tracking, and scene transitions
- Two-phase final boss featuring three attacks and a boss health bar
- Animated main menu, defeat/retry flow, victory screen, and narrative epilogue
- Scene-based soundtrack, contextual sound effects, and music transitions

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
| Presentation | TextMeshPro, transitions, reusable audio manager |

See [Architecture](docs/ARCHITECTURE.md) and the [C# code examples](examples/README.md).

## About this repository

This is a **public technical showcase**, **not the full Unity project**. The full project remains private because it includes licensed third-party pixel art. The playable Windows build is distributed on itch.io.

Selected first-party C# scripts are included under `examples/` to demonstrate implementation. These depend on other Unity components and **are not a standalone compilable game**.

## Development

**Developer:** Ariel Rojas  
**Technology:** Unity 6 / C#  
**Status:** Published Windows demo

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
