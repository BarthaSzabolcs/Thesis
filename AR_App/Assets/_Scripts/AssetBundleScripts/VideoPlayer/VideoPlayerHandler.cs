using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerHandler : MonoBehaviour
{
    #region Hide in editor

    private VideoPlayer videoPlayer;
    private ContentHandler contentHandler;

    #endregion

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        contentHandler = transform.parent.GetComponent<ContentHandler>();

        contentHandler.OnTrackingStart += HandleTrackingStart;
        contentHandler.OnTrackingEnd += HandleTrackingEnd;
    }
    
    private void HandleTrackingStart()
    {
        videoPlayer.Play();
    }

    private void HandleTrackingEnd()
    {
        videoPlayer.Pause();
    }
}
