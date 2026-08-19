# Wikipedia App Percy Visual Tests — NUnit + BrowserStack

Automated mobile tests for the Wikipedia Android app using **NUnit**, **Appium**, and **App Percy** for visual regression testing on BrowserStack App Automate.

---

## Project Structure

```
WikipediaPercyTests/
├── WikipediaTests.cs          # NUnit test file (4 test cases)
├── browserstack.yml           # BrowserStack config (devices, Percy enabled)
├── WikipediaPercyTests.csproj # .NET project file
├── .gitignore                 # Git ignore rules
└── README.md                  # This file
```

---

## Prerequisites

Make sure you have the following installed before you begin:

| Requirement | Notes |
|---|---|
| .NET SDK 10.0+ | https://dotnet.microsoft.com/download |
| Node.js + npm | https://nodejs.org (required for Percy CLI) |
| Git | https://git-scm.com |

> No BrowserStack account setup needed — credentials are already in `browserstack.yml`.
> Percy CLI is installed automatically by `run-tests.sh` if not already present.

---

## Step-by-Step: Clone and Run

### Step 1 — Clone the repository

```bash
git clone <your-github-repo-url>
cd WikipediaPercyTests
```

---

### Step 2 — Install NuGet packages

Restore all dependencies:

```bash
dotnet restore
```

---

### Step 3 — Install the BrowserStack SDK (one-time setup)

```bash
dotnet add package BrowserStack.TestAdapter
dotnet build
dotnet browserstack-sdk setup
```

> This sets up the BrowserStack SDK tool locally. Only needed once per machine.

---

### Step 4 — Build the project

```bash
dotnet build
```

Expected output:
```
Build succeeded.
0 Error(s)
```

---

### Step 5 — Set the App Percy token

```bash
export PERCY_TOKEN=app_13815b667557cc....
```

> This token is tied to the Percy project for this repo. Keep it set in your shell session before running tests.

---

### Step 6 — Run the tests with Percy

A convenience script is included that sets the Percy token and wraps `dotnet test` with `percy exec` automatically:

```bash
./run-tests.sh
```

This is equivalent to:
```bash
export PERCY_TOKEN=app_13815b667557cc482516a282966de770d5b5cad7b9639b99e44015046bb5c2d8
~/.browserstack/percy exec -- dotnet test
```

> ⚠️ Running `dotnet test` alone will pass functional tests but Percy will **not** be initiated and no visual snapshots will be captured. Always use `./run-tests.sh` or the `percy exec` command above.

---

### Step 7 — View results

**Functional test results** are printed in the terminal:
```
Passed!  - Failed: 0, Passed: 5, Skipped: 0, Total: 5
```

**Percy visual diffs** are available at the build URL printed at the end:
```
[percy] Finalized build: https://percy.io/...
```

---

## Run a Specific Test

```bash
~/.browserstack/percy exec -- dotnet test --filter "FullyQualifiedName~SearchWikipedia_ShouldShowResults"
```

Available test names:
- `SearchWikipedia_ShouldShowResults`
- `ExploreFeed_ShouldDisplaySearchBar`
- `OverflowMenu_ShouldOpen`
- `MyListsTab_ShouldBeAccessible`

---

## Test Cases

| Test | Description | Percy Snapshots |
|---|---|---|
| `SearchWikipedia_ShouldShowResults` | Searches "BrowserStack", asserts results list appears | Wikipedia Home Screen, Search Results List |
| `ExploreFeed_ShouldDisplaySearchBar` | Verifies Explore feed loads with search bar | Explore Feed |
| `OverflowMenu_ShouldOpen` | Opens toolbar overflow menu, asserts Settings item visible | Home Before Overflow Menu, Overflow Menu Open |
| `MyListsTab_ShouldBeAccessible` | Taps My Lists bottom-nav tab, asserts tab visible | My Lists Tab |

---

## Devices (browserstack.yml)

| Device | OS Version |
|---|---|
| Samsung Galaxy S22 Ultra | Android 12.0 |
| Google Pixel 7 Pro | Android 13.0 |
| OnePlus 9 | Android 11.0 |

---

## Percy Visual Testing

Percy capture mode is set to **`testcase`** — one screenshot per test case at the end of each test.

To change capture mode, edit `browserstack.yml`:

```yaml
percyCaptureMode: testcase   # options: auto | testcase | click | screenshot | manual
```

---

## Troubleshooting

| Issue | Fix |
|---|---|
| `dotnet: command not found` | Install .NET SDK from https://dotnet.microsoft.com/download |
| `percy: command not found` | Run `dotnet build` first — the BrowserStack SDK installs Percy CLI to `~/.browserstack/percy` |
| Percy snapshots not captured | Ensure `PERCY_TOKEN` is exported AND you used `percy exec -- dotnet test` |
| `NullReferenceException` in SetUp | Check that `browserstack.yml` credentials are valid |
| Build warning about `tool-manifest` | Safe to ignore — SDK creates its own manifest |