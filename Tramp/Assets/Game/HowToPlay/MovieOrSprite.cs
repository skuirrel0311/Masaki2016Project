using UnityEngine;

public class MovieOrSprite : MonoBehaviour
{
    public Sprite sprite;
    public MovieTexture movie;

    bool IsSprite { get { return sprite != null; } }
    bool IsMovie { get { return movie != null; } }

    public bool isText=false;

    public Texture tex;

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
            if(isText)
            {
                GUI.DrawTexture(new Rect(Screen.width*0.17f,Screen.height*0.85f,Screen.width*0.65f,Screen.height*0.15f),tex);

            }
        }
    }
}
