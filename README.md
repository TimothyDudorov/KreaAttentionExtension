# KreaAttention for SwarmUI

A custom C# extension for SwarmUI that introduces advanced Krea-style attention mechanics, giving you direct control over prompt emphasis, de-emphasis, and advanced model weighting inside SwarmUI's generation pipeline.

---

## Prerequisites

Before installing this extension, ensure your system meets the following requirements:

### 1. ComfyUI Custom Nodes
This extension acts as a SwarmUI wrapper for Kijai's custom nodes. You must have the following pack installed in your active ComfyUI backend:
* **[ComfyUI-KJNodes](https://github.com/kijai/ComfyUI-KJNodes)**: This contains the underlying `Krea2PromptWeight` node that handles the model patching and text conditioning logic.
  * *To Install:* Use the ComfyUI Manager inside your backend and search for `ComfyUI-KJNodes`, or clone it directly into your ComfyUI `custom_nodes` directory:
    ```bash
    cd SwarmUI/dlbackend/comfy/ComfyUI/custom_nodes/
    git clone [https://github.com/kijai/ComfyUI-KJNodes.git](https://github.com/kijai/ComfyUI-KJNodes.git)
    ```

### 2. Software Requirements
* **SwarmUI:** A working, up-to-date installation of SwarmUI.
* **.NET 8.0 SDK:** SwarmUI dynamically compiles extension code on boot. If your development environment does not have it, download and install the [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

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
git clone [https://github.com/TimothyDudorov/KreaAttentionExtension.git](https://github.com/TimothyDudorov/KreaAttentionExtension.git)
```
---

## How to Use

1. Open the SwarmUI **Generate** tab.
2. In the parameter panel on the left (under the **Sampling** group), locate the **Krea Attention** section.
3. Toggle **Krea Attention Weighting** to **True**.
4. Adjust the **Krea Attention Strength** slider to control the global intensity multiplier.

---

## Prompting Examples & Use Cases

Krea-style attention works differently than standard ComfyUI weighting. Instead of just multiplying prompt embeddings which can quickly burn or distort the image, it uses Kijai's specialized scaling logic to dynamically adjust attention layers. 

### Example 1: Boosting Fine Details without "CFG Burn"
If you want to make highly detailed elements pop without ruining the contrast of your image:
* **Prompt:** `A futuristic cyberpunk city street, raining, hyper-detailed neon signs, 8k resolution, cinematic lighting`
* **Settings:**
  * **Krea Attention Weighting:** `True`
  * **Krea Attention Strength:** `1.4` (A mild boost)
* **What to look for:** The neon signs and rain reflections will become significantly sharper and more defined, but the darker areas of the street won't get deep-fried or oversaturated like they would if you simply raised the CFG scale.

### Example 2: Bringing Out Subtle Textures
Great for organic textures like fur, fabric, or skin.
* **Prompt:** `Close up portrait of a majestic snow leopard in a blizzard, highly detailed soft fur, intense green eyes`
* **Settings:**
  * **Krea Attention Weighting:** `True`
  * **Krea Attention Strength:** `1.8` (Strong boost)
* **What to look for:** The individual strands of fur and the falling snowflakes will gain distinct physical presence and texture separation, making the overall render feel much more tactile.

### Example 3: Subduing an Overpowering Prompt
If your prompt is too aggressive and you want to dial back its overall impact:
* **Prompt:** `A vibrant, chaotic watercolor splash painting of a sailing ship in a storm`
* **Settings:**
  * **Krea Attention Weighting:** `True`
  * **Krea Attention Strength:** `0.6` (De-emphasis)
* **What to look for:** The chaotic watercolor element is softened and blended, letting the underlying structural shape of the ship anchor the composition.

---

## Under the Hood: How it Works

When you hit Generate, the extension intercepts your pipeline:
1. It takes your raw text prompt and feeds it to Kijai's `Krea2PromptWeight` node alongside your active `MODEL` and `CLIP` models.
2. It processes the attention vectors using your designated **Strength** value.
3. It bypasses Swarm's default conditioning block for the positive prompt, passing the Krea-weighted conditioning stream directly into the sampler.