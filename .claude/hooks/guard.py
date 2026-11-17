#!/usr/bin/env python3
"""Reviewer guard for Claude Code (PreToolUse hook).

Davyd writes the game code and pushes. The agent reviews, branches and commits.
This guard stops accidents, it is not a sandbox: a determined bypass through
scripts or git aliases is possible. Real protection of main is GitHub branch protection.

Exit code 2 = block, stderr goes back to the agent.
"""
import json
import os
import re
import shlex
import subprocess
import sys

HOME = os.path.expanduser("~")
PROJECT = os.path.realpath(os.environ.get("CLAUDE_PROJECT_DIR") or os.getcwd())
CODE_DIRS = ("Assets", "ProjectSettings", "Packages")

FILE_WRITERS = {"rm", "rmdir", "mv", "cp", "touch", "mkdir", "ln", "chmod", "chown", "tee",
                "rsync", "unzip", "tar", "patch", "dd", "install", "truncate", "ditto", "trash",
                "xattr", "shred", "curl", "wget", "open"}
INTERPRETERS = {"python", "python3", "node", "ruby", "perl", "osascript", "php", "deno", "bun",
                "swift", "pwsh", "eval", "exec", "xargs", "find", "awk", "env", "sudo", "nohup",
                "time", "command", "builtin"}
SHELLS = {"bash", "sh", "zsh", "dash", "fish"}
GH_READONLY = {("pr", "view"), ("pr", "list"), ("pr", "diff"), ("pr", "status"), ("pr", "checks"),
               ("repo", "view"), ("issue", "view"), ("issue", "list")}


def block(reason):
    print(f"Blocked by reviewer guard: {reason}. Do not try to work around it. "
          "Tell Davyd what you wanted to do and stop.", file=sys.stderr)
    sys.exit(2)


def resolve(path, cwd):
    path = os.path.expanduser(path)
    if not os.path.isabs(path):
        path = os.path.join(cwd, path)
    return os.path.realpath(path)


def protected_reason(abs_path):
    """Why this path must not be written to, or None."""
    p = abs_path
    for d in CODE_DIRS:
        root = os.path.join(PROJECT, d)
        if p == root or p.startswith(root + os.sep):
            return f"{d}/ is written by Davyd only"
    claude_proj = os.path.join(PROJECT, ".claude")
    if p == claude_proj or p.startswith(claude_proj + os.sep):
        return ".claude/ (hooks and settings) is edited by Davyd only"
    git_dir = os.path.join(PROJECT, ".git")
    if p == git_dir or p.startswith(git_dir + os.sep):
        return ".git/ must not be edited directly"
    claude_home = os.path.join(HOME, ".claude")
    if p == claude_home or p.startswith(claude_home + os.sep):
        if re.search(r"/projects/[^/]+/memory(/|$)", p[len(claude_home):]):
            return None
        return "~/.claude/ (global settings) is edited by Davyd only"
    if os.path.basename(p) in (".gitconfig",) or p.startswith(os.path.join(HOME, ".config", "git")):
        return "git config is edited by Davyd only"
    return None


def mentions_protected(text, cwd):
    """Loose text scan for scripts: does the text name a protected location?"""
    names = r"(Assets|ProjectSettings|Packages|\.claude|\.git(?!ignore|attributes)|\.gitconfig)"
    return re.search(r"(^|[\s/'\"=:(,])" + names + r"([/\s'\"),]|$)", text) is not None


# ---------------- git ----------------

GIT_GLOBAL_WITH_VALUE = {"-C", "-c", "--git-dir", "--work-tree", "--namespace", "--exec-path",
                         "--super-prefix", "--config-env"}


def current_branch(cwd):
    try:
        return subprocess.run(["git", "-C", cwd, "rev-parse", "--abbrev-ref", "HEAD"],
                              capture_output=True, text=True, timeout=5).stdout.strip()
    except Exception:
        return ""


def split_opts(args, with_value):
    """Return (options, positionals). Options with values swallow the next token."""
    opts, pos, i, after_dd = [], [], 0, False
    while i < len(args):
        a = args[i]
        if after_dd:
            pos.append(a)
        elif a == "--":
            after_dd = True
            opts.append(a)
        elif a.startswith("--"):
            opts.append(a.split("=", 1)[0])
            if "=" not in a and a in with_value:
                i += 1
        elif a.startswith("-") and len(a) > 1:
            opts.append(a)
            # short cluster like -am: value-taking letter at the end swallows next token
            letters = a[1:]
            for j, ch in enumerate(letters):
                if "-" + ch in with_value:
                    if j == len(letters) - 1:
                        i += 1
                    break
        else:
            pos.append(a)
        i += 1
    return opts, pos


def short_has(opts, letter, with_value=()):
    for o in opts:
        if o.startswith("-") and not o.startswith("--"):
            for ch in o[1:]:
                if ch == letter:
                    return True
                if "-" + ch in with_value:
                    break
    return False


def check_git(tokens, cwd, ctx):
    i = 1
    while i < len(tokens) and tokens[i].startswith("-"):
        t = tokens[i]
        if t == "-c" or t.startswith("--config-env"):
            block("git -c / --config-env overrides are not allowed")
        if t == "-C" and i + 1 < len(tokens):
            cwd = resolve(tokens[i + 1], cwd)
        if t in GIT_GLOBAL_WITH_VALUE and "=" not in t:
            i += 1
        i += 1
    if i >= len(tokens):
        return
    sub, args = tokens[i], tokens[i + 1:]

    if sub in ("push", "send-pack", "rebase", "merge", "cherry-pick", "clean", "filter-branch",
               "filter-repo", "update-ref", "reflog", "gc", "prune", "replace", "am", "apply",
               "config", "remote", "worktree", "submodule", "notes", "symbolic-ref", "pull"):
        block(f"git {sub} is not allowed for the agent")

    if sub == "commit":
        wv = {"-m", "--message", "-F", "--file", "-C", "--reuse-message", "-c", "--reedit-message",
              "--author", "--date", "-t", "--template", "--cleanup", "--fixup", "--squash",
              "--trailer", "-S", "--gpg-sign", "--pathspec-from-file"}
        opts, pos = split_opts(args, wv)
        if "--amend" in opts:
            block("git commit --amend")
        if "--all" in opts or short_has(opts, "a", wv):
            block("git commit -a/--all (stage explicit paths with git add)")
        if "--no-verify" in opts or short_has(opts, "n", wv):
            block("git commit --no-verify")
        if pos or "--include" in opts or "--only" in opts or "--pathspec-from-file" in opts:
            block("git commit with paths (stage with git add, then commit)")
        if ctx["branch_changed"]:
            block("branch change and commit in one command (run them separately)")
        if current_branch(cwd) in ("main", "master", "HEAD"):
            block("commit on main (create a feature branch first)")
        return

    if sub == "add":
        opts, pos = split_opts(args, {"--chmod", "--pathspec-from-file"})
        if set(opts) & {"--all", "-A", "--update", "-u", "--no-ignore-removal", "--force", "-f",
                        "--pathspec-from-file"} or short_has(opts, "A") or short_has(opts, "u") \
                or short_has(opts, "f"):
            block("bulk or forced git add (stage explicit file paths)")
        if not pos:
            block("git add without paths")
        for p in pos:
            if p.startswith(":") or any(c in p for c in "*?[") or resolve(p, cwd) in (PROJECT, os.path.dirname(PROJECT)):
                block(f"git add {p} is too broad (stage explicit file paths)")
            if os.path.isdir(resolve(p, cwd)) and resolve(p, cwd) == resolve(cwd, cwd):
                block(f"git add {p} is too broad (stage explicit file paths)")
        return

    if sub == "reset":
        opts, pos = split_opts(args, {"--pathspec-from-file"})
        if set(opts) & {"--hard", "--merge", "--keep", "--soft", "--mixed"}:
            block("git reset that moves the branch or discards changes (only unstaging is allowed)")
        if "--" not in opts and pos and pos[0] != "HEAD" and not os.path.exists(resolve(pos[0], cwd)):
            block(f"git reset {pos[0]} looks like moving the branch (only unstaging paths is allowed)")
        return

    if sub == "restore":
        opts, _ = split_opts(args, {"-s", "--source", "--pathspec-from-file"})
        staged = "--staged" in opts or short_has(opts, "S")
        worktree = "--worktree" in opts or short_has(opts, "W")
        if not staged or worktree:
            block("git restore that discards working tree changes (only --staged is allowed)")
        return

    if sub == "checkout":
        opts, pos = split_opts(args, {"-b", "-B", "--orphan", "--conflict", "--pathspec-from-file"})
        if set(opts) & {"-f", "--force", "-B", "--", "--orphan", "-p", "--patch", "--ours",
                        "--theirs", "--merge", "-m", "--pathspec-from-file", "--overwrite-ignore"} \
                or short_has(opts, "f") or short_has(opts, "B"):
            block("git checkout that can discard changes or reset a branch")
        if "-b" in opts:
            ctx["branch_changed"] = True
            return
        if len(pos) > 1:
            block("git checkout <commit> <path> overwrites files")
        if pos and (pos[0] in (".", "./") or os.path.exists(resolve(pos[0], cwd))):
            block(f"git checkout {pos[0]} discards changes in files")
        ctx["branch_changed"] = True
        return

    if sub == "switch":
        opts, _ = split_opts(args, {"-c", "-C", "--create", "--force-create", "--orphan", "--conflict"})
        if set(opts) & {"--discard-changes", "-f", "--force", "-C", "--force-create", "--orphan",
                        "--merge", "-m"} or short_has(opts, "f") or short_has(opts, "C"):
            block("git switch that can discard changes or reset a branch")
        ctx["branch_changed"] = True
        return

    if sub == "branch":
        opts, pos = split_opts(args, {"-u", "--set-upstream-to", "--track"})
        bad = {"-D", "-f", "--force", "-M", "-m", "--move", "-C", "-c", "--copy",
               "--delete", "-d", "--edit-description", "--unset-upstream", "-u", "--set-upstream-to"}
        if set(opts) & bad or any(short_has(opts, ch) for ch in "DfMmCcd"):
            block("git branch that deletes, moves or overwrites branches")
        if len(pos) > 1:
            block("git branch <name> <start-point> (create branches from the current one)")
        return

    if sub == "stash":
        if args and args[0] in ("drop", "clear", "pop", "apply", "branch", "store"):
            block(f"git stash {args[0]}")
        return

    if sub == "tag":
        opts, pos = split_opts(args, {"-m", "-F", "-u"})
        if set(opts) & {"-d", "--delete", "-f", "--force"} or short_has(opts, "d") or short_has(opts, "f"):
            block("git tag delete/force")
        return

    # read-only and other commands (status, log, diff, show, blame, revert, ls-files...) pass


# ---------------- shell ----------------

def segments(command):
    """Split a shell command into simple commands. Returns list of (tokens, redirect_targets)."""
    lex = shlex.shlex(command, posix=True, punctuation_chars=";&|<>()")
    lex.whitespace_split = True
    lex.commenters = ""
    toks = list(lex)
    segs, cur, redir, i = [], [], [], 0
    while i < len(toks):
        t = toks[i]
        if t and set(t) <= set(";&|()") :
            if cur or redir:
                segs.append((cur, redir))
            cur, redir = [], []
        elif t and set(t) <= set("<>&") :
            if ">" in t and i + 1 < len(toks):
                redir.append(toks[i + 1])
            if t.startswith("<<"):
                raise ValueError("heredoc")
            i += 1
        else:
            cur.append(t)
        i += 1
    if cur or redir:
        segs.append((cur, redir))
    return segs


def strip_prefix(tokens):
    """Drop leading VAR=value assignments."""
    while tokens and re.match(r"^[A-Za-z_][A-Za-z0-9_]*=", tokens[0]):
        tokens = tokens[1:]
    return tokens


def check_bash(command, cwd, depth=0):
    if depth > 3:
        block("nested shell too deep")
    if re.search(r"<<|\$\(|`", command):
        # heredocs and command substitution: fall back to a loose scan
        if mentions_protected(command, cwd) and re.search(
                r"(>|\b(rm|mv|cp|tee|touch|sed|perl|python3?|node|ruby|dd|ln|chmod|truncate)\b)", command):
            block("heredoc or command substitution that touches a protected location")
        if re.search(r"\bgit\b[^|;&]*\b(push|commit|reset|restore|checkout|switch|branch|rebase|merge|clean|config)\b", command) \
                or re.search(r"\bgh\b", command):
            block("git or gh inside heredoc or command substitution (run git commands plainly)")
        return
    try:
        segs = segments(command)
    except ValueError:
        block("could not parse the command")
    ctx = {"branch_changed": False}
    for tokens, redirs in segs:
        for r in redirs:
            if r in ("/dev/null", "/dev/stdout", "/dev/stderr") or r.isdigit() or r.startswith("&"):
                continue
            why = protected_reason(resolve(r, cwd))
            if why:
                block(f"redirect into {r}: {why}")
        tokens = strip_prefix(tokens)
        if not tokens:
            continue
        cmd = os.path.basename(tokens[0])
        args = tokens[1:]
        if cmd == "cd":
            cwd = resolve(args[0] if args else HOME, cwd)
            continue
        if cmd == "git":
            check_git(tokens, cwd, ctx)
            continue
        if cmd == "gh":
            if len(args) >= 2 and (args[0], args[1]) in GH_READONLY:
                continue
            block("gh is read-only for the agent (Davyd pushes and creates PRs himself)")
        if cmd in SHELLS:
            if "-c" in args and args.index("-c") + 1 < len(args):
                check_bash(args[args.index("-c") + 1], cwd, depth + 1)
                continue
            if any(a.endswith((".sh", ".bash", ".zsh")) for a in args):
                block("running shell scripts is not allowed (run commands directly)")
            continue
        if cmd.endswith(".sh"):
            block("running shell scripts is not allowed (run commands directly)")
        if cmd in FILE_WRITERS or (cmd == "sed" and any(a.startswith("-i") or a == "--in-place" or re.match(r"^-[a-zA-Z]*i", a) for a in args)) \
                or (cmd == "perl" and any(re.match(r"^-[a-zA-Z]*i", a) for a in args)):
            for a in args:
                if a.startswith("-"):
                    continue
                why = protected_reason(resolve(a, cwd))
                if why:
                    block(f"{cmd} {a}: {why}")
            if cmd in ("curl", "wget", "open"):
                pass
            continue
        if cmd in INTERPRETERS or cmd in ("sed", "perl"):
            text = " ".join(args)
            if mentions_protected(text, cwd):
                block(f"{cmd} with a protected location in its arguments")
            if cmd in ("xargs", "find", "env", "sudo", "nohup", "time", "command", "builtin", "exec", "eval"):
                inner = [a for a in args if not a.startswith("-")]
                if any(os.path.basename(x) in ({"git", "gh"} | FILE_WRITERS) for x in inner):
                    block(f"{cmd} wrapping git, gh or file writers (run them directly)")
            # python3 some_script.py: check the script text
            for a in args:
                if a.endswith((".py", ".js", ".rb", ".pl", ".ts")):
                    try:
                        src = open(resolve(a, cwd), encoding="utf-8", errors="ignore").read()
                    except OSError:
                        continue
                    if mentions_protected(src, cwd) or re.search(r"\bsubprocess|os\.system|child_process|\bgit\b", src):
                        block(f"script {a} touches git, processes or protected locations")


def main():
    try:
        data = json.load(sys.stdin)
    except Exception:
        block("hook input was not valid JSON")
    tool = data.get("tool_name", "")
    inp = data.get("tool_input", {}) or {}
    cwd = os.path.realpath(data.get("cwd") or PROJECT)

    if tool in ("Write", "Edit", "MultiEdit", "NotebookEdit"):
        path = inp.get("file_path") or inp.get("notebook_path") or ""
        why = protected_reason(resolve(path, cwd)) if path else None
        if why:
            block(f"{tool} {path}: {why}")
        return

    if tool == "Bash":
        check_bash(inp.get("command", ""), cwd)


if __name__ == "__main__":
    try:
        main()
    except SystemExit:
        raise
    except Exception as e:  # fail closed: a crash in the guard must block, not allow
        block(f"guard error ({type(e).__name__}: {e})")
    sys.exit(0)
