# 🛠️ PingTool

A lightweight, command-line utility for continuous or limited ICMP ping testing with optional audio alerts. Built in C# for sysadmins and network engineers who want a no-frills diagnostic tool with just the right amount of feedback.

## 🚀 Features

- Continuous or fixed-count pinging  
- Customizable packet size (up to 65,500 bytes)  
- Audio alerts on success or failure  
- Linux-style output formatting  
- Graceful Ctrl+C handling with summary statistics  

## 📦 Usage

```bash
pingtool.exe <target> [-c count] [-s size] [-a success|failure]
```
### Arguments
| Option     | Description                                                  |
|------------|--------------------------------------------------------------|
| `<target>` | Required. IP address or hostname to ping.                    |
| `-c count` | Optional. Number of pings to send. Defaults to continuous.   |
| `-s size`  | Optional. Size of data payload in bytes. Default is 56.      |
| `-a mode`  | Optional. Play system sound on success or failure. Default is failure. |
```bash
Example
pingtool.exe 8.8.8.8 -c 10 -s 128 -a success
```
📊 Output

Formatted similar to Linux ping, including:

- ICMP sequence number
- TTL and round-trip time
- Summary with packet loss and RTT stats

Example output:
```
128 bytes from 8.8.8.8: icmp_seq=1 ttl=117 time=23 ms
128 bytes from 8.8.8.8: icmp_seq=2 ttl=117 time=22 ms
--- 8.8.8.8 ping statistics ---
10 packets transmitted, 10 received, 0.0% packet loss, time 10000ms
rtt min/avg/max/mdev = 21.5/22.3/23.1/0.6 ms
```
🔔 Audio Alerts

Uses built-in Windows system sounds:

- SystemSounds.Asterisk for success

- SystemSounds.Hand for failure

🧠 Why This Exists

Sometimes you just want a simple ping tool with audible feedback—especially when monitoring connectivity in the background. This tool is ideal for jump boxes, diagnostics, or even alerting during flaky Wi-Fi sessions.

🛠️ Requirements

- .NET 8 LTS Framework 

- Admin privileges not required

## 📄 License

 See [`LICENSE`](LICENSE) 

## 💰 Sustainability

To ensure long-term maintenance and support, this project will adopt the [Open Source Maintenance Fee](https://opensourcemaintenancefee.org/) model. This helps fund ongoing development while keeping the code open and transparent.
**Note:** This project will migrate to using [Open Source Maintenance Fee](https://opensourcemaintenancefee.org/) to help sustain development and support.
