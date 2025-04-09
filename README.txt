This guide explains how to clone, open, and build this Unity project from GitHub.

### Prerequisites

Make sure you have the following installed:

- [Unity Hub](https://unity.com/download)
- Unity Editor (**Recommended Version:** [Insert Unity Version])
- [Git](https://git-scm.com/downloads)

Optional:
- Visual Studio (for script editing)

---

### 1. Clone the Repository

Open a terminal or Git Bash and run:

```bash
git clone https://github.com/YourUsername/YourRepositoryName.git


### 2. Open the Project in Unity

Open Unity Hub.

Click Add Project.

Navigate to the cloned repository folder and select it.

Open the project.
Unity may take some time to import assets when opening for the first time.

### 3. Set the Build Target (Optional)

If you want to deploy to a specific platform:

Go to File → Build Settings.

Choose your platform (e.g., PC, Android, iOS).

Click Switch Platform.

Make sure there are no errors in the Console window.


###4. Build the Project

To build the project:

Go to File → Build Settings.

Add your game scenes to the Scenes In Build list.

Choose your target platform(all scenes)

Click Build.

Select an output folder and wait for Unity to build the project.

The final build (e.g., .exe, .apk, etc.) will appear in the output folder.

###. If Motion Input Window Doesn't Open

del C:\Users\user\ProjectName\Assets\StreamingAssets\MotionInput\matplotlib\mpl-data\stylelib\._*.mplstyle

Run this script on cd C:\Users\UserName\ProjectName\Assets\StreamingAssets\MotionInput

The issue is windows not being compatiable with ios


