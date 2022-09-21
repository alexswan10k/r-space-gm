module Entities
open System
open Fable.Core.Rust

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
[<Measure>] type Rad

open Fable.Core

module Testing =
    [<Emit("assert_eq!")>]
    let inline equal expected actual: unit = nativeOnly
    [<Emit("assert_ne!")>]
    let inline notEqual expected actual: unit = nativeOnly
    type FactAttribute() = inherit System.Attribute()

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
    let calcAttractiveForce struct(m1: float32<kg>, p1) struct(m2: float32<kg>, p2): float32<N> = 
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
    let toVec (magnitude: float32<'a>) (dir: float32<Rad>) =
        let x = Math.Cos(dir |> float) |> float32
        let y = Math.Sin(dir |> float) |> float32
        { x = x * magnitude; y = y * magnitude }
    let normalize (v: Vector2<'a>) =
        let hyp = magnitude v
        let x = v.x / hyp
        let y = v.y / hyp
        { x = x; y = y }
    let invert (v: Vector2<'a>) = 
        {x = -v.x; y = -v.y}

    module Tests = 
        open Testing
        [<Fact>]
        let is4 () =
            1 |> equal 1


[<Struct>]
type Spatial = {
    Rotation: float32<Rad>
    Position: Vector2<m>
}

type PhysicsSimulated = {
    Velocity: Vector2<m/s>
    Forces: Vector2<N>
    Mass: float32<kg>
}

module PhysicsSimulated =
    let gravForceVec (position1: Vector2<m>, item1) (position2: Vector2<m>, item2) =
        let force = Vec.calcAttractiveForce struct(item1.Mass, position1) struct(item2.Mass, position2)
        let direction = position2 - position1
        let item1Direction = Vec.normalize direction
        //let item2Direction = Vec.invert item1Direction
        let item1Force = item1Direction * force
        //let item2Force = item2Direction * force
        item1Force//, item2Force

    let gravInteractions (sim) (item1Pos: Vector2<m>, item1: PhysicsSimulated) = 
        let allForces = 
            (item1.Forces, sim) 
            ||> Seq.fold (fun acc struct(item2Pos, item2) -> 
                if item2.Mass = item1.Mass then acc //skip self
                else acc + gravForceVec (item1Pos, item1) (item2Pos, item2)               
                )
        {item1 with Forces = allForces}
        
    let incrementFrameFromForces (dt: float32<s>) (position, p: PhysicsSimulated) = 
        let acceleration = Vec.getAcceleration p.Forces p.Mass
        position + (p.Velocity * dt),
            { p with    Velocity = p.Velocity + (acceleration * dt) 
                        Forces = Vec.zero() }
    
    // let nextFrameAll  (dt: float32<s>) (ps: PhysicsSimulated array) =
    //     [|for p in ps ->  
    //         let pInt = gravInteractions ps p
    //         nextFrame dt pInt
    //         |]

type Entity = {
    Name: string
    Spatial: Spatial
    Physics: PhysicsSimulated option
    // Components: Component list
}
module Entity = 
    let named name = { Name = name ; Physics = None; Spatial = { Position = Vec.zero(); Rotation = 0f<Rad> } }
    let withPhysics physics ent = { ent with Physics = Some physics}
    let withPosition x y ent= 
        { ent with Spatial = {ent.Spatial with Position = { x = x; y = y }}}
    let withMass mass ent = 
        withPhysics { Mass = mass; Velocity = Vec.zero(); Forces = Vec.zero() } ent
    let applyForce f ent =
        match ent.Physics with
        | Some phys ->
            {ent with Physics = Some {phys with Forces = phys.Forces + f }}
        | None -> ent
    let applyForceMD m dir ([<ByRef>]ent) =
        match ent.Physics with
        | Some phys ->
            {ent with Physics = Some {phys with Forces = Vec.toVec m dir }}
        | None -> ent
    let rotate r ([<ByRef>]ent) = 
        {ent with Spatial = {ent.Spatial with Rotation = ent.Spatial.Rotation + r}}

type Viewport = {
    Position: Vector2<m>
    Zoom: float32
}

type GameState = {
    Entities: Entity list
    Viewport: Viewport
}