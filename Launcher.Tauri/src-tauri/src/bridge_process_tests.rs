#[test]
fn builds_bridge_request_payload() {
    let payload = crate::bridge_process::build_payload("settings.read", None);
    assert!(payload.contains("settings.read"));
}
