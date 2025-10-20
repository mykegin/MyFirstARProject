using UnityEngine;

public class HandAnimationController : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            anim.SetBool("isGrabbing", true);
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            anim.SetBool("isGrabbing", false);
        }
    }
}