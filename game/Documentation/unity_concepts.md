---
_layout: landing
---

# Unity Fundamentals
## Scenes
A Scene in Unity holds all the elements of a specific part of the game, eg.
menus/levels. Every object in a Scene is a GameObject (kind of like a DOM element), which have Monobehaviors
"attached" which define their behavior.  
Scenes themselves are implemented with Unity's visual editor, where we can 
position, rotate, and scale objects. 
> Remark: Simple scenes such as menus can be implented directly (with some code
> to control shading/animations/transitions), while more dynamic scenes such as
> level loading require extra scripting.

## GameObject
GameObjects are the building blocks of Scenes. Their behavior is defined
attaching scripts to the MonoBehavior.  Some of these are provided by Unity,
while others need to be implemented by us.
Some examples behaviors we've implemented are:
* [GameManager](/api/global/GameManager.html)
* [IWeapon](/api/Global.IWeapon.html)
* [LevelEditor](/api/Global.LevelEditor.html)

Provided behaviors implement low-level behavior, such as rendering and physics:
* [CircleCollider2D](https://docs.unity3d.com/ScriptReference/CircleCollider2D.html)
* [SpriteRenderer](https://docs.unity3d.com/ScriptReference/SpriteRenderer.html)

## Monobehaviour
Monobehaviours integrate Unity with C#, allowing us to use C# OOP alongside
Unity's life cycle management and built-in classes. Unity implements an event
system through Monobehaviour's interface which calls functions depending on
various conditions.

## Lifecycle:
1) Start: called when a GameObject is created (like OnLoad)
2) Update: called every frame, useful for behavior like movement
3) Destroy: destroys the GameObject