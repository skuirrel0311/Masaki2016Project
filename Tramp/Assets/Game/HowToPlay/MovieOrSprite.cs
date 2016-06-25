using UnityEngine;

public class MovieOrSprite : MonoBehaviour
{
    public Sprite sprite;
    public MovieTexture movie;

    bool IsSprite { get { return sprite != null; } }
    bool IsMovie { get { return movie != null; } }

    void Start()
    {
        if (IsMovie) movie.loop = true;
    }

    public void Play()
    {
        movie.Play();
    }

    public void Stop()
    {
        movie.Stop();
    }

    void OnGUI()
    {
        if (IsMovie)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), movie);
        }
    }
}
