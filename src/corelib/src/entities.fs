module Entities
open System

[<Struct>]
type Vector2<[<Measure>] 'a> = {
    x: float32<'a>
    y: float32<'a>
} 
with
    static member (+) (v1 : Vector2<'a>, v2: Vector2<'a>) =
        { x = v1.x + v2.x; y =  v1.y + v2.y }
    static member (-) (v1 : Vector2<'a>, v2: Vector2<'a>) =
        { x = v1.x - v2.x; y =  v1.y - v2.y }
    static member (+) (v : Vector2<'a>, s) =
        { x = s + v.x; y =  s + v.y }
    static member (-) (v : Vector2<'a>, s) =
        { x = s - v.x; y =  s - v.y }
    static member (*) (v : Vector2<'a>, s) =
        { x = s * v.x; y =  s * v.y }
    static member (*) (v1 : Vector2<'a>, v2: Vector2<'a>) =
        { x = v1.x * v2.x; y =  v1.x * v2.y }
    static member (/) (v : Vector2<'a>, s) =
        { x = v.x / s; y =  v.y / s }

[<Measure>] type kg
[<Measure>] type m
[<Measure>] type s
[<Measure>] type N = kg*m/s^2
[<Measure>] type Deg

module Vec = 
    let create x y = { x = x; y = y }
    let zero () = { x = 0f<_>; y = 0f<_> }
    // let (.*) a b =
    //     { x = a.x * b.x; y = a.y * b.y }
    //let f32<[<Measure>] 'a> (f: float<'a>) = f |> float32
    //type single<[<Measure>] 'Measure> = float32<'Measure>
    // let pow2<[<Measure>] 'm> (x: float32<'a>) =
    //     let p = Math.Pow(float x, 2) |> float32 
    //     p * (1f<_>)
    // let square//<[<Measure>] 'a> 
    //     (f: float32<'a>): float32<'a^2> =
    //     (Math.Pow(f |> float, 2) |> float32) * 1f<_>
    let square (a: float32<_>) = a * a

    let distance (v2: Vector2<m>) (v1: Vector2<m>): float32<m> =
        let dist = v2 - v1
        let magnitude = (pown (float dist.x) 2 + pown (float dist.y) 2) |> float32 //Math.Pow(dist.y |> float, 2)
        let mUnits = (magnitude * 1f<m^2>)
        mUnits |> sqrt
    let calcNext (velocity: Vector2<m/s>, dt: float32<s>) (position:Vector2<m>) =
        let positionDelta = velocity * dt
        position + (positionDelta)
    let calcNextVelocity (accel: Vector2<m/s^2>, dt: float32<s>) (velocity: Vector2<m/s>) = 
        let velocityDelta = accel * dt
        velocity + velocityDelta
    // F = Gm1m2/r^2
    let calcAttractiveForce (m1: float32<kg>, p1) (m2: float32<kg>, p2): float32<N> = 
        let g = 6.674 * Math.Pow(10, -11) |> float32
        let g = g * 1f<m^3*kg^-1*s^-2>
        let r = distance p2 p1
        let gm12 = g * m1 * m2 
        let res = gm12 / square r
        res
    // F = m a --> F / m = a
    let getAccelerationScalar (force: float32<N>) (m: float32<kg>): float32<m/s^2> = 
        force / m
    let getAcceleration (force: Vector2<N>) (m: float32<kg>): Vector2<m/s^2> = 
        force / m
    let magnitudeSq (v: Vector2<'a>) =
        square v.x + square v.y
    let magnitude (v: Vector2<'a>) = 
        //pythagoras
        sqrt (magnitudeSq v)
    let normalize (v: Vector2<'a>) =
        let hyp = magnitude v
        let x = v.x / hyp
        let y = v.y / hyp
        { x = x; y = y }
    let invert (v: Vector2<'a>) = 
        {x = -v.x; y = -v.y}

type PhysicsSimulated = {
    Rotation: float32<Deg>
    Position: Vector2<m>
    Velocity: Vector2<m/s>
    Forces: Vector2<N>
    Mass: float32<kg>
}

module PhysicsSimulated =
    let forceVec item1 item2 =
        let force = Vec.calcAttractiveForce (item1.Mass, item1.Position) (item2.Mass, item2.Position)
        let direction = item2.Position - item1.Position
        let item1Direction = Vec.normalize direction
        //let item2Direction = Vec.invert item1Direction
        let item1Force = item1Direction * force
        //let item2Force = item2Direction * force
        item1Force//, item2Force

    let interactions (dt: float32<s>) (sim: PhysicsSimulated seq) (item1: PhysicsSimulated) = 
        let allForces = 
            (item1.Forces, sim) 
            ||> Seq.fold (fun acc item2 -> 
                    acc + forceVec item1 item2               
                    )
        {item1 with Forces = allForces}
        
    let nextFrame (dt: float32<s>) (p: PhysicsSimulated) = 
        let acceleration = Vec.getAcceleration p.Forces p.Mass
        { p with    Position = p.Position + (p.Velocity * dt)
                    Velocity = p.Velocity + (acceleration * dt) 
                    Forces = Vec.zero() }
    
    let nextFrameAll  (dt: float32<s>) (ps: PhysicsSimulated array) =
        [|for p in ps ->  
            let pInt = interactions dt ps p
            nextFrame dt pInt
            |]
type Entity = {
    Name: string
    Physics: PhysicsSimulated option
    // Components: Component list
}
module Entity = 
    let named name = { Name = name ; Physics = None}
    let withPhysics physics x = { x with Physics = Some physics}

type GameState = {
    Entities: Entity list
}