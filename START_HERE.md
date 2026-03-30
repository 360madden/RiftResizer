# Start Here

RiftResizer is being built as a **Windows layout manager for multiple RIFT clients**.

## What Exists Right Now
- project scope and architecture docs
- external reference notes based on ISBoxer and OpenMultiBoxing layout concepts
- a first-pass .NET starter codebase for:
  - display detection
  - RIFT window detection
  - tiled layout generation for 1 to 5 windows
  - JSON profile save/load
  - layout application through Win32

## First Useful Commands
From the repo root:

```powershell
dotnet run --project .\\src\\RiftResizer.App\\RiftResizer.App.csproj -- displays
dotnet run --project .\\src\\RiftResizer.App\\RiftResizer.App.csproj -- scan
dotnet run --project .\\src\\RiftResizer.App\\RiftResizer.App.csproj -- generate 5
dotnet run --project .\\src\\RiftResizer.App\\RiftResizer.App.csproj -- save-profile fivebox 5
dotnet run --project .\\src\\RiftResizer.App\\RiftResizer.App.csproj -- apply-profile fivebox
```

## Immediate Reality Check
This is a starter implementation, not a finished product.
The first thing to test is whether RIFT window detection is reliable on the target machine.
