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

> You will need a BrowserStack account and a Percy project token to run these tests.
> Percy CLI is installed automatically by `run-tests.sh` if not already present.

---

## Step-by-Step: Clone and Run

### Step 1 — Clone the repository

```bash
git clone git@github.com:jostan30/CNunitPercyVisual.git
cd CNunitPercyVisual
cd WikipediaPercyTests
```

---

### Step 2 — Install NuGet packages

Restore all dependencies:

```bash
dotnet restore
```

---

### Step 3 — Upload your app to BrowserStack

> ⚠️ Run this command from the directory where your APK file is located (e.g. the `YPU` folder that contains `WikipediaSample.apk`). Using the wrong filename or running from a different directory will cause a `curl: (26) Failed to open/read local data` error.

```bash
# Navigate to the folder containing your APK first
cd /path/to/YPU

curl -u "YOUR_BROWSERSTACK_USERNAME:YOUR_BROWSERSTACK_ACCESS_KEY" \
  -X POST "https://api-cloud.browserstack.com/app-automate/upload" \
  -F "file=@WikipediaSample.apk"
```

You will get a response like:
```json
{ "app_url": "bs://abc123def456..." }
```

Copy the `app_url` value and update the `app:` field in `browserstack.yml`:

```yaml
app: bs://abc123def456...
```

---

### Step 4 — Add your BrowserStack credentials to browserstack.yml

Open `browserstack.yml` and replace the placeholder values with your own BrowserStack username and access key:

```yaml
userName: YOUR_BROWSERSTACK_USERNAME
accessKey: YOUR_BROWSERSTACK_ACCESS_KEY
```

> Get your credentials from: https://www.browserstack.com/accounts/settings

---

### Step 5 — Install the BrowserStack SDK (one-time setup)

```bash
dotnet add package BrowserStack.TestAdapter
dotnet build
```

> Only needed once per machine. This also installs the Percy CLI to `~/.browserstack/percy`.

---

### Step 6 — Add your Percy token to run-tests.sh, then execute

Open `run-tests.sh` and set your Percy token on this line:

```bash
PERCY_TOKEN_VALUE="app_YOUR_PERCY_TOKEN_HERE"
```

> Get your Percy token from: **percy.browserstack.com → your project → Settings**

Save the file, then run:

```bash
./run-tests.sh
```

> ⚠️ Running `dotnet test` alone will pass functional tests but Percy will **not** be initiated and no visual snapshots will be captured. Always use `./run-tests.sh`.

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
