# Validation Checklist

## Immediate Objective
Verify that RiftResizer can detect RIFT windows, report monitor information, generate tile rectangles, save profiles, and apply layouts.

## Step 1
Run bootstrap.ps1.
Then build the solution.

Expected result:
- solution and project files exist
- build completes without project-file errors

## Step 2
Run the displays command.

Expected result:
- all monitors are listed
- the primary monitor is identified correctly
- work-area values look correct

## Step 3
Launch one or more RIFT clients.
Run the scan command.

Expected result:
- RIFT windows are detected consistently
- process names and titles look plausible
- the number of detected windows matches reality

## Step 4
Run the generate command for counts 1 through 5.

Expected result:
- generated slots remain inside the target display work area
- no overlaps occur unless deliberately designed
- output remains stable across repeated runs

## Step 5
Save a profile and then list profiles.

Expected result:
- profile file is created under the user's AppData path
- the profile name appears in the list output

## Step 6
Apply the saved profile while RIFT windows are open.

Expected result:
- windows move into the intended layout slots
- no windows land off-screen
- mismatch messages are understandable when the number of live windows does not match the saved profile

## First Bugs To Fix
- missed RIFT window detection
- wrong monitor or work-area selection
- incorrect rectangle math for generated slots
- profile load or save failures
- failures on minimized windows or unusual client states
