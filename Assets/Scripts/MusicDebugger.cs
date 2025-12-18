using UnityEngine;

// TEMPORARY DEBUG SCRIPT
// Add this to any GameObject in Sandbox scene to check music state
public class MusicDebugger : MonoBehaviour
{
    void Start()
    {
        Debug.Log("========== MUSIC DEBUGGER START ==========");
        
        if (MusicManager.Instance == null)
        {
            Debug.LogError("[MusicDebugger] MusicManager.Instance is NULL!");
            return;
        }
        
        Debug.Log("[MusicDebugger] MusicManager.Instance exists ✅");
        
        // Check if methods exist (will cause compile error if not)
        try
        {
            bool isMuted = MusicManager.Instance.IsMuted();
            Debug.Log($"[MusicDebugger] IsMuted: {isMuted}");
            
            bool isPlaying = MusicManager.Instance.IsPlaying();
            Debug.Log($"[MusicDebugger] IsPlaying: {isPlaying}");
            
            Debug.Log("[MusicDebugger] All methods exist ✅");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[MusicDebugger] Method missing! {e.Message}");
        }
        
        Debug.Log("========== MUSIC DEBUGGER END ==========");
    }
    
    void Update()
    {
        // Press 'M' to manually stop music
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("[MusicDebugger] Manually calling StopMusic()...");
            MusicManager.Instance.StopMusic();
        }
        
        // Press 'P' to check if playing
        if (Input.GetKeyDown(KeyCode.P))
        {
            bool isPlaying = MusicManager.Instance.IsPlaying();
            Debug.Log($"[MusicDebugger] IsPlaying: {isPlaying}");
        }
    }
}