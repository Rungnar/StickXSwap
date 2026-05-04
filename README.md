# StickXSwap

[![GitHub release](https://img.shields.io/github/v/release/Rungnar/StickXSwap)](https://github.com/Rungnar/StickXSwap/releases/latest)
[![License](https://img.shields.io/badge/license-GPL--3.0-blue)](https://www.gnu.org/licenses/gpl-3.0.html)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

Lightweight Windows input utility that swaps the X-axis of both analog sticks on an Xbox-compatible (XInput) controller in real time using virtual controller emulation via ViGEmBus. It runs in the system tray and supports toggle switching with hotkey and on-screen status indicator.

The physical controller should be plugged in before launching StickXSwap. Supports XInput-compatible controllers only.

---

## Downloads

The latest build can be downloaded from the [Releases](https://github.com/Rungnar/StickXSwap/releases) page.

When using releases, ensure all dependencies are installed:

- ViGEmBus must be installed and running
- HidHide must be configured correctly to hide the physical controller while allowing StickXSwap access

---

## Requirements

- [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [ViGEmBus Driver](https://github.com/ViGEm/ViGEmBus/releases)
- [HidHide](https://github.com/ViGEm/HidHide/releases)

---

## Installation

- Install ViGEmBus
- Install HidHide
- Build or download StickXSwap
- Run the executable
- Use Alt + F7 to toggle swap ON/OFF

---

## HidHide Setup Notes

When using HidHide:

- Install HidHide
- Open HidHide Configuration Client as administrator
- Select your physical controller under Devices
- Add `StickXSwap.exe` under Applications
- Enable device hiding

Incorrect configuration may result in one or more of the following issues:

- Double input in games  
- Controller not detected in-game  
- X-axis swap not being applied

---

## Controls

- **Alt + F7** → Toggle X-axis swap  
- System tray menu → Toggle / Exit  

---

## Notes

- Only XInput-compatible controllers (Xbox controllers and equivalents) are supported
- The current version does not support rumble
- Some games may require administrator privileges  
- Exclusive fullscreen mode may affect input behavior  
- HidHide must be configured correctly for proper operation  
- Designed for low-latency real-time controller remapping  
- The controller should be connected before launching StickXSwap
- If there is input duplication, no input detection, the X-axis swap is not applied, or toggle hotkey is unresponsive, see: [TROUBLESHOOTING](./TROUBLESHOOTING.md)
  
---

## License

This project is licensed under the GNU General Public License v3.0 (GPL-3.0). See the [LICENSE](LICENSE) file for full details.
