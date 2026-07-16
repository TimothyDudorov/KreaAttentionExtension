# KreaAttention for SwarmUI

A custom C# extension for SwarmUI that introduces advanced Krea-style attention mechanics, giving you direct control over prompt emphasis, de-emphasis, and advanced model weighting inside SwarmUI's generation pipeline.

---

## Features

* **Custom Attention Weighting:** Easily handle complex prompt structures without breaking standard generation flow.
* **Seamless Integration:** Adds parameters directly to SwarmUI's parameter lists and UI tab.
* **No Manual Comfy Setup:** Hooks directly into your backend generation pipeline automatically.

---

## Installation

### Method 1: Via SwarmUI Extensions Manager (Recommended)
Once registered in the global directory, you can install this directly from the **Server -> Extensions** tab inside SwarmUI.

### Method 2: Manual Git Clone
If you are installing or testing manually, clone this repository directly into your SwarmUI extensions directory:

```bash
cd SwarmUI/src/Extensions/
git clone [https://github.com/YOUR_GITHUB_USERNAME/KreaAttentionExtension.git](https://github.com/YOUR_GITHUB_USERNAME/KreaAttentionExtension.git)