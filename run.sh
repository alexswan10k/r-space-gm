
cargo build --target wasm32-unknown-unknown --release 
cp target/wasm32-unknown-unknown/release/testgame.wasm testgame.wasm
http-server -g