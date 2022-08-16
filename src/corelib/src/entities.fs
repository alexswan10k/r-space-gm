module Entities
open System

[<Struct>]
type Vec = {
    x: float32
    y: float32
}

module Vec = 
    let create x y = { x = x; y = y }
    let zero = { x = 0f; y = 0f }

[<Struct>]
type EntityId = { id: Guid }
module EntityId = 
    let create eId = { id = eId }
    let createRand() = Guid.NewGuid() |> create
[<Struct>]
type ComponentId = { id: Guid }
module ComponentId = 
    let create eId = { id = eId }
    let createRand() = Guid.NewGuid() |> create

type Entity = {
    Name: string
    Components: Set<ComponentId>
}

// type PhysicsSimulated = {
//     Position: Vec
//     Velocity: Vec
// }

[<Struct>]
type ComponentType = 
    | Position of pos: Vec
    | Velocity of vel: Vec
    //sprite

[<Struct>]
type Component = {
    Type: ComponentType
}

module Component = 
    let position vec = { Type = Position vec }
    let velocity vec = { Type = Velocity vec }

type ECS = {
    Entities: Map<EntityId, Entity>
    Components: Map<ComponentId, Component>
}

module ECS = 
    module Ety = 
        let private getEntities ecs = 
            ecs.Entities |> Map.toList
        let private tryGetEntity eId ecs = 
            ecs.Entities |> Map.tryFind eId
        let private setEntity eId e ecs =
            { ecs with Entities = ecs.Entities |> Map.add eId e }
        let removeEntity eId ecs = 
            match ecs.Entities |> Map.tryFind eId with
            | Some entity -> 
                {   Entities = ecs.Entities |> Map.remove eId
                    Components = ecs.Components |> Map.filter(fun key t -> entity.Components |> Set.contains key |> not) }
            | None -> ecs
        let createEntity name components (ecs: ECS) = 
            let eId = EntityId.createRand()
            let componentsWithIds = components |> List.map(fun c -> ComponentId.createRand(), c)
            let e = { Name = name; Components = componentsWithIds |> List.map fst |> Set.ofList }
            eId, { Entities = ecs.Entities |> Map.add eId e; Components = ecs.Components }
            
    module Comps =
        let private getComponents ecs =
            ecs.Components |> Map.toList 
        let private getComponentsFor (entity: Entity) (ecs: ECS) = 
            entity.Components |> Set.map (fun k -> ecs.Components |> Map.tryFind k)

        let private tryGetComponent cId ecs = 
            ecs.Components |> Map.tryFind cId
        let private setComponent cId comp (ecs: ECS) =
            { ecs with Components = ecs.Components |> Map.add cId comp }

        let mapComponents f (ecs: ECS) =
            { ecs with Components = ecs.Components |> Map.map (fun k v -> f v) }
    module Behaviour = 
        let apply f (ecs: ECS) =
            //for stuff that has these components, do something
        //
    let create (items) = 
        ({ Entities = Map.empty; Components = Map.empty }, items)
        ||> List.fold (fun state (name, components) -> Ety.createEntity name components state |> snd)
// type Ship = {
//     Phys: PhysicsSimulated
// }

// type Planet = {
//     Position: Vec
// }

// type WorldObjectType = 
//     | Player of Ship
//     | Ship of Ship
//     | Planet of Planet

// type WorldObject = {
//     Id: int
//     Type: WorldObjectType
// }


// module WorldObject = 
//     let mutable idNext = 0
//     let create t = 
//         let curr = idNext
//         idNext <- curr + 1
//         {
//             Id = idNext
//             Type = t
//         }