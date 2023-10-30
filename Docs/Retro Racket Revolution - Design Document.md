# Retro Racket Revolution - Design Document

![](.\images\gamelogo.png)

![](.\images\dag-logo.png)

# Contents

[TOC]

# Vision statement

*A “Breakout” clone, paying homage to the game “Batty” by Elite Systems, released in 1987 by Elite games. Faithful recreation of the levels and core features of “Batty” but with frantic modern gameplay, interesting and challenging level design, bright retro visuals, thumping retro soundtrack.*

# Planned features

## Must have

- Bat and ball, physics game.
- Multiple brick types, laid out in colourful, interesting levels.
- Challenging physics-based puzzles, focussed on rebounding the ball in the right place at the right angle.
- Multiple power ups, giving advantage and disadvantage to players. For example:
  - Extra life
  - Ball slow down
  - Ball splits into multiple balls
  - Expanded player character, making it easier to hit the ball.
  - Laser weapon, for a short period of time, allowing players to shoot bricks.
  - Catch function, allowing players to catch the ball, reposition themselves, and release in a controlled way.
  - All 15 original levels of the “Batty” game, faithfully recreated.
- Two player “co-op” play.
- Retro aesthetics
- Retro music and sound FX

## Nice to have

- Level editor to allow players to create and share their own levels.
- New levels to play.
- New powerups. For example:
  - Gatling laser
  - Mini-balls
  - ...

# Feature references

## Batty, ZX Spectrum, 1987

### Unique Features

- One player, two player “in turns”, two player “co-op”

### Collectibles

- Extra life – adds an additional player life.
- Slow ball – slows the ball on screen to make it easier to hit.
- 5000 points – adds 5000 points to the player score.
- Long bat – grows the player character width.
- Short bat – shrinks the player character width.
- Triple-ball – spawns 3 additional balls.
- Smash shot – makes the ball “smash” through bricks for a period of time.
- The hand / catch – allows the player to catch the ball for a period of time.
- Laser – adds a laser weapon to the player for a period of time.
- Rocket pack / level skip – skips the whole level.
- Kill aliens – prevents aliens from spawning for a period of time.

### Obstacles

- Aliens dropping bombs that can destroy the player.
- Alien ships can be destroyed by the ball or the player character.
- Disruptors – change the path of the ball. Can be on or off.
- Indestructible bricks.
- Multi-hit bricks.

## Reference links

- Spectrum Computing “Batty” page: https://spectrumcomputing.co.uk/entry/472/ZX-Spectrum/Batty
- World of Spectrum “Batty” page: https://worldofspectrum.org/archive/software/games/batty-hit-pak
- Wikipedia “Batty” page: https://en.wikipedia.org/wiki/Batty_(video_game)
- “Batty” game walkthrough on RZX Archive: https://www.youtube.com/watch?v=Y0cCiBZWgeQ

# Level references

## Batty, ZX Spectrum, 1987

| **Level**    | **Level Layout**                 | **Level**    | **Level Layout**                 |
| ------------ | -------------------------------- | ------------ | -------------------------------- |
| **Level 1**  | ![](.\images\batty-level-1.jpg)  | **Level 2**  | ![](.\images\batty-level-2.jpg)  |
| **Level 3**  | ![](.\images\batty-level-3.jpg)  | **Level 4**  | ![](.\images\batty-level-4.jpg)  |
| **Level 5**  | ![](.\images\batty-level-5.png)  | **Level 6**  | ![](.\images\batty-level-6.jpg)  |
| **Level 7**  | ![](.\images\batty-level-7.jpg)  | **Level 8**  | ![](.\images\batty-level-8.jpg)  |
| **Level 9**  | ![](.\images\batty-level-9.jpg)  | **Level 10** | ![](.\images\batty-level-10.jpg) |
| **Level 11** | ![](.\images\batty-level-11.jpg) | **Level 12** | ![](.\images\batty-level-12.jpg) |
| **Level 13** | ![](.\images\batty-level-13.jpg) | **Level 14** | ![](.\images\batty-level-14.jpg) |
| **Level 15** | ![](.\images\batty-level-15.jpg) |              |                                  |

 



# Main Design Elements

## Game UI

 ![](.\images\game-ui.png)

- Player 1 score – shows player 1’s current score.
- Player 2 score– shows player 2’s current score.
- High score – shows current high score.

## Play Area

- 15 bricks wide (columns)
- 12 bricks deep (rows)

## Menu UI

### Main Menu

 ![](.\images\main-meni-ui.png)

### Settings

 ![](.\images\settings.ui.png)

### High score

 ![](.\images\high-score-ui.png)

### How to play

 ![](.\images\how-to-play-ui.png)

### Pause

 ![](E:\Dev\DAG\Retro Racket Revolution\Docs\images\pause-ui.png)

# Implementation

## Core concepts

- Decouple objects.
  - Managers to handle spawning and de-spawning.
  - UI Window and UI Manager separate and independent.
- Use pooling to reduce memory usage:
  - Bricks
  - Enemies
  - Projectiles
  - Player fire laser
  - Enemy bombs

## Class design

 ![](.\images\class-design.png)