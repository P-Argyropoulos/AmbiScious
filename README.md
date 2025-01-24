Gameplay Video : https://youtu.be/a4X6b6PTX3k?feature=shared

C# Scripting in Scripts Folder

AmbiScious is a fast pace 2.5D (3D environment but player's POV in Horizontal and Vertical Axis) platformer game that implements some core mechanics of a platformer game.
It's still in very early development phase with only the core movement, control and animations of the main character beeing implemented.

    Gameplay Related : 

Gameplay mechanics include ground movement with custom acceleration and deceleration values, variable jump height, air acceleration/air deceleration values, dash ability, shooting.

Custom gravity values and in general gravity behaviour  applied to the player for the whole purpose of player's movement and control to feel responsive and satisfying (values are still being tweaked).

Helping mechanics grace time/coyote time (player is still able to jump in a small time window after running of a platform) and jump buffer ( player pressed the jump button a fraction before landing , the action is still executed)  are implemented.

    Technical Related : 

Unity's new input system used for versatility and expandability.

Player's inputs pass through a Scriptable object class to Player Controller class, in order to be accesible from multiple points, loosely coupled and for a possible multiplayer implementation. 

An expandable Finite State Machine is implemented to handle character's animations and animations transitions between states. 

    Future Implementations: 

Next steps in the course of development are a gameplay story and a purpose that the level design gonna based of, enemy Ai with aggresive behaviour against the character , Boss fight, UI, Audio sound and audio effects.

Also a better connection between animations and movement. 
