using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

    [SerializeField]
    float speed=10;

    Animator anim;
    AnimatorStateInfo stateInfo;

    enum AnimationState
    {
        wait,run
    }
    AnimationState animationState;

	// Use this for initialization
	void Start () {
        anim = transform.GetComponent<Animator>();
        animationState = AnimationState.wait;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");



        Vector3 vec = new Vector3(horizontal, 0, vertical);

        if (vec == Vector3.zero)
        {
            if (animationState == AnimationState.run)
            {
                anim.CrossFadeInFixedTime("WAIT00", 0.3f);
                animationState = AnimationState.wait;
            }
        }
        else
        {
            if (animationState == AnimationState.wait)
            {
                anim.CrossFadeInFixedTime("RUN00_F", 0.3f);
                animationState = AnimationState.run;
            }
        }

        vec *= Time.deltaTime * speed;

        transform.Translate(vec);

    }
}
