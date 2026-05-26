# NutriScan — Food & Drink Nutrition Analyzer

**Author:** 学生姓名  
**Course:** Mobile Computing  
**Stack:** .NET 9 MAUI (API-compatible with .NET 8 MAUI patterns)

## Concept

NutriScan is a **food nutrition analyzer** for the Food & Drink assessment theme. Users photograph meals, run **ONNX machine learning** classification, track calories in SQLite, browse recipes from **TheMealDB**, shake the phone for a random meal, use **voice notes** on recipes, and find **nearby restaurants** via GPS.

## Hardware features

| Feature | Type | How to test |
|--------|------|-------------|
| **Camera + ONNX ML** | Advanced | Scan tab → Capture photo → see label & confidence |
| **Microphone + speech-to-text** | Advanced | Open a recipe → Start listening → speak a note |
| **Accelerometer (shake)** | Standard | Home tab → shake device → random meal appears |
| **Geolocation (GPS)** | Standard | Home → Nearby → Refresh location |
| **Flashlight** | Standard | Scan tab → Toggle flashlight (physical device) |
| **Haptic feedback** | Standard | Save settings or complete a scan (vibration on phone) |

### Why camera + ONNX is “advanced”

The pipeline captures a bitmap, resizes to 224×224, normalizes RGB tensors, and runs **Microsoft.ML.OnnxRuntime** inference. This combines a hardware sensor (camera) with on-device ML—not just taking a photo.

### ONNX model

Place a trained `food_classifier.onnx` in `FoodApp/Resources/Raw/`. Labels are in `food_labels.json`. Without the ONNX file, the app uses a documented **heuristic fallback** so emulators still demo the flow.

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

## Demo video

Recording link: _(add your 10–15 min walkthrough URL here)_

## Run instructions

### Prerequisites

- Visual Studio 2022 17.12+ with **.NET MAUI** workload
- .NET 9 SDK (or retarget `FoodApp.csproj` to `net8.0-*` if you use .NET 8 SDK only)

### Android (API 29+ emulator or device)

```powershell
cd d:\MobileComputing\FoodApp
dotnet build -t:Run -f net9.0-android
```

Grant camera, microphone, and location when prompted.

### Windows (second platform)

```powershell
cd d:\MobileComputing\FoodApp
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

### Solution build

```powershell
cd d:\MobileComputing
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
  Resources/Raw/          # food_labels.json, food_classifier.onnx (optional)
```

## NuGet packages

- CommunityToolkit.Mvvm, CommunityToolkit.Maui  
- sqlite-net-pcl  
- Microcharts.Maui  
- Microsoft.ML.OnnxRuntime  
- Microsoft.Extensions.Http  

## GitHub

Commit history includes 15+ incremental commits (features, fixes, docs)—not a single bulk upload.
