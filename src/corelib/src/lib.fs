module CoreLib

open Shapes
open Entities
open World

// let hello () =
//     CoreLib.Test.hello ()

// type SceneItem = 
//     | SSquare of Square
//     | World of WorldSt

// let mutable scene = [
//     SSquare (Square.create (2, 1))
//     World {
//         items = [
//             WorldObject.create (Player { Phys = { Position = Vec.create 10f 10f; Velocity = Vec.zero} })
//         ]
//     }
// ]

// let getScene () = scene


// let updateElems = function 
//     | SSquare s -> SSquare (Square.create(s.y + 1., s.x))
//     | World worldSt -> World worldSt

// let tick () = 
//     let next = (scene |> List.map(updateElems))
//     scene <- next

let mutable gamestate = 
    {
        Entities = [
            Entity.named "player"
            |> Entity.withPosition 1000f<m> 1000f<m>
            |> Entity.withMass 1000f<kg>
            Entity.named "Planet"
            |> Entity.withPosition 2400f<m> 1000f<m>
            |> Entity.withMass 10000000f<kg>
        ]
        Viewport = {
            Position = { x = 1000f<m>; y = 1000f<m> }
            Zoom = 0.5f
        }
    }
let getState() = gamestate

let captureInputs () = ()

[<Struct>]
type Inputs = 
    | UpArrow
    | LeftArrow
    | RightArrow
    | DownArrow
    | NoInput
    
let playerControlSystem inputs world = 
    match world.Entities with
    | ({ Physics = Some phys } as player)::t ->
        let playerNext = 
            match inputs with
            | UpArrow -> player |> Entity.applyForceMD 5000f<N> player.Spatial.Rotation //|> Entity.applyForce { x = 1000f<N>; y = 0f<N> }
            | DownArrow -> player  |> Entity.applyForceMD -200f<N> player.Spatial.Rotation
            | RightArrow -> player |> Entity.rotate 0.02f<Rad>
            | LeftArrow -> player |> Entity.rotate -0.02f<Rad>
            | NoInput -> player
        { gamestate with Entities = playerNext::t }
    | _ -> { gamestate with Entities = [] }

let npcBehaviourSystem x =
    x

let physicsSystem dt world =
    let physSimObjs = 
        world.Entities 
        |> List.choose(function 
            | { Spatial = { Position = position} 
                Physics = Some phys } -> Some struct(position, phys) 
            | _ -> None) |> Array.ofList
    let entities = 
        world.Entities 
        |> List.map (fun e -> 
            match e.Physics with
            | Some ps ->
                let ps = PhysicsSimulated.gravInteractions physSimObjs (e.Spatial.Position, ps)
                let (posNext, ps) = PhysicsSimulated.incrementFrameFromForces dt (e.Spatial.Position, ps)
                { e with Physics = Some ps; Spatial = { e.Spatial with Position = posNext } }
            | None -> e
            )
    { world with Entities = entities }
let renderSystem x = 
    x
let viewportFollowPlayer world = 
    match world.Entities with
    | player::t ->
        { world with Viewport = { world.Viewport with Position = player.Spatial.Position } }
    | _ -> world
let audioSystem x =
    x

let tick dt inputs = 

    let current = gamestate

    let next = 
        current
        |> playerControlSystem inputs
        |> npcBehaviourSystem
        |> physicsSystem dt
        |> viewportFollowPlayer
        |> renderSystem
        |> audioSystem


    //wait vsync

    gamestate <- next

let renderEty (screenWidth: float32, screenHeight: float32) e state =
    let v = { x = screenWidth * 0.5f<m>; y = screenHeight * 0.5f<m> }

    let screenPos = (e.Spatial.Position - state.Viewport.Position) * state.Viewport.Zoom
    screenPos + v