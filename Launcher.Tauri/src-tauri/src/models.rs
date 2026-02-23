use serde::{Deserialize, Serialize};
use serde_json::Value;

#[derive(Debug, Serialize)]
pub struct BridgeRequest {
    pub command: String,
    pub payload: Option<Value>,
}

#[derive(Debug, Deserialize)]
pub struct BridgeError {
    pub code: String,
    pub message: String,
}

#[derive(Debug, Deserialize)]
pub struct BridgeResponse {
    pub success: bool,
    pub data: Option<Value>,
    pub error: Option<BridgeError>,
}
