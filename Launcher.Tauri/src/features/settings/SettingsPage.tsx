import { useEffect, useState } from "react";
import { settingsRead, settingsWrite } from "../../lib/bridge";
import type { SettingsData } from "../../lib/contracts";

const emptySettings: SettingsData = {
  installRoot: ""
};

export function SettingsPage() {
  const [settings, setSettings] = useState<SettingsData>(emptySettings);
  const [status, setStatus] = useState<"loading" | "idle" | "saving" | "error">("loading");
  const [message, setMessage] = useState<string | null>(null);

  useEffect(() => {
    let active = true;

    void settingsRead()
      .then((nextSettings) => {
        if (!active) {
          return;
        }

        setSettings({
          ...emptySettings,
          ...nextSettings,
          installRoot: nextSettings.installRoot ?? "."
        });
        setStatus("idle");
      })
      .catch(() => {
        if (!active) {
          return;
        }

        setSettings({ installRoot: "." });
        setStatus("error");
        setMessage("读取设置失败，请检查桥接服务。");
      });

    return () => {
      active = false;
    };
  }, []);

  const save = async () => {
    setStatus("saving");
    setMessage(null);

    try {
      const payload = {
        ...settings,
        installRoot: settings.installRoot.trim() === "" ? "." : settings.installRoot.trim()
      };
      const saved = await settingsWrite(payload);
      setSettings({
        ...emptySettings,
        ...saved,
        installRoot: saved.installRoot ?? "."
      });
      setStatus("idle");
      setMessage("设置已保存。");
    } catch {
      setStatus("error");
      setMessage("保存设置失败，请稍后重试。");
    }
  };

  return (
    <section>
      <h2>设置</h2>
      <p>读取和更新安装目录等基础配置。</p>
      <label htmlFor="install-root">安装根目录</label>
      <input
        id="install-root"
        value={settings.installRoot}
        onChange={(event) => {
          const nextInstallRoot = event.target.value;
          setSettings((current) => ({
            ...current,
            installRoot: nextInstallRoot
          }));
        }}
      />
      <button type="button" disabled={status === "loading" || status === "saving"} onClick={() => void save()}>
        {status === "saving" ? "保存中..." : "保存设置"}
      </button>
      {message ? <p role={status === "error" ? "alert" : undefined}>{message}</p> : null}
    </section>
  );
}
