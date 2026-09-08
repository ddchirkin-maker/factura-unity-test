# Car Turret Shooter

A small mobile-style arcade shooter built as a Unity test assignment. The player controls a turret mounted on a car that drives forward automatically; enemies spawn ahead along the road and rush the car when it gets close. Aim the turret with a joystick, shoot down as many enemies as possible, and reach the finish line before the car's HP runs out.

## Gameplay loop

1. Tap the screen to start. The car begins driving forward, camera follows from behind.
2. Enemies spawn ahead in groups (group size and pacing scale with level progress) and start in an idle state; they engage and run at the car once it gets close.
3. Drag the joystick to aim the turret's firing angle; it auto-fires at the current target while the level is running.
4. Both the car and enemies have HP. Taking enemy contact damage drains the car's HP.
5. Lose: car HP hits zero, "You Lose" overlay shows after a short grace period, tap to restart from the beginning.
6. Win: car reaches the finish line, car stops, "You Win" overlay shows, tap to restart.

## Tech stack

- Unity 6000.3.4f1, Universal Render Pipeline.
- VContainer for dependency injection.
- UniTask for all async flow: spawn loops, level-flow waits, UI show/hide sequencing.
- Generic object pooling (`ComponentPool<T>`) reused for enemies, bullets, and all floating combat-feedback UI (damage numbers, health bars, death effect).

## Project structure

```
Assets/
  Scripts/    Car/, Enemy/, EnemySpawn/, Health/, Level/, MovementInput/, Services/, UI/
  Prefab/     Car/, Enemy/, Effect/, Level/, UI/
  Scenes/Main.unity
```

## How to run

Open the project in Unity 6000.3.4f1, open `Assets/Scenes/Main.unity`, press Play.

## Time spent

_~20 hours_
