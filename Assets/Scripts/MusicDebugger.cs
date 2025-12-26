using UnityEngine;

// TEMPORARY DEBUG SCRIPT
// Add this to any GameObject in Sandbox scene to check music state
public class MusicDebugger : MonoBehaviour
{
    void Start()
    {
        
        if (MusicManager.Instance == null)
        {
            return;
        }
        
        
        // Check if methods exist (will cause compile error if not)
        try
        {
            bool isMuted = MusicManager.Instance.IsMuted();
            
            bool isPlaying = MusicManager.Instance.IsPlaying();
        }
        catch (System.Exception e)
        {
        }
        
    }
    
    void Update()
    {
        // Press 'M' to manually stop music
        if (Input.GetKeyDown(KeyCode.M))
        {
            MusicManager.Instance.StopMusic();
        }
        
        // Press 'P' to check if playing
        if (Input.GetKeyDown(KeyCode.P))
        {
            bool isPlaying = MusicManager.Instance.IsPlaying();
        }
    }
}