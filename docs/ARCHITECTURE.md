# Architecture overview

Ashveil Relics is a Unity 6 / C# top-down action RPG prototype with four scenes:

1. **MainMenuScene** — entry point, title UI, controls and ambient visual effects.
2. **VillageScene** — player introduction, NPC dialogue and the *Echoes of Darkness* quest.
3. **DungeonScene** — skeleton encounters, dungeon exploration and progression.
4. **BossRoomScene** — orc ambush, Lucifer Skeleton Ancient, two combat phases and victory.

## Gameplay systems

**Player:** input and directional movement, timed melee strikes, health, directional dodge with collision checks and temporary invulnerability.

**Enemies:** pursuit and attack logic, telegraphed attacks, damage reactions, and weapon visuals. Orcs and skeletons share common enemy behavior where appropriate.

**Final boss:** Lucifer's state-based AI includes pursuit, attacks, phase transition and death. Attacks are telegraphed, with a stronger attack available in the second phase.

**Progression:** the quest journal, NPC dialogue, scene portals and encounter progress coordinate the adventure. Returning to the village after defeating the boss leads to the epilogue.

**Presentation:** UI panels handle dialogue, health, retry, victory and story ending. A persistent audio manager handles scene-based music and reusable sound effect playback.

## Selected code samples

Three gameplay scripts from the private project are available under [examples](../examples/). They demonstrate implementation patterns, **not** a self-contained Unity project. These classes refer to other components that are intentionally not redistributed here.

## Distribution

The full Unity project remains private due to third-party graphical assets. The playable Windows demo is hosted on [itch.io](https://arojases.itch.io/ashveil-relics).
