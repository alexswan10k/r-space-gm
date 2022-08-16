use corelib::{self, CoreLib::{getScene, SceneItem, tick}};
use macroquad::prelude::*;

#[macroquad::main("BasicShapes")]
async fn main() {
    let mut x = 40.0;

    loop {
        clear_background(BLACK);

        let scene = getScene();
        for s in scene.into_iter() {
            match s.as_ref() {
                SceneItem::SSquare(s) => 
                    draw_circle(s.x as f32, s.y as f32, 30.0, BLUE),
            }
        }
        draw_triangle(Vec2::new(123.0, 55.0), Vec2::new(25.3, 43.5), Vec2::new(75.2, 22.1), GREEN);

        // draw_line(x, 40.0, 100.0, 200.0, 15.0, BLUE);
        draw_rectangle(screen_width() / 2.0 - 60.0, 100.0, 120.0, 60.0, GREEN);
        // draw_circle(screen_width() - 30.0, screen_height() - 30.0, 15.0, YELLOW);
        
        // draw_text("IT WORKS!", 20.0, 20.0, 30.0, DARKGRAY);
        // x = x + 1.0;
        if is_key_down(KeyCode::Up) {

        }

        tick();
        next_frame().await

    }
}

