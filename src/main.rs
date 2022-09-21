use corelib::{
    self,
    CoreLib::{getState, renderEty, tick, Inputs},
};
use macroquad::{miniquad::gl::GL_TIME_ELAPSED, prelude::*};

#[macroquad::main("BasicShapes")]
async fn main() {
    let mut x = 40.0;
    let v = vec2(3.0, 2.0);

    let mut prevTime = get_time();

    print!("pretick");
    loop {
        clear_background(BLACK);
        let state = getState();
        for e in state.Entities.into_iter() {
            let v = renderEty(e, state.clone());

            draw_rectangle(screen_width() / 2.0 - 60.0, 100.0, 120.0, 60.0, GREEN);
        }

        draw_triangle(
            Vec2::new(123.0, 55.0),
            Vec2::new(25.3, 43.5),
            Vec2::new(75.2, 22.1),
            GREEN,
        );

        draw_line(x, 40.0, 100.0, 200.0, 15.0, BLUE);
        draw_rectangle(screen_width() / 2.0 - 60.0, 100.0, 120.0, 60.0, GREEN);
        draw_circle(screen_width() - 30.0, screen_height() - 30.0, 15.0, YELLOW);

        draw_text(
            ("IT WORKS!".to_owned() + prevTime.to_string().as_str()).as_str(),
            20.0,
            20.0,
            30.0,
            DARKGRAY,
        );
        // x = x + 1.0;
        let mut input = Inputs::NoInput;
        if is_key_down(KeyCode::Up) {
            input = Inputs::UpArrow
        }
        if is_key_down(KeyCode::Down) {
            input = Inputs::DownArrow
        }
        if is_key_down(KeyCode::Left) {
            input = Inputs::LeftArrow
        }
        if is_key_down(KeyCode::Right) {
            input = Inputs::RightArrow
        }

        let curr_time = get_time();
        let dt = curr_time - prevTime;
        let htext = format!("dt: {:?} {:?}", dt, input);
        draw_text(htext.as_str(), 40.0, 40.0, 30.0, DARKGREEN);
        tick(dt as f32, input);
        prevTime = curr_time;
        next_frame().await
    }
}
