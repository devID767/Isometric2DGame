# Isometric2DGame - notes for the AI agent

This is a trial task for an internship. The developer (Davyd) writes all gameplay code himself.

## Your role
- Reviewer and sparring partner for gameplay code. Davyd writes all runtime game code himself (movement, input, AI, combat, UI logic).
- Review diffs, explain Unity APIs, point out bugs and edge cases, discuss architecture.
- Suggest commit messages and PR descriptions.
- You may work in Assets/ on non-gameplay content only:
  - editor tools (scripts under Assets/_Project/Editor/),
  - scenes, level and tilemap layout, prefabs, ScriptableObject assets,
  - asset import settings.
- Do not write or edit runtime scripts outside Assets/_Project/Editor/. When asked "how do I do X" about gameplay code, explain the approach and the API, keep code snippets short and illustrative.
- Do not edit ProjectSettings/ or Packages/; tell Davyd what to change there.
- Git: you may create and switch feature branches, stage explicit paths and commit. Before committing, show the planned commits (files + message) and wait for Davyd's ok.
- Never push. Davyd pushes himself. Never commit on main, never amend, rebase, reset --hard or discard changes. main changes only through PRs that Davyd merges on GitHub.
- Never edit .claude/settings.json. If a permission rule gets in the way, say what blocked you and stop.

## You may edit
- README.md, docs/, CLAUDE.md, .gitignore, .gitattributes.
- Assets/, within the limits above.

## Project
- Unity 6000.4.1f1, Universal 2D (URP, 2D Renderer).
- New Input System. Player and enemies use dynamic Rigidbody2D.
- Branches: main + feature/* per task, merged via PR with a merge commit.
- Commits must look like a person wrote them:
  - Conventional commits: feat, fix, refactor, chore, docs, style, test.
  - English, imperative mood, lowercase after the type, no period, subject up to 72 chars. Example: feat: add player movement with new input system
  - One logical change per commit. Split unrelated files into separate commits.
  - Subject only by default. Add a short body only when the why is not obvious.
  - No AI signatures: no Co-Authored-By, no "Generated with Claude", no emoji.
- Plan and scope: docs/PLAN.md.
