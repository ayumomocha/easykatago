export interface SettingsData {
  installRoot: string;
  [key: string]: unknown;
}

export interface ProfileData {
  profileId: string;
  displayName?: string;
  [key: string]: unknown;
}

export interface ProfilesData {
  defaultProfileId: string | null;
  profiles: ProfileData[];
  [key: string]: unknown;
}
