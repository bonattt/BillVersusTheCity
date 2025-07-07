

using System.IO;
using UnityEngine;
using UnityEngine.Video;

public static class VideoLibrary {

    public const string TUTORIAL_VIDEO_RESOURCE_PATH = "TutorialVideos";

    public static VideoClip GetTutorialVideo(string tutorial_name) {
        // gets a video clip from a tutorial name.
        string resource_path = VideoResourcePath(tutorial_name);
        return Resources.Load<VideoClip>(resource_path);
    }

    public static string VideoResourcePath(string tutorial_name) {
        return Path.Combine(TUTORIAL_VIDEO_RESOURCE_PATH, tutorial_name);
    }

    public static bool HasTutorialVideo(string tutorial_name) {
        return GetTutorialVideo(tutorial_name) != null;
    }
}