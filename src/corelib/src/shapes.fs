namespace Shapes

type Square = {
    x: float
    y: float
}

module Square = 
    let create (x, y) = { x = x; y = y}