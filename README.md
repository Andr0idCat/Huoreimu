# Huoreimu - Slay the Spire 2 Mod

**作者：** 古明地绫（GitHub: [Andr0idCat](https://github.com/Andr0idCat)）

**状态：** 🚧 开发中

## 简介

《Huoreimu》是一个针对《杀戮尖塔2》（Slay the Spire 2）的 mod，旨在为游戏添加新的内容、机制或角色（具体功能待定）。本仓库包含该 mod 的完整源代码、资源文件及构建配置。

## 下载

前往 [Releases 页面](https://github.com/Andr0idCat/huoreimu/releases) 下载最新版本。

## 依赖

本 Mod 依赖于 [BaseLib](https://github.com/Alchyr/BaseLib-StS2)，运行前请确保已安装。

## 配置

考虑到每个人的开发环境不同，拉取仓库后需要在目录下新建LocalConfig.targets文件，在其中填写如下内容：
```xml
<Project>
    <PropertyGroup>
        <!-- STS2Path改成你的杀戮尖塔2安装路径 -->
        <STS2Path>D:\Steam\steamapps\common\Slay the Spire 2</STS2Path>
        <!-- GodotExe是你的Godot路径 -->
        <GodotExe>E:\Godot\Godot_v4.5.1-stable_mono_win64\Godot_v4.5.1-stable_mono_win64.exe</GodotExe>
        <!-- Sts2DataDir不需要填写 -->
        <Sts2DataDir>$(STS2Path)/data_sts2_windows_x86_64</Sts2DataDir>
    </PropertyGroup>
</Project>
```
将文件中的STS2Path和GodotExe改成你本地的路径，Sts2DataDir不需要修改。

## 构建

```bash
dotnet build -t:ExportPck
```
构建时需要指定 `-t:ExportPck` 参数，以便通过 Godot 生成对应的资源包（.pck）。

### 特效清单自动收集

本仓库包含一个构建工具 `tools/VfxManifestGenerator/`，会在 `ExportPck` 之前自动扫描 `Huoreimu/Scenes/vfx/` 目录下的所有 `.tscn` 文件，生成 `vfx_manifest.json` 清单。

运行时 `VfxUtil.LoadVfx()` 读取该清单并预加载所有特效场景。添加新 `.tscn` 文件后重新构建即可自动更新，无需手动维护。


## 贡献

目前 mod 为个人项目，暂不接受外部贡献。如有建议或 Bug 报告，欢迎提交 [Issue](https://github.com/Andr0idCat/huoreimu/issues)。

## 许可

本项目仅供学习与交流使用，未经许可禁止用于商业用途。

---

**注意：** 此 mod 仍在早期开发阶段，功能可能不完整，请耐心等待后续更新。