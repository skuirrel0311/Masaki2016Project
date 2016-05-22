using UnityEngine;

public class MicState : MonoBehaviour
{
    AppealAreaState appealArea;

    //耐久値
    int maxHp = 10;
    public int hp;

    void Start()
    {
        appealArea = GetComponentInParent<AppealAreaState>();
        Initialize();
    }

    public void Initialize()
    {
        hp = maxHp;
    }

    void Update()
    {
        if(hp == 0)
        {
            appealArea.Owner = null;
        }
    }
}
