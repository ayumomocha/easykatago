import { invoke } from "@tauri-apps/api/core";
import { useEffect, useState } from "react";

export function App() {
  const [message, setMessage] = useState("loading");

  useEffect(() => {
    void invoke<string>("ping")
      .then((value) => {
        setMessage(value);
      })
      .catch(() => {
        setMessage("error");
      });
  }, []);

  return (
    <main>
      <h1>EasyKataGo Launcher</h1>
      <p data-testid="ping-message">{message}</p>
    </main>
  );
}
