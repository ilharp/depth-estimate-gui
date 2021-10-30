> 中文用户可直接左转 [B 站动态](https://t.bilibili.com/587366069405400548)

<div align="center">

<img src="https://raw.githubusercontent.com/depth-estimate-gui/depth-estimate-gui/master/assets/icon.png" width="200px">

<h1>Depth Estimate GUI</h1>

</div>

GUI for generating depth graphics. Use [MiDaS](https://github.com/isl-org/MiDaS) and [monodepth2](https://github.com/nianticlabs/monodepth2). Support Windows, Mac and Linux.

## Screenshots

![Screenshot](https://raw.githubusercontent.com/Afanyiyu/img/master/img-picgo/depth-estimate-gui-screenshot.png)

## Features

- [x] Generate depth graphics

- [x] Use [MiDaS](https://github.com/isl-org/MiDaS) and [monodepth2](https://github.com/nianticlabs/monodepth2)

- [x] Support common image formats for input/output

- [ ] Batch process (TODO)

## Matrix

### GUI Package Matrix

Windows x64|macOS x64|Linux x64
-|-|-
`gui-win-x64.zip` (:white_check_mark:)|`gui-osx-x64.zip` (:white_check_mark:)|`gui-linux-x64.zip` (:heavy_check_mark:)

### Tools Package Matrix

　|CUDA 10.2|CUDA 11.1|CPU (`cpuonly`)
-|-|-|-
Windows x64|`tools-win-x64-10.2.zip` (:white_check_mark:)|`tools-win-x64-11.1.zip` (:white_check_mark:)|`tools-win-x64-cpuonly.zip` (:white_check_mark:)
macOS x64|:x:|:x:|`tools-osx-x64-cpuonly.zip` (:white_check_mark:)
Linux x64|`tools-linux-x64-10.2.zip` (:heavy_check_mark:)|`tools-linux-x64-11.1.zip` (:heavy_check_mark:)|`tools-linux-x64-cpuonly.zip` (:heavy_check_mark:)

:white_check_mark:: Tested

:heavy_check_mark:: Supported

:x:: Unsupported

## Installation

You need to download 2 packages: **GUI Package** and **Tools Package**. Check the [Matrix](#matrix) section and find the suitable package for your computer.

If you do not have an NVIDIA graphic card, download the `cpuonly` package. Otherwise, check [CUDA Release Notes](https://docs.nvidia.com/cuda/cuda-toolkit-release-notes/index.html) to select CUDA version.

Go to [Releases](https://github.com/depth-estimate-gui/depth-estimate-gui/releases) to download packages. After downloading these two packages, **unzip** them, **merge** them and then start the `depth-estimate-gui` executable.

## BUGs & Issues

Feel free to [open issues](https://github.com/depth-estimate-gui/depth-estimate-gui/issues/new/choose). Please attach a **log** (`...` -> `Show Log` in editor window) when submitting a bug.

## Contributions

PRs are welcome! Feel free to contribute on this project.

## LICENSE

All files and codes **in this repo** use [MIT LICENSE](https://github.com/depth-estimate-gui/depth-estimate-gui/blob/master/LICENSE). For [MiDaS](https://github.com/isl-org/MiDaS) and [monodepth2](https://github.com/nianticlabs/monodepth2), please check their respective licenses.
