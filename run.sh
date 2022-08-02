cargo build --target wasm32-unknown-unknown --release
&& cp target/release/
&& http-server -g