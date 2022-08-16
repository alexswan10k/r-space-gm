module CoreLib

open Shapes
open Entities
open World

let hello () =
    CoreLib.Test.hello ()

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
            Entity.named "test"
        ]
    }

let captureInputs () = ()

let playerControlSystem inputs x =
    x
let npcBehaviourSystem x =
    x

let physicsSystem x =
    x
let renderSystem x = 
    x
let audioSystem x =
    x

let tick () = 
    let inputs = captureInputs()

    let current = gamestate

    let next = 
        current
        |> playerControlSystem inputs
        |> npcBehaviourSystem
        |> physicsSystem
        |> renderSystem
        |> audioSystem


    //wait vsync

    gamestate <- next