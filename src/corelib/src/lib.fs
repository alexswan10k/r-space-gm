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

let mutable ecs = 
    ECS.create([
        "Hello", [
            //sprite
            Vec.create 0f 0f |> Component.position 
            Vec.create 0f 0f |> Component.velocity 
        ]
    ])

let tick () = 
    let next = (ecs |> ECS.Comps.mapComponents (fun c -> c))
    ecs <- next