# NutriScan — Food & Drink Nutrition Analyzer

**Author:** Siya Yu  
**Course:** Mobile Computing  
**GitHub:** https://github.com/HubuManchester/fooddrink-Youo-O  
**Stack:** .NET 8 MAUI (Android + Windows)

## Concept

NutriScan is a **food nutrition analyzer** for the Food & Drink assessment theme. Users photograph meals, run **ONNX machine learning** classification, track calories in SQLite, browse recipes from **TheMealDB**, shake the phone for a random meal, use **voice notes** and **text-to-speech** on recipes, and find **nearby restaurants** via GPS.

## Hardware features (7 types)

| Feature | Type | How to test |
|--------|------|-------------|
| **Camera + ONNX ML** | Advanced | Scan tab → Capture photo → see label & confidence |
| **Microphone + speech-to-text** | Advanced | Recipe detail → Start listening → speak a note |
| **Text-to-speech** | Standard | Recipe detail → Read aloud |
| **Accelerometer (shake)** | Standard | Home tab → shake device → random meal appears |
| **Geolocation (GPS)** | Standard | Home → Nearby → Refresh location |
| **Flashlight** | Standard | Scan tab → Toggle flashlight (physical device) |
| **Haptic feedback** | Standard | Save settings or complete a scan (vibration on phone) |

### Why camera + ONNX is “advanced”

The pipeline captures a bitmap, resizes to 224×224, normalizes RGB tensors, and runs **Microsoft.ML.OnnxRuntime** inference. This combines a hardware sensor (camera) with on-device ML—not just taking a photo.

Regenerate the bundled demo model:

```powershell
# From repository root (uses Tsinghua PyPI mirror if needed)
.\scripts\generate-food-onnx.ps1
```

Or manually:

```powershell
python -m venv .venv
.\.venv\Scripts\pip install onnx -i https://pypi.tuna.tsinghua.edu.cn/simple
.\.venv\Scripts\python scripts/generate-food-onnx.py
```

Labels are in `FoodApp/Resources/Raw/food_labels.json`. Without the ONNX file, the app uses a documented **heuristic fallback** so emulators still demo the flow.

## Accessibility & orientation

- **WCAG 2.1 AA:** contrast palette, screen reader labels, dark mode, in-app font scale — see [docs/ACCESSIBILITY.md](docs/ACCESSIBILITY.md)
- **Help:** Tap **?** on any page, or Settings → User guide
- **Orientation:** Portrait and landscape supported; tablet layouts use wider padding and two-column recipe lists

## Development plan (iterations)

| Week | Delivered |
|------|-----------|
| 1 | Solution scaffold, Shell navigation, MVVM + DI |
| 2 | TheMealDB API, SQLite favorites & scan history |
| 3 | ONNX service, camera scan page, image cache |
| 4 | Speech notes, shake random meal, geolocation nearby |
| 5 | Microcharts weekly calories, settings/theme, accessibility |
| 6 | Validation, error handling, README, Windows + Android deploy |

## Screenshots

| Home | Scan | Recipes |
|------|------|---------|
| _docs/screenshots/home.png_ | _docs/screenshots/scan.png_ | _docs/screenshots/recipes.png_ |

## Demo video (Screencast)

**Required:** 10–15 minute walkthrough uploaded to mmutube and linked here.

Recording link: `https://mmutube.mmu.ac.uk/YOUR_VIDEO_ID` _(replace after upload)_

Suggested outline: see [docs/评分标准检查报告.md](docs/评分标准检查报告.md) → Screencast 建议演示大纲

## Run instructions

### Prerequisites

- Visual Studio 2022 17.8+ with **.NET MAUI** workload
- **.NET 8 SDK**

### Visual Studio 2022（推荐）

1. Open `FoodApp.sln`
2. Set **FoodApp** as startup project
3. Select **Windows Machine** or **Android Emulator** → press **F5**

### Android (API 29+ emulator or device)

```powershell
cd d:\MobileComputing\MobileComputing\FoodApp
dotnet build -t:Run -f net8.0-android
```

Grant camera, microphone, and location when prompted.

**Emulator Scan demo:** add food photos under `docs/samples/`, start the emulator, then:

```powershell
# From repository root
.\scripts\push-demo-images.ps1
```

In the app: **Scan → Browse image file → Downloads**.

### Windows (second platform)

```powershell
cd d:\MobileComputing\MobileComputing\FoodApp
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```

### Solution build

```powershell
cd d:\MobileComputing\MobileComputing
dotnet build FoodApp.sln
```

## Project structure

```
FoodApp.sln
FoodApp/
  MauiProgram.cs          # DI registration
  AppShell.xaml           # Tab navigation + routes
  Models/                 # Meal, ScanRecord, settings
  ViewModels/             # MVVM (CommunityToolkit.Mvvm)
  Views/                  # All UI in XAML
  Services/               # API, DB, ML, hardware
  Controls/               # RatingBar, NutritionLabel, CachedFoodImage
  Resources/Strings/      # AppResources.resx (no hardcoded UI strings)
  Resources/Raw/          # food_labels.json, food_classifier.onnx
docs/
  ACCESSIBILITY.md        # WCAG 2.1 notes for screencast
  评分标准检查报告.md      # Rubric self-assessment
```

## NuGet packages

- CommunityToolkit.Mvvm, CommunityToolkit.Maui  
- sqlite-net-pcl  
- Microcharts.Maui  
- Microsoft.ML.OnnxRuntime  
- Microsoft.Extensions.Http  

## GitHub

21+ incremental commits (features, fixes, docs)—not a single bulk upload.
