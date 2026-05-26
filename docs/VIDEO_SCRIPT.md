# NutriScan — 10–15 分钟评估讲解稿

## 开场（1 分钟）

“大家好，我是【学生姓名】。这款应用叫 **NutriScan**，主题是 **食品与饮料**，方向是 **食物营养分析器**。技术栈是 **.NET MAUI**，采用 **MVVM** 和依赖注入。下面我按评分标准逐项演示。”

---

## 1. UI/UX 与可访问性（约 3 分钟）

### 演示

1. 打开应用，展示 **Shell 底部 Tab**：Home、Scan、Recipes、History、Settings。
2. 进入 **Settings**，打开 **Dark mode**，说明主题会 **持久化** 到 SQLite/Preferences。
3. 指出主色 `#1B5E20` 与白底对比度 **≥ 4.5:1**，符合 **WCAG 2.1 AA**。
4. 在系统设置中 **调大字体**，返回应用展示布局仍可读（动态字体）。
5. 打开 **TalkBack / Narrator**，聚焦按钮，说明已设置 `AutomationProperties.Name` 和 `HelpText`。
6. 指出按钮 **最小 44×44** 点触摸区域。

### 代码

- 打开 `Resources/Styles/AppStyles.xaml`、`Resources/Strings/AppResources.resx`。
- 强调：**所有界面均为 XAML**，无 C# 硬编码 UI。

### 性能

- 说明食谱缩略图经 `CachedFoodImage` + `ImageCacheService` **磁盘缓存**。
- `RecipesViewModel` 使用 `ObservableCollection` 与 `async/await` 加载 API。

---

## 2. 移动硬件（约 3 分钟）

### 高级 1：相机 + ONNX 机器学习

1. 进入 **Scan** 页，点击 **Capture photo**（真机）或 **Choose from gallery**（模拟器）。
2. 展示分类结果与置信度。
3. **讲解高级用法**：“这不仅调用相机，还把图像转为张量，用 **OnnxRuntime** 跑 `food_classifier.onnx`。相机是输入，ML 是推理，属于传感器 + AI 的复合高级场景。”
4. 打开 `Services/OnnxFoodMlService.cs` 展示 `InferenceSession` 与预处理。

### 高级 2：麦克风 + 语音识别

1. 打开任意食谱详情，点 **Start listening**，说一句备注。
2. 说明使用 **CommunityToolkit.Maui.Media** 的 `SpeechToText`，属于语音输入高级场景。

### 其他硬件

| 功能 | 演示 |
|------|------|
| 加速度计/摇动 | Home 页摇动 → 随机食谱 |
| GPS | Nearby → Refresh location |
| 手电筒 | Scan → Toggle flashlight |
| 触觉 | 保存设置或扫描成功时震动 |

### 不可用处理

- 模拟器无相机时，Scan 页显示 **Camera unavailable**，按钮禁用。
- 展示 `CameraService` 中的 `IsCameraAvailable` 检查。

---

## 3. 功能性（约 2 分钟）

- **基础控件**：搜索框、列表、Entry、Switch、对话框、**左滑删除**（Recipes/History）。
- **网络**：TheMealDB API（`MealApiService`），失败显示友好错误 + **Retry**。
- **SQLite**：扫描历史、收藏、设置（`DatabaseService`）。
- **图表**：Home 页 **Microcharts** 周卡路里柱状图。
- **集成**：扫描结果自动写入历史；摇动 → 随机餐；图表对应 SQLite 聚合数据。

---

## 4. 验证与错误处理（约 2 分钟）

1. Settings：输入非法卡路里（如 `50`）→ 显示 **“Calorie goal must be between 500 and 10000.”**
2. 食谱详情：份量输入 `0` → **Portion must be 1–999**。
3. 断网搜索食谱 → 网络错误 + Retry。
4. 打开 `MealApiService.cs`、`ScanViewModel.cs` 展示 `try/catch`、`HttpRequestException` 与日志。

---

## 5. 代码质量（约 2 分钟）

- **MVVM**：`Views/` 仅绑定，`ViewModels/` 含命令与状态。
- **DI**：`MauiProgram.cs` 注册 `ICameraService`、`IFoodMlService` 等。
- **XML 注释**：公共服务接口与公共方法带 `/// <summary>`。
- **命名**：PascalCase 类，`camelCase` 参数，`_camelCase` 私有字段。
- **可复用控件**：`RatingBar`、`NutritionLabel`。
- **资源文件**：所有 UI 字符串在 `AppResources.resx`。

---

## 6. 部署（约 1 分钟）

- 说明已在 **Android** 与 **Windows** 构建运行（展示 `dotnet build -f net9.0-android` / Windows）。
- 平板：`HomePage` 使用 `OnIdiom`/多列布局利用宽屏。

---

## 7. GitHub（约 1 分钟）

- 展示 **15+ commits**，消息如 “添加 ONNX 食物分类服务”、“修复地理位置权限提示”。
- 展示 **README**：硬件列表、运行步骤、开发计划、截图占位。

---

## 结束语（30 秒）

“NutriScan 覆盖了评估要求中的 UI/可访问性、四种以上硬件（含两项高级）、API+SQLite+图表、验证与异常处理、MVVM 架构，以及 Android 与 Windows 双平台部署。谢谢。”
