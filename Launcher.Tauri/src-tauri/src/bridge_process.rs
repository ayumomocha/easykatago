use std::io::Write;
use std::path::PathBuf;
use std::process::{Command, Stdio};

use serde_json::Value;

use crate::models::{BridgeError, BridgeRequest, BridgeResponse};

#[derive(Clone)]
pub struct BridgeProcess {
    executable_path: PathBuf,
}

#[derive(Clone)]
pub struct AppState {
    pub bridge: BridgeProcess,
}

impl BridgeProcess {
    pub fn from_environment() -> Self {
        let bridge_path = std::env::var("LAUNCHER_BRIDGE_PATH")
            .unwrap_or_else(|_| "Launcher.Bridge.exe".to_string());
        Self {
            executable_path: PathBuf::from(bridge_path),
        }
    }

    pub async fn invoke(&self, command: &str, payload: Option<Value>) -> Result<Value, String> {
        let executable_path = self.executable_path.clone();
        let request_payload = build_payload(command, payload);

        tauri::async_runtime::spawn_blocking(move || run_bridge(&executable_path, &request_payload))
            .await
            .map_err(|err| format!("Bridge process join error: {err}"))?
    }
}

pub fn build_payload(command: &str, payload: Option<Value>) -> String {
    serde_json::to_string(&BridgeRequest {
        command: command.to_string(),
        payload,
    })
    .expect("bridge request serialization failed")
}

fn run_bridge(executable_path: &PathBuf, request_payload: &str) -> Result<Value, String> {
    let mut child = Command::new(executable_path)
        .stdin(Stdio::piped())
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .spawn()
        .map_err(|err| format!("Failed to start bridge process: {err}"))?;

    if let Some(stdin) = child.stdin.as_mut() {
        stdin
            .write_all(request_payload.as_bytes())
            .map_err(|err| format!("Failed to write bridge payload: {err}"))?;
    }

    let output = child
        .wait_with_output()
        .map_err(|err| format!("Failed to read bridge process output: {err}"))?;

    if !output.status.success() {
        let stderr = String::from_utf8_lossy(&output.stderr);
        return Err(format!("Bridge process exited with {}: {stderr}", output.status));
    }

    let response: BridgeResponse = serde_json::from_slice(&output.stdout)
        .map_err(|err| format!("Failed to parse bridge response: {err}"))?;

    if response.success {
        return response
            .data
            .ok_or_else(|| "Bridge response did not include data payload".to_string());
    }

    Err(format_bridge_error(response.error))
}

fn format_bridge_error(error: Option<BridgeError>) -> String {
    match error {
        Some(details) => format!("{}: {}", details.code, details.message),
        None => "Bridge request failed without details".to_string(),
    }
}
