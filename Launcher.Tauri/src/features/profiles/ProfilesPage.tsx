import { useEffect, useState } from "react";
import { profilesRead, profilesWrite } from "../../lib/bridge";
import type { ProfilesData } from "../../lib/contracts";

const emptyProfiles: ProfilesData = {
  defaultProfileId: null,
  profiles: []
};

export function ProfilesPage() {
  const [profiles, setProfiles] = useState<ProfilesData>(emptyProfiles);
  const [status, setStatus] = useState<"loading" | "idle" | "saving" | "error">("loading");
  const [message, setMessage] = useState<string | null>(null);

  useEffect(() => {
    let active = true;

    void profilesRead()
      .then((nextProfiles) => {
        if (!active) {
          return;
        }

        setProfiles({
          ...emptyProfiles,
          ...nextProfiles,
          profiles: Array.isArray(nextProfiles.profiles) ? nextProfiles.profiles : []
        });
        setStatus("idle");
      })
      .catch(() => {
        if (!active) {
          return;
        }

        setStatus("error");
        setMessage("读取档案失败，请检查桥接服务。");
      });

    return () => {
      active = false;
    };
  }, []);

  const save = async () => {
    setStatus("saving");
    setMessage(null);

    try {
      const saved = await profilesWrite(profiles);
      setProfiles({
        ...emptyProfiles,
        ...saved,
        profiles: Array.isArray(saved.profiles) ? saved.profiles : []
      });
      setStatus("idle");
      setMessage("档案已写回。 ");
    } catch {
      setStatus("error");
      setMessage("保存档案失败，请稍后重试。");
    }
  };

  return (
    <section>
      <h2>档案</h2>
      <p>默认档案: {profiles.defaultProfileId ?? "未设置"}</p>
      <p>档案数量: {profiles.profiles.length}</p>
      <button type="button" disabled={status === "loading" || status === "saving"} onClick={() => void save()}>
        {status === "saving" ? "保存中..." : "写回档案"}
      </button>
      {message ? <p role={status === "error" ? "alert" : undefined}>{message}</p> : null}
      <pre>{JSON.stringify(profiles, null, 2)}</pre>
    </section>
  );
}
