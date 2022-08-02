module CoreLib

open Shapes

let hello () =
    CoreLib.Test.hello ()

type SceneItem = 
    | SSquare of Square

let mutable scene = [
    SSquare (Square.create (2, 1))
]

let getScene () = scene

let tick () = 
    let next = (scene |> List.map(function | SSquare s -> SSquare (Square.create(s.y + 1., s.x))))
    scene <- next