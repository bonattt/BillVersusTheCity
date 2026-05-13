using UnityEngine;

public static class DebugUtils
{
    // utils using unity `Debug`
    
    public static void DrawDebugStar(Vector3 point, Color c) {
        Vector3 p = new Vector3(0.5f, 0, 0.5f); 
        Debug.DrawLine(point + p, point - p, c);
        
        p = new Vector3(0.5f, 0, -0.5f);
        Debug.DrawLine(point + p, point - p, c);
        
        p = new Vector3(0.75f, 0, 0f);
        Debug.DrawLine(point + p, point - p, c);
        
        p = new Vector3(0f, 0, 0.75f);
        Debug.DrawLine(point + p, point - p, c);
        
        p = new Vector3(0f, 0.75f, 0f);
        Debug.DrawLine(point + p, point - p, c);
    }

    public static void DrawBlockedRay(Vector3 start, Vector3 end, Vector3 blockage) 
            => DrawBlockedRay(start, end, blockage, Color.blue, Color.yellow, Color.red);
    public static void DrawBlockedRay(Vector3 start, Vector3 end, Vector3 blockage, Color color_clear, Color color_blocked, Color star_color) {
        Debug.DrawLine(start, blockage, color_clear);
        Debug.DrawLine(blockage, end, color_blocked);
        DrawDebugStar(blockage, star_color);
    }
}
