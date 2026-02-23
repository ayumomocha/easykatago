import { useState } from "react";
import { DiagnosticsPage } from "./features/diagnostics/DiagnosticsPage";
import { LogsPage } from "./features/logs/LogsPage";
import { ProfilesPage } from "./features/profiles/ProfilesPage";
import { SettingsPage } from "./features/settings/SettingsPage";
import "./styles/app.css";

type PageId = "settings" | "profiles" | "logs" | "diagnostics";

const pageItems: Array<{ id: PageId; label: string }> = [
  { id: "settings", label: "设置" },
  { id: "profiles", label: "档案" },
  { id: "logs", label: "日志" },
  { id: "diagnostics", label: "诊断" }
];

export function App() {
  const [activePage, setActivePage] = useState<PageId>("settings");

  return (
    <div className="app-shell">
      <aside>
        {pageItems.map((item) => (
          <button
            key={item.id}
            type="button"
            aria-pressed={activePage === item.id}
            onClick={() => setActivePage(item.id)}
          >
            {item.label}
          </button>
        ))}
      </aside>
      <main>
        {activePage === "settings" ? <SettingsPage /> : null}
        {activePage === "profiles" ? <ProfilesPage /> : null}
        {activePage === "logs" ? <LogsPage /> : null}
        {activePage === "diagnostics" ? <DiagnosticsPage /> : null}
      </main>
    </div>
  );
}
