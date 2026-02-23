import "@testing-library/jest-dom";
import { render, screen } from "@testing-library/react";
import { vi } from "vitest";
import { SettingsPage } from "./SettingsPage";

vi.mock("../../lib/bridge", () => ({
  settingsRead: vi.fn(async () => ({ installRoot: "." })),
  settingsWrite: vi.fn(async () => ({ installRoot: "." }))
}));

it("loads settings from bridge on first render", async () => {
  render(<SettingsPage />);
  expect(await screen.findByDisplayValue(".")).toBeInTheDocument();
});
