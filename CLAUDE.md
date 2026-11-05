# CLAUDE.md

@AGENTS.md

## Claude Code notes

- **Where guidance goes:** put guidance that applies to all agents in `AGENTS.md`. Keep only Claude Code-specific notes in this file.
- **Area rules:** before editing the listener, Dispatcher or async code, read `.cursor/rules/unity-threading.mdc`. Before editing `PNConfigAsset` or its inspector, read `.cursor/rules/pnconfig-asset.mdc`.
- **Skill:** `update-csharp-sdk`, for importing a new C# SDK DLL.
- **Compiling:** you can't compile or run tests without the Unity Editor. Ask the user to verify in Unity, and say what you couldn't check.
- **Personal files:** overrides go in `.claude/settings.local.json` and notes in `CLAUDE.local.md`; both are git-ignored.
